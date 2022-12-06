using System.Diagnostics;
namespace Requests.Models;

public class RequestContext
{
    public int RequestNumber { get; set; }
    public Request Request { get; set; }
    public Stopwatch Timer { get; } = new Stopwatch();
    public double RequestStartDelay { get; set; }
    public TimeSpan RequestTimeout { get; set; }
    public CancellationToken CancellationToken { get; set; }


    public async Task WaitStartDelay()
    {
        if (RequestStartDelay > 0)
        {
            await Task.Delay((int)RequestStartDelay);
            RequestStartDelay = 0;
        }
    }
    public CancellationToken CreateCombinedTokenWithRequestTimeOut()
    {
        var timeoutToken = new CancellationTokenSource(RequestTimeout).Token;
        return CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, timeoutToken).Token;
    }

}
