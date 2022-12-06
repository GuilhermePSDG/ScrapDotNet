namespace Html;

public interface IHtmlLoader
{
    public IPerformQuerySelection Load(string html);
    public IPerformQuerySelection Load(Stream stream);
}
