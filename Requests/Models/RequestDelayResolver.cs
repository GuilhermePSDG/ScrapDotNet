using Microsoft.Extensions.Logging;

namespace Requests.Models;
public class RequestDelayResolver
{
    private protected readonly ILogger<RequestDelayResolver>? logger;
    private protected readonly RequestProcessorOptions options;
    private protected int successesSinceLastThrottle;
    private protected int currentBackoff;

    public RequestDelayResolver(ILogger<RequestDelayResolver>? logger = null, RequestProcessorOptions? options = null)
    {
        this.logger = logger;
        this.options = options ?? RequestProcessorOptions.Zero;
    }

    public virtual void HandleRequestContext(RequestContext requestContext)
    {
        if (options.ExcedThrottleTimeout(requestContext.Timer.Elapsed))
        {
            IncressThrottling();
            logger?.LogInformation($"Increased backoff to {currentBackoff}ms.");
        }
        else if (currentBackoff > 0)
        {
            successesSinceLastThrottle += 1;

            if (options.CanMinimiseThrottling(successesSinceLastThrottle))
            {
                var newBackoff = currentBackoff - options.ThrottlingRequestBackoff.TotalMilliseconds;
                currentBackoff = Math.Max(0, (int)newBackoff);
                successesSinceLastThrottle = 0;
                logger?.LogInformation($"Decreased backoff to {currentBackoff}ms.");
            }
        }
    }

    public virtual void IncressThrottling()
    {
        successesSinceLastThrottle = 0;
        currentBackoff += (int)options.ThrottlingRequestBackoff.TotalMilliseconds;
    }

    public virtual double GenerateNewStartDelay()
    {
        var requestStartDelay = 0d;
        if (options.HasDelayBetweenRequestStart())
        {
            requestStartDelay += options.DelayBetweenRequestStart.TotalMilliseconds;
            requestStartDelay += options.GetRandomDelay().TotalMilliseconds;
        }
        requestStartDelay += currentBackoff;
        return requestStartDelay;
    }

}
