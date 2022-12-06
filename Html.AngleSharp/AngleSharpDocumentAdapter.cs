using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Text;

namespace Html.Adapters;

public class AngleSharpDocumentAdapter : IScrapHtmlDocument
{
    private IHtmlDocument document;

    public IPerformQuerySelection Load(string html)
    {
        document = new HtmlParser().ParseDocument(html);
        return this;
    }
    public IPerformQuerySelection Load(Stream stream) => Load(StreamToString(stream));

    private string StreamToString(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public IScrapElement? QuerySelector(string path) => AngleSharpElementAdapter.CreateOrNull(document.QuerySelector(path));

    public IEnumerable<IScrapElement> QuerySelectorAll(string path) => document.QuerySelectorAll(path).Select(x => new AngleSharpElementAdapter(x));
}
