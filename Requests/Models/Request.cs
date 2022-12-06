namespace Requests.Models;
public class Request
{
    public Request(string TraceId, Uri Uri)
    {
        this.TraceId = TraceId;
        this.Uri = Uri;
    }
    public string TraceId { get; init; }
    public Uri Uri { get; init; }
}
