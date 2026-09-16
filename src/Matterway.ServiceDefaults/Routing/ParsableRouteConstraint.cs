using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Matterway.ServiceDefaults.Routing;

public sealed class ParsableRouteConstraint<TValue> : IRouteConstraint
    where TValue : IParsable<TValue>
{
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        ArgumentNullException.ThrowIfNull(routeKey);
        ArgumentNullException.ThrowIfNull(values);

        if (!values.TryGetValue(routeKey, out var rawValue) || rawValue is null)
            return false;

        var text = Convert.ToString(rawValue, CultureInfo.InvariantCulture);
        return TValue.TryParse(text, CultureInfo.InvariantCulture, out _);
    }
}