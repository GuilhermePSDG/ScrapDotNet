using Microsoft.Extensions.Logging;
using Requests.Contracts;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

namespace Requests.Models;
   
public class RequestProcessor : IRequestProcessor
    {
        private RequestResultHandler handler;

        private readonly ConcurrentDictionary<Task<RequestResult>, RequestContext> activeRequests;
        private readonly ConcurrentQueue<Request> RequestQueue;
        private int requestCount;
        
        private HttpClient httpClient;
        private readonly Func<Task<HttpClient>> clientFactory;
        
        private readonly RequestProcessorOptions RequestOptions;
        private readonly RequestDelayResolver requestDelayResolver;
        
        private readonly ILogger<RequestProcessor>? logger;
        public bool IsRunning { get; private set; } = false;
        
        public int PendingRequests => RequestQueue.Count;
        public int ActiveRequests => activeRequests.Count;

        public int TotalRequestsAdded { get; private set; }
        public Task RunningTask { get; private set; }

        public RequestProcessor(
            ILogger<RequestProcessor>? logger = null,
            RequestDelayResolver? requestDelayResolver = null,
            Func<Task<HttpClient>>? clientFactory = null,
            RequestProcessorOptions? options = null
            )
        {
            this.RequestOptions = options ?? RequestProcessorOptions.Zero;
            this.requestDelayResolver = requestDelayResolver ?? new(null, RequestOptions);
            this.clientFactory = clientFactory ?? (static () => Task.FromResult(new HttpClient()));

            this.logger = logger;
            this.requestCount = new();
            this.RunningTask = Task.CompletedTask;
            this.RequestQueue = new();
            activeRequests = new (RequestOptions.MaxNumberOfSimultaneousRequests, RequestOptions.MaxNumberOfSimultaneousRequests);
            this.httpClient =  this.clientFactory().Result;
        }

        private RequestProcessor() { }

        public virtual void AddRequest(Request request)
        {
            TotalRequestsAdded++;
            RequestQueue.Enqueue(request);
        }

        public virtual void AddRequest(IEnumerable<Request> requests)
        {
            foreach (var request in requests)
                AddRequest(request);
        }

        public virtual async Task ProcessAsync(RequestResultHandler handler,CancellationToken cancellationToken = default)
        {
            this.handler = handler;
            logger?.LogInformation("Start processing");
            while (!activeRequests.IsEmpty || !RequestQueue.IsEmpty)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ProcessQueue(cancellationToken);
                await ProcessActiveRequest(cancellationToken).ConfigureAwait(false);
            }
            logger?.LogInformation("Completed processing");
            IsRunning = false;
        }

        private protected virtual void ProcessQueue(CancellationToken cancellationToken)
        {
            while (!RequestQueue.IsEmpty)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!RequestQueue.TryDequeue(out var request)) continue;
                var requestStartDelay = requestDelayResolver.GenerateNewStartDelay();
                var requestContext = new RequestContext
                {
                    RequestNumber = requestCount + 1,
                    Request = request,
                    RequestStartDelay = requestStartDelay,
                    RequestTimeout = RequestOptions.RequestTimeout,
                    CancellationToken = cancellationToken,
                };

                logger?.LogInformation("Request #{@requestNumber}/{@totalRequests} - starting with a {@requestStartDelay}ms delay.", requestContext.RequestNumber, TotalRequestsAdded, requestStartDelay);

                var requestResultTask = PerformRequestAsync(requestContext);
                activeRequests.TryAdd(requestResultTask, requestContext);
                requestCount++;
                if (RequestOptions.ExcedMaxNumberOfSimultaneousRequests(activeRequests.Count)) break;
            }
        }

        private protected virtual async Task ProcessActiveRequest(CancellationToken cancellationToken)
        {
            await Task.WhenAny(activeRequests.Keys).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            var completedRequests = activeRequests.Where(t => t.Key.IsCompleted);
            foreach (var completedRequest in completedRequests)
            {
                if (!activeRequests.TryRemove(completedRequest.Key, out var requestContext)) continue;

                if (completedRequest.Key.IsFaulted) ExceptionDispatchInfo.Capture(completedRequest.Key.Exception!.InnerException!).Throw();

                var nextBehavior = await handler(completedRequest.Key.Result);

                await ResolveNext(nextBehavior, requestContext);

                requestDelayResolver.HandleRequestContext(requestContext);
            }
        }

        private protected virtual async Task ResolveNext(Next behavior, RequestContext ctx)
        {
            switch (behavior)
            {
                case Next.Continue:
                    return;
                case Next.Retry:
                    AddRequest(ctx.Request);
                    return;
                case Next.IncressThrottling:
                    requestDelayResolver.IncressThrottling();
                    return;
                case Next.ChangeHttpClient:
                    AddRequest(ctx.Request);
                    requestDelayResolver.IncressThrottling();
                    httpClient = await clientFactory();
                    return;
                case Next.ClearQueue:
                    RequestQueue.Clear();
                    break;
            }
        }

        private protected virtual async Task<RequestResult> PerformRequestAsync(RequestContext context)
        {
            await context.WaitStartDelay();
            var result = new RequestResult()
            {
                Request = context.Request,
                RequestStart = DateTime.UtcNow,
                RequestStartDelay = context.RequestStartDelay,
            };
            var requestToken = context.CreateCombinedTokenWithRequestTimeOut();
            try
            {
                context.Timer.Start();
                var httpResponseMsg = await httpClient.GetAsync(context.Request.Uri, requestToken);
                context.Timer.Stop();
                result.ElapsedTime = context.Timer.Elapsed;
                await result.FillWithResponseMessageAsync(httpResponseMsg);

                if (context.CancellationToken.IsCancellationRequested)
                {
                    result.WasCanceled = true;
                    context.CancellationToken.ThrowIfCancellationRequested();
                }

                logger?.LogInformation("Request #{@requestNumber}/{@totalRequests} - elapsed {@elapsedMs}ms .", context.RequestNumber, TotalRequestsAdded, context.Timer.ElapsedMilliseconds);

            }
            catch (Exception ex)
            {
                result.Exception = ex;
                logger?.LogInformation("Request #{@requestNumber}/{@totalRequests} - elapsed {@elapsedMs}ms FAILED, Exception : {@exception}.", context.RequestNumber, TotalRequestsAdded, context.Timer.ElapsedMilliseconds, ex.Message);
            }
            finally
            {
                if (context.Timer.IsRunning)
                {
                    context.Timer.Stop();
                    result.ElapsedTime = context.Timer.Elapsed;
                }
            }
            return result;
        }
    }
