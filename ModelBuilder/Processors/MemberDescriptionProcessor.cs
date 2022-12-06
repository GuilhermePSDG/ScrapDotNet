using Html;
using ModelBuilder.Models;

namespace ModelBuilder.Processors;

public class MemberDescriptionProcessor
{
    public virtual IEnumerable<string?> Process(MemberDescription member, IPerformQuerySelection element)
    {
        var selected_element = GetElements(member, element);

        foreach (var el in selected_element)
        {
            yield return GetValue(member, el, element);
        }
    }

    private static IEnumerable<IScrapElement?> GetElements(MemberDescription member, IPerformQuerySelection element)
    {
        if (member.QuerybleDescription.ContainerSelector != null)
        {
            element = element.QuerySelector(member.QuerybleDescription.ContainerSelector);
        }
        if (member.ValueDescription.IsCollection)
        {
            foreach (var el in element.QuerySelectorAll(member.QuerybleDescription.QuerySelector))
            {
                yield return el;
            }
        }
        else
        {
            yield return element.QuerySelector(member.QuerybleDescription.QuerySelector);
        }
    }

    private string? GetValue(MemberDescription member, IScrapElement? selected_element, IPerformQuerySelection rootQuery)
    {
        string? value = null;

        if (selected_element != null)
        {
            value = GetValueResult(member, selected_element);
            if (member.ConcatMember != null)
            {
                value += Process(member.ConcatMember, rootQuery).FirstOrDefault();
            }

        }

        value ??= member.Options.DefaultValue?.ToString();

        if (value == null && !member.Options.AllowNull)
        {
            throw new InvalidOperationException($"The value of the member \"{member.ValueDescription.MemberName}\" is null and the property \"AllowNull\" is false");
        }
        else
        {
            if (!member.Options.TryProcessOptions(ref value))
            {
                throw new Exception($"Validation failed for {member.ValueDescription.MemberName} with value {value}");
            }
            return value;
        }



    }

    private static string? GetValueResult(MemberDescription member, IScrapElement selected_element)
    {
        string? value;
        switch (member.QuerybleDescription.Location)
        {
            case MemberValueLocation.InnerHtml:
                value = selected_element.GetHtml();
                break;
            case MemberValueLocation.InnerText:
                value = selected_element.GetText();
                break;
            case MemberValueLocation.Attribute:
                if (member.QuerybleDescription.LocationValue == null) throw new InvalidOperationException($"For \"{member.ValueDescription.MemberName.ToString()}\" \"{nameof(MemberDescription.ValueDescription.MemberName)}\" cannot be null");
                value = selected_element.GetAtribute(member.QuerybleDescription.LocationValue);
                break;
            default: throw new InvalidOperationException($"{member.QuerybleDescription.Location.ToString()} is not suported.");
        }

        return value;
    }





}
