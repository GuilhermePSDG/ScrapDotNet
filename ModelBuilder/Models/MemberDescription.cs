
namespace ModelBuilder.Models;

public class MemberDescription
{
    public QuerybleDescription QuerybleDescription { get; set; } = new();

    public MemberDescriptionOptions Options { get; set; } = new();

    public ValueDescription ValueDescription { get; set; } = new();

    public MemberDescription? ConcatMember { get; set; }
}

