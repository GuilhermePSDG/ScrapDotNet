namespace Html;

public interface IScrapElement : IPerformQuerySelection, IHaveAtributes
{
    public string? GetText();
    public string? GetHtml();
}
