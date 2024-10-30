namespace Service.Extensions;

public static class StringExtensions
{
    public static bool MatchesPropertyName<T>(this string input)
        where T : class
    {
        var type = typeof(T);
        var properties = type.GetProperties();

        return properties.Any(property => string.Equals(property.Name, input));
    }

    public static DateTime ToDateTime(this string input, string label)
    {
        var isDateTime = DateTime.TryParse(input, out var result);

        if (!isDateTime)
        {
            throw new InvalidOperationException($"{label} is not of DateTime format!");
        }

        return result;
    }
}