using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Domain;

/// <summary>
/// RSQL-powered filtering helpers for <see cref="Product"/> queries.
/// Supports AND (<c>;</c>) and OR (<c>,</c>) operators plus comparison operators
/// <c>eq</c>, <c>!=</c>, <c>ge</c>, <c>le</c>, <c>in</c>, and <c>out</c>.
/// </summary>
/// <remarks>
/// This extension translates a filter string into LINQ expressions EF can execute server-side.
/// Base product fields live in <see cref="ProductFieldRules"/>, detail slugs in
/// <see cref="ProductDetailRules"/>, and specification slugs in <see cref="ProductSpecificationRules"/>;
/// add entries there to expose new filters without touching callers.
/// Examples:
/// <list type="bullet">
/// <item><description><c>title==bulb;price=le=5000</c></description></item>
/// <item><description><c>connectivity=in=(wi-fi,ethernet)</c></description></item>
/// <item><description><c>resolution=out=(720p,1080p),power=ge=5</c></description></item>
/// </list>
/// </remarks>
public static class ProductRsqlExtensions
{
    private static readonly MethodInfo StringContainsMethod =
        typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]) ?? throw new InvalidOperationException();

    private static readonly MethodInfo ToLowerMethod =
        typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes) ?? throw new InvalidOperationException();

    private static readonly MethodInfo AnyDetailMethod = typeof(Enumerable)
        .GetMethods()
        .Single(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
        .MakeGenericMethod(typeof(ProductDetail));

    private static readonly MethodInfo AnySpecificationMethod = typeof(Enumerable)
        .GetMethods()
        .Single(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
        .MakeGenericMethod(typeof(ProductSpecification));

    /// <summary>
    /// Token map for parsing operator fragments inside the filter string.
    /// </summary>
    private static readonly (string Token, RsqlOperator Operator)[] OperatorTokens =
    [
        ("=ge=", RsqlOperator.GreaterThanOrEqual),
        ("=le=", RsqlOperator.LessThanOrEqual),
        ("=in=", RsqlOperator.In),
        ("=out=", RsqlOperator.NotIn),
        ("=eq=", RsqlOperator.Equal),
        ("==", RsqlOperator.Equal),
        ("!=", RsqlOperator.NotEqual)
    ];

    // Base product properties that can be filtered via RSQL (keys are requester-facing names).
    private static readonly IReadOnlyDictionary<string, ProductFieldRule> ProductFieldRules =
        new Dictionary<string, ProductFieldRule>(StringComparer.OrdinalIgnoreCase)
        {
            ["title"] = ProductFieldRule.Text(p => p.Title),
            ["code"] = ProductFieldRule.Text(p => p.ProductCode),
            ["description"] = ProductFieldRule.Text(p => p.Description),
            ["price"] = ProductFieldRule.Number(p => p.Prices.FirstOrDefault(p => p.Currency == ESupportedCurrency.RSD)
                .Amount),
            ["available"] = ProductFieldRule.Bool(p => p.IsAvailable)
        };

    // Product detail slugs that can be filtered via RSQL (string comparisons only).
    private static readonly IReadOnlyDictionary<string, ProductDetailRule> ProductDetailRules =
        new Dictionary<string, ProductDetailRule>(StringComparer.OrdinalIgnoreCase)
        {
            ["audio"] = ProductDetailRule.Text("audio", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["audio-quality"] = ProductDetailRule.Text("audio-quality", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["battery"] = ProductDetailRule.Text("battery", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["brand"] = ProductDetailRule.Text("brand", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["camera"] = ProductDetailRule.Text("camera", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["color"] = ProductDetailRule.Text("color", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["color-temperature"] = ProductDetailRule.Text("color-temperature", RsqlOperator.Equal,
                RsqlOperator.In, RsqlOperator.NotIn),
            ["compatibility"] = ProductDetailRule.Text("compatibility", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["connectivity"] = ProductDetailRule.Text("connectivity", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["display"] = ProductDetailRule.Text("display", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["features"] = ProductDetailRule.Text("features", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["material"] = ProductDetailRule.Text("material", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["model"] = ProductDetailRule.Text("model", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["operating-system"] = ProductDetailRule.Text("operating-system", RsqlOperator.Equal,
                RsqlOperator.In, RsqlOperator.NotIn),
            ["ports"] = ProductDetailRule.Text("ports", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["processor"] = ProductDetailRule.Text("processor", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["resolution"] = ProductDetailRule.Text("resolution", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["video-quality"] = ProductDetailRule.Text("video-quality", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn)
        };

    // Product specification slugs that can be filtered via RSQL (numeric comparisons).
    private static readonly IReadOnlyDictionary<string, ProductSpecificationRule> ProductSpecificationRules =
        new Dictionary<string, ProductSpecificationRule>(StringComparer.OrdinalIgnoreCase)
        {
            ["battery-size"] = ProductSpecificationRule.Number(
                "battery-size",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["depth"] = ProductSpecificationRule.Number(
                "depth",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["height"] = ProductSpecificationRule.Number(
                "height",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["power"] = ProductSpecificationRule.Number(
                "power",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["ram-size"] = ProductSpecificationRule.Number(
                "ram-size",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["refresh-rate"] = ProductSpecificationRule.Number(
                "refresh-rate",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["screen-size"] = ProductSpecificationRule.Number(
                "screen-size",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["storage"] = ProductSpecificationRule.Number(
                "storage",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["weight"] = ProductSpecificationRule.Number(
                "weight",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["width"] = ProductSpecificationRule.Number(
                "width",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn)
        };

    /// <summary>
    /// Applies an RSQL string to a product query.
    /// </summary>
    /// <param name="query">Queryable of products to filter.</param>
    /// <param name="rsqlFilter">RSQL filter string (<c>field=op=value</c>), or null to skip filtering.</param>
    /// <returns>The filtered queryable; original instance if no filter is provided or no predicate is built.</returns>
    public static IQueryable<Product> ApplyProductRsql(this IQueryable<Product> query, string? rsqlFilter)
    {
        if (string.IsNullOrWhiteSpace(rsqlFilter))
            return query;

        var predicate = BuildPredicate(rsqlFilter);
        return query.Where(predicate);
    }

    private static Expression<Func<Product, bool>> BuildPredicate(string rsqlFilter)
    {
        var filterGroups = ParseFilterGroupsOrThrow(rsqlFilter);
        var parameter = Expression.Parameter(typeof(Product), "product");
        Expression? orExpression = null;

        foreach (var andGroup in filterGroups)
        {
            Expression? andExpression = null;
            foreach (var token in andGroup)
            {
                var expression = BuildExpressionForToken(parameter, token);
                andExpression = andExpression is null
                    ? expression
                    : Expression.AndAlso(andExpression, expression);
            }

            if (andExpression != null)
                orExpression = orExpression is null ? andExpression : Expression.OrElse(orExpression, andExpression);
        }

        if (orExpression is null)
            throw BadFilter("Filter is invalid or empty.");

        return Expression.Lambda<Func<Product, bool>>(orExpression, parameter);
    }

    private static IReadOnlyList<IReadOnlyList<FilterToken>> ParseFilterGroupsOrThrow(string rsqlFilter)
    {
        var result = new List<IReadOnlyList<FilterToken>>();
        foreach (var orSegment in SplitSegments(rsqlFilter, ','))
        {
            var tokens = SplitSegments(orSegment, ';')
                .Select(ParseTokenOrThrow)
                .ToList();

            if (tokens.Count == 0)
                throw BadFilter("Filter is invalid or empty.");

            result.Add(tokens);
        }

        if (result.Count == 0)
            throw BadFilter("Filter is invalid or empty.");

        return result;
    }

    private static IEnumerable<string> SplitSegments(string value, char separator)
    {
        return value.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static FilterToken ParseTokenOrThrow(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            throw BadFilter("Filter is invalid or empty.");

        var trimmed = rawToken.Trim();
        foreach (var (opToken, op) in OperatorTokens)
        {
            var opIndex = trimmed.IndexOf(opToken, StringComparison.OrdinalIgnoreCase);
            if (opIndex <= 0)
                continue;

            var field = trimmed[..opIndex].Trim();
            var rawValue = trimmed[(opIndex + opToken.Length)..].Trim();
            if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(rawValue))
                break;

            var values = op is RsqlOperator.In or RsqlOperator.NotIn
                ? SplitList(rawValue)
                : [UnwrapValue(rawValue)];

            return new FilterToken(field, op, values);
        }

        throw BadFilter($"Invalid filter fragment '{rawToken}'.");
    }

    private static IReadOnlyList<string> SplitList(string rawValue)
    {
        var trimmed = rawValue.Trim();
        if (trimmed.StartsWith('(') && trimmed.EndsWith(')'))
            trimmed = trimmed[1..^1];

        return trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(UnwrapValue)
            .ToArray();
    }

    private static string UnwrapValue(string value)
    {
        var trimmed = value.Trim();
        if ((trimmed.StartsWith('"') && trimmed.EndsWith('"')) ||
            (trimmed.StartsWith('\'') && trimmed.EndsWith('\'')))
            return trimmed[1..^1];

        return trimmed;
    }

    private static Expression BuildExpressionForToken(ParameterExpression parameter, FilterToken token)
    {
        if (ProductFieldRules.TryGetValue(token.Field, out var fieldRule))
        {
            var expression = fieldRule.Build(parameter, token);
            return expression ??
                   throw BadFilter($"Operator '{token.Operator}' is not supported for field '{token.Field}'.");
        }

        if (ProductDetailRules.TryGetValue(token.Field, out var detailRule))
            return BuildDetailExpression(parameter, token, detailRule);

        if (ProductSpecificationRules.TryGetValue(token.Field, out var specificationRule))
            return BuildSpecificationExpression(parameter, token, specificationRule);

        throw BadFilter($"Unknown filter field or slug '{token.Field}'.");
    }

    private static Expression BuildDetailExpression(
        ParameterExpression productParam,
        FilterToken token,
        ProductDetailRule rule)
    {
        if (!rule.SupportedOperators.Contains(token.Operator))
            throw BadFilter(
                $"Operator '{token.Operator}' is not supported for detail '{rule.Slug}'. Use one of: {string.Join(", ", rule.SupportedOperators)}.");

        return BuildTextDetailExpression(productParam, token, rule);
    }

    private static Expression BuildTextDetailExpression(
        ParameterExpression productParam,
        FilterToken token,
        ProductDetailRule rule)
    {
        var valuePredicate = BuildStringPredicate(
            NormalizeString(Expression.Property(DetailParameter, nameof(ProductDetail.Value))),
            token);

        var slugPredicate = Expression.Equal(
            Expression.Property(DetailParameter, nameof(ProductDetail.DetailSlug)),
            Expression.Constant(rule.Slug));

        var detailMatchBody = Expression.AndAlso(slugPredicate, valuePredicate);
        var detailLambda = Expression.Lambda<Func<ProductDetail, bool>>(detailMatchBody, DetailParameter);

        return BuildAnyCall(
            productParam,
            nameof(Product.ProductDetails),
            detailLambda,
            AnyDetailMethod);
    }

    private static Expression BuildSpecificationExpression(
        ParameterExpression productParam,
        FilterToken token,
        ProductSpecificationRule rule)
    {
        if (!rule.SupportedOperators.Contains(token.Operator))
            throw BadFilter(
                $"Operator '{token.Operator}' is not supported for specification '{rule.Slug}'. Use one of: {string.Join(", ", rule.SupportedOperators)}.");

        var numbers = ParseNumericValues(token.Values);

        var parsedValue = Expression.Convert(
            Expression.Property(SpecificationParameter, nameof(ProductSpecification.Value)),
            typeof(double));

        var comparison = BuildNumericComparison(parsedValue, token.Operator, numbers);
        var slugPredicate = Expression.Equal(
            Expression.Property(SpecificationParameter, nameof(ProductSpecification.SpecificationSlug)),
            Expression.Constant(rule.Slug));

        var specificationMatchBody = Expression.AndAlso(slugPredicate, comparison);
        var specificationLambda =
            Expression.Lambda<Func<ProductSpecification, bool>>(specificationMatchBody, SpecificationParameter);

        return BuildAnyCall(
            productParam,
            nameof(Product.ProductSpecifications),
            specificationLambda,
            AnySpecificationMethod);
    }

    private static Expression BuildAnyCall(
        ParameterExpression productParam,
        string propertyName,
        LambdaExpression predicate,
        MethodInfo anyMethod)
    {
        var property = Expression.Property(productParam, propertyName);
        var notNull =
            Expression.NotEqual(property, Expression.Constant(null, property.Type));
        var anyCall = Expression.Call(anyMethod, property, predicate);
        return Expression.AndAlso(notNull, anyCall);
    }

    private static Expression BuildStringPredicate(Expression normalizedProperty, FilterToken token)
    {
        var normalizedValues = token.Values
            .Select(v => UnwrapValue(v).ToLower())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();

        if (normalizedValues.Length == 0)
            throw BadFilter("Filter value is required for string comparison.");

        Expression Contains(string value)
        {
            return Expression.Call(normalizedProperty, StringContainsMethod, Expression.Constant(value));
        }

        return token.Operator switch
        {
            RsqlOperator.Equal => Contains(normalizedValues.First()),
            RsqlOperator.NotEqual => Expression.Not(Contains(normalizedValues.First())),
            RsqlOperator.In => CombineWithOr(normalizedValues.Select(Contains)),
            RsqlOperator.NotIn => Expression.Not(CombineWithOr(normalizedValues.Select(Contains))),
            _ => throw BadFilter($"Operator '{token.Operator}' is not supported for string comparisons.")
        };
    }

    private static Expression BuildNumericPredicate(
        ParameterExpression parameter,
        Expression<Func<Product, double>> selector,
        FilterToken token)
    {
        var values = ParseNumericValues(token.Values);

        var property = ReplaceParameter(selector, parameter);
        return BuildNumericComparison(property, token.Operator, values);
    }

    private static Expression BuildNumericComparison(
        Expression numericExpression,
        RsqlOperator op,
        IReadOnlyList<double> values)
    {
        var value = Expression.Constant(values.First());
        return op switch
        {
            RsqlOperator.Equal => Expression.Equal(numericExpression, value),
            RsqlOperator.NotEqual => Expression.NotEqual(numericExpression, value),
            RsqlOperator.GreaterThan => Expression.GreaterThan(numericExpression, value),
            RsqlOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(numericExpression, value),
            RsqlOperator.LessThan => Expression.LessThan(numericExpression, value),
            RsqlOperator.LessThanOrEqual => Expression.LessThanOrEqual(numericExpression, value),
            RsqlOperator.In => Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Contains),
                [typeof(double)],
                Expression.Constant(values.ToArray()),
                numericExpression),
            RsqlOperator.NotIn => Expression.Not(Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Contains),
                [typeof(double)],
                Expression.Constant(values.ToArray()),
                numericExpression)),
            _ => throw BadFilter($"Operator '{op}' is not supported for numeric comparisons.")
        };
    }

    private static Expression BuildBooleanPredicate(
        ParameterExpression parameter,
        Expression<Func<Product, bool>> selector,
        FilterToken token)
    {
        if (!bool.TryParse(token.Values.FirstOrDefault(), out var boolValue))
            throw BadFilter("Boolean comparison requires 'true' or 'false'.");

        var property = ReplaceParameter(selector, parameter);
        var constant = Expression.Constant(boolValue);

        return token.Operator switch
        {
            RsqlOperator.Equal => Expression.Equal(property, constant),
            RsqlOperator.NotEqual => Expression.NotEqual(property, constant),
            _ => throw BadFilter($"Operator '{token.Operator}' is not supported for boolean comparisons.")
        };
    }

    private static Expression NormalizeString(Expression value)
    {
        var coalesced = Expression.Coalesce(value, Expression.Constant(string.Empty));
        return Expression.Call(coalesced, ToLowerMethod);
    }

    private static IReadOnlyList<double> ParseNumericValues(IReadOnlyList<string> values)
    {
        var numbers = new List<double>();
        foreach (var raw in values)
        {
            if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                throw BadFilter($"Value '{raw}' is not a valid number.");

            numbers.Add(number);
        }

        if (numbers.Count == 0)
            throw BadFilter("At least one numeric value is required.");

        return numbers;
    }

    private static Expression ReplaceParameter(LambdaExpression expression, ParameterExpression parameter)
    {
        var visitor = new ReplaceParameterVisitor(expression.Parameters[0], parameter);
        return visitor.Visit(expression.Body);
    }

    private static Expression CombineWithOr(IEnumerable<Expression> expressions)
    {
        Expression? combined = null;
        foreach (var expression in expressions)
            combined = combined is null ? expression : Expression.OrElse(combined, expression);

        return combined ?? Expression.Constant(false);
    }

    private static BadHttpRequestException BadFilter(string detail)
    {
        return new BadHttpRequestException(detail, StatusCodes.Status400BadRequest);
    }

    private static readonly ParameterExpression DetailParameter =
        Expression.Parameter(typeof(ProductDetail), "detail");

    private static readonly ParameterExpression SpecificationParameter =
        Expression.Parameter(typeof(ProductSpecification), "specification");

    /// <summary>
    /// Parsed token representing a single <c>field op value</c> fragment in the filter string.
    /// </summary>
    private sealed record FilterToken(string Field, RsqlOperator Operator, IReadOnlyList<string> Values);

    /// <summary>
    /// Describes how a product detail slug should be interpreted (string operators only).
    /// </summary>
    private sealed record ProductDetailRule(
        string Slug,
        ISet<RsqlOperator> SupportedOperators)
    {
        public static ProductDetailRule Text(string slug, params RsqlOperator[] operators)
        {
            return new ProductDetailRule(slug, operators.ToHashSet());
        }
    }

    /// <summary>
    /// Describes how a product specification slug should be interpreted (numeric operators).
    /// </summary>
    private sealed record ProductSpecificationRule(
        string Slug,
        ISet<RsqlOperator> SupportedOperators)
    {
        public static ProductSpecificationRule Number(string slug, params RsqlOperator[] operators)
        {
            return new ProductSpecificationRule(slug, operators.ToHashSet());
        }
    }

    /// <summary>
    /// Describes how a top-level product field is converted into an expression tree for a filter token.
    /// </summary>
    private sealed record ProductFieldRule(Func<ParameterExpression, FilterToken, Expression?> Build)
    {
        public static ProductFieldRule Text(Expression<Func<Product, string?>> selector)
        {
            return new ProductFieldRule((parameter, token) =>
            {
                var normalizedProperty = NormalizeString(ReplaceParameter(selector, parameter));
                return BuildStringPredicate(normalizedProperty, token);
            });
        }

        public static ProductFieldRule Number(Expression<Func<Product, double>> selector)
        {
            return new ProductFieldRule((parameter, token) => BuildNumericPredicate(parameter, selector, token));
        }

        public static ProductFieldRule Bool(Expression<Func<Product, bool>> selector)
        {
            return new ProductFieldRule((parameter, token) => BuildBooleanPredicate(parameter, selector, token));
        }
    }

    /// <summary>
    /// Replaces a known parameter in an expression tree so selectors can be reused with new root parameters.
    /// </summary>
    private sealed class ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == source ? target : base.VisitParameter(node);
        }
    }

    private enum RsqlOperator
    {
        Equal,
        NotEqual,
        In,
        NotIn,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }
}