using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Domain;

/// <summary>
/// RSQL-powered filtering helpers for <see cref="Article"/> queries.
/// Supports AND (<c>;</c>) and OR (<c>,</c>) operators plus comparison operators
/// <c>eq</c>, <c>!=</c>, <c>ge</c>, <c>le</c>, <c>in</c>, and <c>out</c>.
/// </summary>
/// <remarks>
/// This extension translates a filter string into LINQ expressions EF can execute server-side.
/// Base article fields live in <see cref="ArticleFieldRules"/>, text detail slugs in
/// <see cref="ArticleDetailRules"/>, and numeric detail slugs in <see cref="ArticleNumericDetailRules"/>;
/// add entries there to expose new filters without touching callers.
/// Examples:
/// <list type="bullet">
/// <item><description><c>title==bulb;price=le=5000</c></description></item>
/// <item><description><c>connectivity=in=(wi-fi,ethernet)</c></description></item>
/// <item><description><c>resolution=out=(720p,1080p),power=ge=5</c></description></item>
/// </list>
/// </remarks>
public static class ArticleRsqlSupport
{
    private static readonly MethodInfo StringContainsMethod =
        typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]) ?? throw new InvalidOperationException();

    private static readonly MethodInfo ToLowerMethod =
        typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes) ?? throw new InvalidOperationException();

    private static readonly MethodInfo AnyDetailTextMethod = typeof(Enumerable)
        .GetMethods()
        .Single(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
        .MakeGenericMethod(typeof(ArticleDetailText));

    private static readonly MethodInfo AnyDetailNumericMethod = typeof(Enumerable)
        .GetMethods()
        .Single(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
        .MakeGenericMethod(typeof(ArticleDetailNumeric));

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

    // Base article properties that can be filtered via RSQL (keys are requester-facing names).
    private static readonly IReadOnlyDictionary<string, ArticleFieldRule> ArticleFieldRules =
        new Dictionary<string, ArticleFieldRule>(StringComparer.OrdinalIgnoreCase)
        {
            ["title"] = ArticleFieldRule.Text(p => p.Title),
            ["code"] = ArticleFieldRule.Text(p => p.ArticleCode),
            ["description"] = ArticleFieldRule.Text(p => p.Description),
            ["price"] = ArticleFieldRule.Number(p =>
                p.BasePrice *
                (1 - p.Discounts
                    .Where(d => d.ValidFrom <= DateTime.UtcNow &&
                                (d.ValidTo == null || d.ValidTo >= DateTime.UtcNow))
                    .OrderByDescending(d => d.Percentage)
                    .Select(d => d.Percentage)
                    .FirstOrDefault())),
            ["available"] = ArticleFieldRule.Bool(p => p.IsAvailable)
        };

    // Article detail slugs that can be filtered via RSQL (string comparisons only).
    private static readonly IReadOnlyDictionary<string, ArticleDetailRule> ArticleDetailRules =
        new Dictionary<string, ArticleDetailRule>(StringComparer.OrdinalIgnoreCase)
        {
            ["audio"] = ArticleDetailRule.Text("audio", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["audio-quality"] = ArticleDetailRule.Text("audio-quality", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["battery"] = ArticleDetailRule.Text("battery", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["brand"] = ArticleDetailRule.Text("brand", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["camera"] = ArticleDetailRule.Text("camera", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["color"] = ArticleDetailRule.Text("color", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["color-temperature"] = ArticleDetailRule.Text("color-temperature", RsqlOperator.Equal,
                RsqlOperator.In, RsqlOperator.NotIn),
            ["compatibility"] = ArticleDetailRule.Text("compatibility", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["connectivity"] = ArticleDetailRule.Text("connectivity", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["display"] = ArticleDetailRule.Text("display", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["features"] = ArticleDetailRule.Text("features", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["material"] = ArticleDetailRule.Text("material", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["model"] = ArticleDetailRule.Text("model", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["operating-system"] = ArticleDetailRule.Text("operating-system", RsqlOperator.Equal,
                RsqlOperator.In, RsqlOperator.NotIn),
            ["ports"] = ArticleDetailRule.Text("ports", RsqlOperator.Equal, RsqlOperator.In, RsqlOperator.NotIn),
            ["processor"] = ArticleDetailRule.Text("processor", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["resolution"] = ArticleDetailRule.Text("resolution", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn),
            ["video-quality"] = ArticleDetailRule.Text("video-quality", RsqlOperator.Equal, RsqlOperator.In,
                RsqlOperator.NotIn)
        };

    // Article numeric detail slugs that can be filtered via RSQL (numeric comparisons).
    private static readonly IReadOnlyDictionary<string, ArticleDetailNumericRule> ArticleNumericDetailRules =
        new Dictionary<string, ArticleDetailNumericRule>(StringComparer.OrdinalIgnoreCase)
        {
            ["battery-size"] = ArticleDetailNumericRule.Number(
                "battery-size",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["depth"] = ArticleDetailNumericRule.Number(
                "depth",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["height"] = ArticleDetailNumericRule.Number(
                "height",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["power"] = ArticleDetailNumericRule.Number(
                "power",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["ram-size"] = ArticleDetailNumericRule.Number(
                "ram-size",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["refresh-rate"] = ArticleDetailNumericRule.Number(
                "refresh-rate",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["screen-size"] = ArticleDetailNumericRule.Number(
                "screen-size",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["storage"] = ArticleDetailNumericRule.Number(
                "storage",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["weight"] = ArticleDetailNumericRule.Number(
                "weight",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn),
            ["width"] = ArticleDetailNumericRule.Number(
                "width",
                RsqlOperator.Equal,
                RsqlOperator.NotEqual,
                RsqlOperator.GreaterThanOrEqual,
                RsqlOperator.LessThanOrEqual,
                RsqlOperator.In,
                RsqlOperator.NotIn)
        };

    /// <summary>
    /// Applies an RSQL string to an article query.
    /// </summary>
    /// <param name="query">Queryable of articles to filter.</param>
    /// <param name="rsqlFilter">RSQL filter string (<c>field=op=value</c>), or null to skip filtering.</param>
    /// <returns>The filtered queryable; original instance if no filter is provided or no predicate is built.</returns>
    public static IQueryable<Article> ApplyArticleRsql(this IQueryable<Article> query, string? rsqlFilter)
    {
        if (string.IsNullOrWhiteSpace(rsqlFilter))
            return query;

        var predicate = BuildPredicate(rsqlFilter);
        return query.Where(predicate);
    }

    private static Expression<Func<Article, bool>> BuildPredicate(string rsqlFilter)
    {
        var filterGroups = ParseFilterGroupsOrThrow(rsqlFilter);
        var parameter = Expression.Parameter(typeof(Article), "article");
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

        return Expression.Lambda<Func<Article, bool>>(orExpression, parameter);
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
        if (ArticleFieldRules.TryGetValue(token.Field, out var fieldRule))
        {
            var expression = fieldRule.Build(parameter, token);
            return expression ??
                   throw BadFilter($"Operator '{token.Operator}' is not supported for field '{token.Field}'.");
        }

        if (ArticleDetailRules.TryGetValue(token.Field, out var detailRule))
            return BuildDetailExpression(parameter, token, detailRule);

        if (ArticleNumericDetailRules.TryGetValue(token.Field, out var numericRule))
            return BuildNumericDetailExpression(parameter, token, numericRule);

        throw BadFilter($"Unknown filter field or slug '{token.Field}'.");
    }

    private static Expression BuildDetailExpression(
        ParameterExpression articleParam,
        FilterToken token,
        ArticleDetailRule rule)
    {
        if (!rule.SupportedOperators.Contains(token.Operator))
            throw BadFilter(
                $"Operator '{token.Operator}' is not supported for detail '{rule.Slug}'. Use one of: {string.Join(", ", rule.SupportedOperators)}.");

        return BuildTextDetailExpression(articleParam, token, rule);
    }

    private static Expression BuildTextDetailExpression(
        ParameterExpression articleParam,
        FilterToken token,
        ArticleDetailRule rule)
    {
        var valuePredicate = BuildStringPredicate(
            NormalizeString(Expression.Property(TextDetailParameter, nameof(ArticleDetailText.Value))),
            token);

        var slugPredicate = Expression.Equal(
            Expression.Property(TextDetailParameter, nameof(ArticleDetailText.DetailSlug)),
            Expression.Constant(rule.Slug));

        var detailMatchBody = Expression.AndAlso(slugPredicate, valuePredicate);
        var detailLambda = Expression.Lambda<Func<ArticleDetailText, bool>>(detailMatchBody, TextDetailParameter);

        return BuildAnyCall(
            articleParam,
            nameof(Article.ArticleDetailTexts),
            detailLambda,
            AnyDetailTextMethod);
    }

    private static Expression BuildNumericDetailExpression(
        ParameterExpression articleParam,
        FilterToken token,
        ArticleDetailNumericRule rule)
    {
        if (!rule.SupportedOperators.Contains(token.Operator))
            throw BadFilter(
                $"Operator '{token.Operator}' is not supported for numeric detail '{rule.Slug}'. Use one of: {string.Join(", ", rule.SupportedOperators)}.");

        var numbers = ParseNumericValues(token.Values);

        var parsedValue = Expression.Property(NumericDetailParameter, nameof(ArticleDetailNumeric.Value));

        var comparison = BuildNumericComparison(parsedValue, token.Operator, numbers);
        var slugPredicate = Expression.Equal(
            Expression.Property(NumericDetailParameter, nameof(ArticleDetailNumeric.DetailSlug)),
            Expression.Constant(rule.Slug));

        var numericMatchBody = Expression.AndAlso(slugPredicate, comparison);
        var numericLambda =
            Expression.Lambda<Func<ArticleDetailNumeric, bool>>(numericMatchBody, NumericDetailParameter);

        return BuildAnyCall(
            articleParam,
            nameof(Article.ArticleDetailNumerics),
            numericLambda,
            AnyDetailNumericMethod);
    }

    private static Expression BuildAnyCall(
        ParameterExpression articleParam,
        string propertyName,
        LambdaExpression predicate,
        MethodInfo anyMethod)
    {
        var property = Expression.PropertyOrField(articleParam, propertyName);
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
        Expression<Func<Article, decimal>> selector,
        FilterToken token)
    {
        var values = ParseNumericValues(token.Values);

        var property = ReplaceParameter(selector, parameter);
        return BuildNumericComparison(property, token.Operator, values);
    }

    private static Expression BuildNumericComparison(
        Expression numericExpression,
        RsqlOperator op,
        IReadOnlyList<decimal> values)
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
                [typeof(decimal)],
                Expression.Constant(values.ToArray()),
                numericExpression),
            RsqlOperator.NotIn => Expression.Not(Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Contains),
                [typeof(decimal)],
                Expression.Constant(values.ToArray()),
                numericExpression)),
            _ => throw BadFilter($"Operator '{op}' is not supported for numeric comparisons.")
        };
    }

    private static Expression BuildBooleanPredicate(
        ParameterExpression parameter,
        Expression<Func<Article, bool>> selector,
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

    private static IReadOnlyList<decimal> ParseNumericValues(IReadOnlyList<string> values)
    {
        var numbers = new List<decimal>();
        foreach (var raw in values)
        {
            if (!decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
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

    private static readonly ParameterExpression TextDetailParameter =
        Expression.Parameter(typeof(ArticleDetailText), "detail");

    private static readonly ParameterExpression NumericDetailParameter =
        Expression.Parameter(typeof(ArticleDetailNumeric), "detailNumeric");

    /// <summary>
    /// Parsed token representing a single <c>field op value</c> fragment in the filter string.
    /// </summary>
    private sealed record FilterToken(string Field, RsqlOperator Operator, IReadOnlyList<string> Values);

    /// <summary>
    /// Describes how an article detail slug should be interpreted (string operators only).
    /// </summary>
    private sealed record ArticleDetailRule(
        string Slug,
        ISet<RsqlOperator> SupportedOperators)
    {
        public static ArticleDetailRule Text(string slug, params RsqlOperator[] operators)
        {
            return new ArticleDetailRule(slug, operators.ToHashSet());
        }
    }

    /// <summary>
    /// Describes how a numeric detail slug should be interpreted (numeric operators).
    /// </summary>
    private sealed record ArticleDetailNumericRule(
        string Slug,
        ISet<RsqlOperator> SupportedOperators)
    {
        public static ArticleDetailNumericRule Number(string slug, params RsqlOperator[] operators)
        {
            return new ArticleDetailNumericRule(slug, operators.ToHashSet());
        }
    }

    /// <summary>
    /// Describes how a top-level article field is converted into an expression tree for a filter token.
    /// </summary>
    private sealed record ArticleFieldRule(Func<ParameterExpression, FilterToken, Expression?> Build)
    {
        public static ArticleFieldRule Text(Expression<Func<Article, string?>> selector)
        {
            return new ArticleFieldRule((parameter, token) =>
            {
                var normalizedProperty = NormalizeString(ReplaceParameter(selector, parameter));
                return BuildStringPredicate(normalizedProperty, token);
            });
        }

        public static ArticleFieldRule Number(Expression<Func<Article, decimal>> selector)
        {
            return new ArticleFieldRule((parameter, token) => BuildNumericPredicate(parameter, selector, token));
        }

        public static ArticleFieldRule Bool(Expression<Func<Article, bool>> selector)
        {
            return new ArticleFieldRule((parameter, token) => BuildBooleanPredicate(parameter, selector, token));
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