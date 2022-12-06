using Html;
using ModelBuilder.Models;
using System.Diagnostics.Metrics;
using System.Runtime.ExceptionServices;

namespace ModelBuilder.Processors;


public record ModelProcessedResult(Dictionary<string, object?> MembersValue, Dictionary<string, ModelProcessedResult?> ComplexMembersValue);

public static class ModelDescriptorResultParser
{
    public static object Parse(ModelProcessedResult result, Type type)
    {
        var context = Activator.CreateInstance(type)!;


        foreach(var member in result.MembersValue)
        {
            var property = type.GetProperty(member.Key) ?? throw new InvalidOperationException($"Property {member.Key} not found in type {type.Name}");
            property.SetValue(context, member.Value);
        };

        foreach(var member in result.ComplexMembersValue)
        {
            var property = type.GetProperty(member.Key) ?? throw new InvalidOperationException($"Property {member.Key} not found in type {type.Name}");
            property.SetValue(context, Parse(member.Value, property.PropertyType));
        };

        return context;
    }
}

public class ModelDescriptorProcessor : IModelDescriptorProcessor
{
    public T Process<T>(ModelDescription member, IPerformQuerySelection element)
    {
        var result = Process(member, element);    
        return (T)ModelDescriptorResultParser.Parse(result, typeof(T));
    }


    private ModelProcessedResult Process(ModelDescription member, IPerformQuerySelection? queryElement)
    {
        if (member.ContainerSelector != null)
        {
            queryElement = queryElement?.QuerySelector(member.ContainerSelector);
        }

        Dictionary<string, object?> MembersValue = new();
        Dictionary<string, ModelProcessedResult?> ComplexMembersValue = new();

        foreach(var alias_members in member.members)
        {
            var memberProcessor = new MemberDescriptionProcessor();

            Exception? ex = null;
            foreach (var value in alias_members.Value)
            {
                try
                {
                    var stringResult = memberProcessor.Process(value, queryElement);
                    var requiredType = ObjectParser.MemberTypeToType(value.ValueDescription.MemberType);
                    if (value.ValueDescription.IsCollection)
                    {
                        MembersValue[value.ValueDescription.MemberName] = ParseObject(value.ValueDescription.MemberType, stringResult);
                    }
                    else
                    {
                        var objt = ParseObject(value.ValueDescription.MemberType, stringResult?.FirstOrDefault());
                        if (!value.Options.PosValidated(objt!))
                        {
                            throw new InvalidOperationException($"Validation error for {value.ValueDescription.MemberName}, value : {objt}");
                        }
                        MembersValue[value.ValueDescription.MemberName] = objt;
                    }
                    ex = null;
                    break;
                }
                catch (Exception exception)
                {
                    ex = exception;
                }
            }
            if (ex != null)
                ExceptionDispatchInfo.Throw(ex);
        };

        foreach (var complex_member in member.complex_members)
        {
            foreach (var complex_member_alias in complex_member.Value)
            {
                var result = Process(complex_member_alias, queryElement);
                ComplexMembersValue[complex_member.Key] = result;
            }
        }

        return new(MembersValue, ComplexMembersValue);
    }



    private static object? ParseObject(ObjectType type,string value)
    {
        return ObjectParser.ParseToObject(type,value);
    }

    private static object? ParseObject(ObjectType type, IEnumerable<string> value)
    {
        var real_primitive_type = ObjectParser.MemberTypeToType(type);
        var ObjectCollection = value
                  .Select(x => ObjectParser.ParseToObject(type, x))
                  .Select(x => Convert.ChangeType(x, real_primitive_type))
                  .ToList();
        return CastList(ObjectCollection, real_primitive_type);
    }

    private static object CastList(List<object> items, Type type)
    {
        var enumerableType = typeof(Enumerable);
        var castMethod = enumerableType.GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(type);
        var toListMethod = enumerableType.GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(type);


        var castedItems = castMethod.Invoke(null, new[] { items });

        return toListMethod.Invoke(null, new[] { castedItems });
    }

}
