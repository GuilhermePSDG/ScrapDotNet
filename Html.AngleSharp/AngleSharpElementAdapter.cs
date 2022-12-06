using AngleSharp.Dom;

namespace Html.Adapters;

public class AngleSharpElementAdapter : IScrapElement
{
    private IElement element;
    public AngleSharpElementAdapter(IElement element) => this.element = element;

    public static AngleSharpElementAdapter? CreateOrNull(IElement? element)
    {
        if (element == null) return null;
        return new AngleSharpElementAdapter(element);
    }
    public AngleSharpElementAdapter() { }

    public string? GetAtribute(string name) => element.GetAttribute(name);

    public string? GetHtml() => element.InnerHtml;

    public string? GetText() => element.TextContent;

    public IScrapElement? QuerySelector(string path) => CreateOrNull(element.QuerySelector(path));
    public IEnumerable<IScrapElement> QuerySelectorAll(string path) => element.QuerySelectorAll(path).Select(e => new AngleSharpElementAdapter(e));
}
