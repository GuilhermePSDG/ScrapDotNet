

using Requests.Contracts;

namespace Requests.Models;

public class TypedRequestProcessor<T> : IRequestProcessor<T>
{
    private readonly IRequestProcessor processor;
    private readonly IRequestResultParser<T> parser;
    private readonly IRequestResultHandler<T> handler;
    
    public bool IsRuning => processor.IsRunning;
    public int PendingRequests => processor.PendingRequests;
    public int TotalRequestsAdded => processor.TotalRequestsAdded;
    public bool IsRunning => processor.IsRunning;
    public int ActiveRequests => processor.ActiveRequests;

    public TypedRequestProcessor(
        IRequestResultParser<T> parser,
        IRequestResultHandler<T> handler,
        IRequestProcessor? requestProcessor = null
        )
    {
        this.processor = requestProcessor ?? new RequestProcessor();
        this.parser = parser;
        this.handler = handler;
    }

    public virtual async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        await this.processor.ProcessAsync(HandleAsync, cancellationToken);
        await this.handler.EndOfWorkAsync();
    }

    public void AddRequest(Request request) => this.processor.AddRequest(request);
    public void AddRequest(IEnumerable<Request> requests) => this.processor.AddRequest(requests);
    

    protected virtual async Task<Next> HandleAsync(RequestResult res)
    {
        Next nextBehavior = 0;

        if (res.Succeeded)
        {
            if (parser.TryParse(res, out var ex) is T parsed)
            {
                await handler.HandleSuccessAsync(parsed, res,this.processor);
            }
            else
            {
                if (ex != null) res.Exception = ex;
                res.ParsingFailed = true;
                nextBehavior = await handler.HandleFailureAsync(res, this.processor);
            }
        }
        else
        {
            nextBehavior = await handler.HandleFailureAsync(res, this.processor);
        }
        return nextBehavior;
    }

}
