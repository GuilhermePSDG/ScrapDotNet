namespace ModelBuilder.Models;
public class QuerybleDescription
{
    public string? ContainerSelector { get; internal set; }
    public string QuerySelector { get; set; }
    public MemberValueLocation Location { get; set; } = MemberValueLocation.InnerHtml;
    public string? LocationValue { get; set; }
}

