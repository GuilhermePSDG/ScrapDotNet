namespace ModelBuilder.Models;
public class ModelDescription
{
    public Dictionary<string, List<MemberDescription>> members = new();
    public Dictionary<string, List<ModelDescription>> complex_members = new();
    public string? ContainerSelector { get;  set; }
}


