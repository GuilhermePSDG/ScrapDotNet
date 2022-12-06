


using Requests.Models;

namespace Requests.Contracts;

public interface IRequestResultHandler<T>
{
    public Task HandleSuccessAsync(T data, RequestResult context, IBaseProcessor requestProcessor);
    public Task<Next> HandleFailureAsync(RequestResult result, IBaseProcessor requestProcessor);
    public Task EndOfWorkAsync();
}
