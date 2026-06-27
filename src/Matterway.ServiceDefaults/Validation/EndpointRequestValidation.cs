using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Matterway.ServiceDefaults.Validation;

internal static class EndpointRequestValidation
{
    public static int[] GetRequestArgumentIndexes(MethodInfo handlerMethod)
    {
        return handlerMethod
            .GetParameters()
            .Select(static (parameter, index) => (parameter.ParameterType, Index: index))
            .Where(static parameter => IsRequestContract(parameter.ParameterType))
            .Select(static parameter => parameter.Index)
            .ToArray();
    }

    public static IReadOnlyDictionary<string, string[]> Validate(
        IList<object?> arguments,
        IReadOnlyList<int> requestArgumentIndexes)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        foreach (var index in requestArgumentIndexes)
        {
            var argument = arguments[index];
            if (argument is not null)
                ValidateObject(argument, null, errors, visited);
        }

        return errors.ToDictionary(
            static pair => pair.Key,
            static pair => pair.Value.Distinct().ToArray(),
            StringComparer.Ordinal);
    }

    private static void ValidateObject(
        object instance,
        string? prefix,
        Dictionary<string, List<string>> errors,
        HashSet<object> visited)
    {
        if (!visited.Add(instance))
            return;

        foreach (var property in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length > 0)
                continue;

            var key = Combine(prefix, GetJsonName(property));
            var value = property.GetValue(instance);

            ValidateProperty(instance, property, value, key, errors);
            ValidateChildren(value, key, errors, visited);
        }
    }

    private static void ValidateProperty(
        object instance,
        PropertyInfo property,
        object? value,
        string key,
        Dictionary<string, List<string>> errors)
    {
        if (!property.GetCustomAttributes<ValidationAttribute>(true).Any())
            return;

        var results = new List<ValidationResult>();
        var context = new ValidationContext(instance)
        {
            MemberName = property.Name,
            DisplayName = key
        };

        Validator.TryValidateProperty(value, context, results);
        foreach (var result in results)
            AddError(errors, key, result.ErrorMessage ?? "The request value is invalid.");
    }

    private static void ValidateChildren(
        object? value,
        string key,
        Dictionary<string, List<string>> errors,
        HashSet<object> visited)
    {
        if (value is null || value is string)
            return;

        if (value is IEnumerable enumerable and not string)
        {
            var itemIndex = 0;
            foreach (var item in enumerable)
            {
                if (item is not null && IsRequestContract(item.GetType()))
                    ValidateObject(item, $"{key}[{itemIndex}]", errors, visited);

                itemIndex++;
            }

            return;
        }

        if (IsRequestContract(value.GetType()))
            ValidateObject(value, key, errors, visited);
    }

    private static bool IsRequestContract(Type type)
    {
        return type.Namespace?.StartsWith("Matterway.", StringComparison.Ordinal) == true &&
               (type.Name.Contains("Request", StringComparison.Ordinal) ||
                type.Name.Contains("Parameters", StringComparison.Ordinal));
    }

    private static string GetJsonName(PropertyInfo property)
    {
        return property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? ToJsonName(property.Name);
    }

    private static string ToJsonName(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? name
            : char.ToLowerInvariant(name[0]) + name[1..];
    }

    private static string Combine(string? prefix, string key)
    {
        return string.IsNullOrWhiteSpace(prefix) ? key : $"{prefix}.{key}";
    }

    private static void AddError(Dictionary<string, List<string>> errors, string key, string message)
    {
        if (!errors.TryGetValue(key, out var messages))
        {
            messages = [];
            errors[key] = messages;
        }

        messages.Add(message);
    }
}