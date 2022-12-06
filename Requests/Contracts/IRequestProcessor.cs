using Requests.Models;

namespace Requests.Contracts;

public delegate Task<Next> RequestResultHandler(RequestResult res);

public interface IRequestProcessor : IBaseProcessor
{
    Task ProcessAsync(RequestResultHandler handler,CancellationToken cancellationToken = default);
}

public interface IRequestProcessor<T> : IBaseProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken = default);
}

public interface IBaseProcessor
{
    int PendingRequests { get; }
    int TotalRequestsAdded { get; }
    bool IsRunning { get; }
    int ActiveRequests { get; }

    void AddRequest(Request request);
    void AddRequest(IEnumerable<Request> requests);
}
