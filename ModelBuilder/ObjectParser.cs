namespace ModelBuilder
{
    public enum ObjectType
    {
        String,
        Int,
        Double,
        Decimal,
        DateTime,
        Bool,
        Guid,
        TimeSpan,
        DateTimeOffset,
        Uri,
        Char,
        Byte,
        Sbyte,
        Short,
        Ushort,
        Uint,
        Long,
        Ulong,
        Float,
        Enum,
        JsonObject
    }

    public static class ObjectParser
    {

        public static object? ParseToObject(ObjectType type, string? value)
        {
            if (value == null) return null;
            return type switch
            {
                ObjectType.String => value,
                ObjectType.Int => int.Parse(value),
                ObjectType.Double => double.Parse(value),
                ObjectType.Decimal => decimal.Parse(value),
                ObjectType.DateTime => DateTime.Parse(value),
                ObjectType.Bool => bool.Parse(value),
                ObjectType.Guid => Guid.Parse(value),
                ObjectType.TimeSpan => TimeSpan.Parse(value),
                ObjectType.DateTimeOffset => DateTimeOffset.Parse(value),
                ObjectType.Uri => new Uri(value),
                ObjectType.Char => char.Parse(value),
                ObjectType.Byte => byte.Parse(value),
                ObjectType.Sbyte => sbyte.Parse(value),
                ObjectType.Short => short.Parse(value),
                ObjectType.Ushort => ushort.Parse(value),
                ObjectType.Uint => uint.Parse(value),
                ObjectType.Long => long.Parse(value),
                ObjectType.Ulong => ulong.Parse(value),
                ObjectType.Float => float.Parse(value),
                _ => throw new Exception($"Type {type} not supported"),
            };
        }

        public static ObjectType ParseTypeToObjectType(Type type)
        {
            if (type == typeof(string))
            {
                return ObjectType.String;
            }
            else if (type == typeof(int))
            {
                return ObjectType.Int;
            }
            else if (type == typeof(int))
            {
                return ObjectType.Int;
            }
            else if (type == typeof(double))
            {
                return ObjectType.Double;
            }
            else if (type == typeof(decimal))
            {
                return ObjectType.Decimal;
            }
            else if (type == typeof(DateTime))
            {
                return ObjectType.DateTime;
            }
            else if (type == typeof(bool))
            {
                return ObjectType.Bool;
            }
            else if (type == typeof(Guid))
            {
                return ObjectType.Guid;
            }
            else if (type == typeof(TimeSpan))
            {
                return ObjectType.TimeSpan;
            }
            else if (type == typeof(DateTimeOffset))
            {
                return ObjectType.DateTimeOffset;
            }
            else if (type == typeof(Uri))
            {
                return ObjectType.Uri;
            }
            else if (type == typeof(char))
            {
                return ObjectType.Char;
            }
            else if (type == typeof(byte))
            {
                return ObjectType.Byte;
            }
            else if (type == typeof(sbyte))
            {
                return ObjectType.Sbyte;
            }
            else if (type == typeof(short))
            {
                return ObjectType.Short;
            }
            else if (type == typeof(ushort))
            {
                return ObjectType.Ushort;
            }
            else if (type == typeof(uint))
            {
                return ObjectType.Uint;
            }
            else if (type == typeof(long))
            {
                return ObjectType.Long;
            }
            else if (type == typeof(ulong))
            {
                return ObjectType.Ulong;
            }
            else if (type == typeof(float))
            {
                return ObjectType.Float;
            }
            else if (type.IsEnum)
            {
                return ObjectType.Enum;
            }
            else
            {
                throw new Exception($"Type {type} not supported");
            }
        }

        internal static Type MemberTypeToType(ObjectType memberType)
        {
            return memberType switch
            {
                ObjectType.String => typeof(string),
                ObjectType.Int => typeof(int),
                ObjectType.Double => typeof(double),
                ObjectType.Decimal => typeof(decimal),
                ObjectType.DateTime => typeof(DateTime),
                ObjectType.Bool => typeof(bool),
                ObjectType.Guid => typeof(Guid),
                ObjectType.TimeSpan => typeof(TimeSpan),
                ObjectType.DateTimeOffset => typeof(DateTimeOffset),
                ObjectType.Uri => typeof(Uri),
                ObjectType.Char => typeof(char),
                ObjectType.Byte => typeof(byte),
                ObjectType.Sbyte => typeof(sbyte),
                ObjectType.Short => typeof(short),
                ObjectType.Ushort => typeof(ushort),
                ObjectType.Uint => typeof(uint),
                ObjectType.Long => typeof(long),
                ObjectType.Ulong => typeof(ulong),
                ObjectType.Float => typeof(float),
                _ => throw new Exception($"Type {memberType} not supported"),
            };
        }
    }
}
