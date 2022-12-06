// See https://aka.ms/new-console-template for more information



public static class StringFilters
{
    public static string OnlyNumbers(string value) => string.Concat(value.Where(x => char.IsNumber(x)));
    public static string OnlyDecimal(string value) => string.Concat(value.Where(x => char.IsNumber(x) || x == ',' || x == '.'));
}
