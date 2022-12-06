
namespace Requests.Models;

public enum Next
{
    Continue,
    Retry,
    IncressThrottling,
    ChangeHttpClient,
    ClearQueue,
}
