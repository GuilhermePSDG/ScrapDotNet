
namespace Requests.Models;

public class RequestProcessorOptions
{
    public static RequestProcessorOptionsBuilder CreateBuilder() => RequestProcessorOptionsBuilder.Create();
    public static RequestProcessorOptions Default => new();
    public static RequestProcessorOptions Agresive => new()
    {
        DelayBetweenRequestStart = TimeSpan.FromMilliseconds(300),
        RandomDelay = TimeSpan.FromMilliseconds(800),
        MinSequentialSuccessesToMinimiseThrottling = 2,
        RequestTimeout = TimeSpan.FromSeconds(5),
        ThrottlingRequestBackoff = TimeSpan.FromMilliseconds(500),
        ThrottlingTimeout = TimeSpan.FromSeconds(5),
    };
    public static RequestProcessorOptions Zero => new()
    {
        DelayBetweenRequestStart = TimeSpan.FromMilliseconds(1),
        RandomDelay = TimeSpan.FromMilliseconds(1),
        MinSequentialSuccessesToMinimiseThrottling = 0,
        RequestTimeout = TimeSpan.FromMinutes(5),
        MaxNumberOfSimultaneousRequests = Environment.ProcessorCount,
        ThrottlingRequestBackoff = TimeSpan.FromMilliseconds(500),
        ThrottlingTimeout = TimeSpan.FromSeconds(5)
    };
    public static RequestProcessorOptions Safe => RequestProcessorOptions
          .CreateBuilder()
          .HasFixedDelayBeetwenEachRequest(TimeSpan.FromMilliseconds(200))
          .HasRandomDelayBeetwenEachRequest(TimeSpan.FromMilliseconds(1200))
          .HasMaxConcurrency(Environment.ProcessorCount)
          .ConfigureThrottling(throttlingBuilder =>
          {
              throttlingBuilder
              .WhenExcedTime(TimeSpan.FromMilliseconds(2000))
              .AplyDelay(TimeSpan.FromMilliseconds(500))
              .ThenMinizeWhen(NumberOfSuccedRequest: 4);
          }).Build();



    public int MaxNumberOfSimultaneousRequests { get; set; } = Environment.ProcessorCount;
    public TimeSpan DelayBetweenRequestStart { get; set; } = TimeSpan.FromMilliseconds(1000);
    public TimeSpan RandomDelay { get; set; } = TimeSpan.FromMilliseconds(1000);
    public TimeSpan ThrottlingTimeout { get; set; } = TimeSpan.FromMilliseconds(2500);
    public TimeSpan ThrottlingRequestBackoff { get; set; } = TimeSpan.FromSeconds(5);
    public int MinSequentialSuccessesToMinimiseThrottling { get; set; } = 5;
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public TimeSpan GetRandomDelay()
    {
        return TimeSpan.FromMilliseconds(RandomDelay.TotalMilliseconds * Random.Shared.NextDouble());
    }
    public bool HasDelayBetweenRequestStart()
    {
        return DelayBetweenRequestStart.TotalMilliseconds > 0;
    }
    public bool ExcedThrottleTimeout(TimeSpan requestElapsed)
    {
        return ThrottlingTimeout.TotalMilliseconds > 0 && requestElapsed > ThrottlingTimeout;
    }
    public bool CanMinimiseThrottling(int sequencialSucessCount)
    {
        return sequencialSucessCount >= MinSequentialSuccessesToMinimiseThrottling;
    }
    public bool ExcedMaxNumberOfSimultaneousRequests(int n)
    {
        return n >= this.MaxNumberOfSimultaneousRequests;
    }

}


public class RequestProcessorOptionsBuilder
{
    private RequestProcessorOptions opt;
    private RequestProcessorOptionsBuilder()
    {
        this.opt = RequestProcessorOptions.Default;
    }
    public static RequestProcessorOptionsBuilder Create()
    {
        return new RequestProcessorOptionsBuilder();
    }
    public RequestProcessorOptions Build()
    {
        return this.opt;
    }

    public RequestProcessorOptionsBuilder HasFixedDelayBeetwenEachRequest(TimeSpan timeSpan)
    {
        this.opt.DelayBetweenRequestStart = timeSpan;
        return this;
    }

    public RequestProcessorOptionsBuilder HasRandomDelayBeetwenEachRequest(TimeSpan MaxTime)
    {
        this.opt.RandomDelay = MaxTime;
        return this;
    }

    public RequestProcessorOptionsBuilder HasMaxConcurrency(int n)
    {
        this.opt.MaxNumberOfSimultaneousRequests = n;
        return this;
    }

    public RequestProcessorOptionsBuilder ConfigureThrottling(Action<ThrottlingWhenExcedTime> builder)
    {
        builder(ThrottlingBuilder.Create(this.opt));
        return this;
    }
}

public interface ThrottlingThenMinizeWhen
{
    public void ThenMinizeWhen(int NumberOfSuccedRequest);
}
public interface ThrottlingAplyDelay
{
    public ThrottlingThenMinizeWhen AplyDelay(TimeSpan timeSpan);
}
public interface ThrottlingWhenExcedTime
{
    public ThrottlingAplyDelay WhenExcedTime(TimeSpan timeSpan);
}
public class ThrottlingBuilder : ThrottlingThenMinizeWhen, ThrottlingAplyDelay, ThrottlingWhenExcedTime
{
    private readonly RequestProcessorOptions ctx;

    private ThrottlingBuilder(RequestProcessorOptions ctx)
    {
        this.ctx = ctx;
    }
    public static ThrottlingWhenExcedTime Create(RequestProcessorOptions ctx) => new ThrottlingBuilder(ctx);

    public void ThenMinizeWhen(int NumberOfSuccedRequest)
    {
        ctx.MinSequentialSuccessesToMinimiseThrottling = NumberOfSuccedRequest;
    }
    public ThrottlingThenMinizeWhen AplyDelay(TimeSpan timeSpan)
    {
        ctx.ThrottlingRequestBackoff = timeSpan;
        return this;
    }

    public ThrottlingAplyDelay WhenExcedTime(TimeSpan timeSpan)
    {
        this.ctx.ThrottlingTimeout = timeSpan;
        return this;
    }

}
