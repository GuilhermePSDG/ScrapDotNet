

using Requests.Models;

namespace Requests.Contracts;

public interface IRequestResultParser<TResult>
{
    public TResult? TryParse(RequestResult result, out Exception? exception);
}
