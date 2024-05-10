namespace WebApplication2.Infrastructure.Extensions;

public static class DoubleExtensions
{
    public static double ToDouble(this string value)
    {
        return Convert.ToDouble(value.Replace(".", ","));
    }
}


public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }
}