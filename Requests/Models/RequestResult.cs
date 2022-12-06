using System.Net;
using System.Net.Http.Headers;

namespace Requests.Models;

public class RequestResult
{
    public Request Request { get; set; }
    public DateTime RequestStart { get; set; }
    public double RequestStartDelay { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public HttpResponseHeaders ResponseHeaders { get; set; }
    public HttpContentHeaders ContentHeaders { get; set; }
    public Stream? Content { get; set; }
    public bool ParsingFailed { get; set; }
    
    object @lock = new object();
    public string? StringContent
    {
        get
        {
            if (this.Content == null)
                return null;
            if (string.IsNullOrEmpty(_StringContent))
            {
                lock (@lock)
                {
                    if (string.IsNullOrEmpty(_StringContent))
                    { 
                        Content.Seek(0, SeekOrigin.Begin);
                        _StringContent = new StreamReader(this.Content).ReadToEnd();
                    }
                }
            }
            return _StringContent;
        }
    }

    private string? _StringContent;

    public TimeSpan ElapsedTime { get; set; }
    public Exception? Exception { get; set; }
    public bool HasException => Exception != null;
    public bool Succeeded => !HasException && !WasCanceled && StatusCode.HasValue && (int)StatusCode.Value > 199 && (int)StatusCode.Value < 300;
    public bool WasCanceled { get; set; } = false;

    public async Task FillWithResponseMessageAsync(HttpResponseMessage response)
    {
        this.StatusCode = response.StatusCode;
        this.ResponseHeaders = response.Headers;
        this.ContentHeaders = response.Content.Headers;
        //
        this.Content = new MemoryStream();
        await response.Content.CopyToAsync(Content);
        Content.Seek(0, SeekOrigin.Begin);
        //
    }
}
