namespace Html;

public interface IPerformQuerySelection
{
    public IScrapElement? QuerySelector(string path);
    public IEnumerable<IScrapElement> QuerySelectorAll(string path);
}
