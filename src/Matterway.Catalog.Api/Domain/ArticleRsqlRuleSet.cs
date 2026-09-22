using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Domain;

internal sealed class ArticleRsqlRuleSet
{
    private ArticleRsqlRuleSet(
        IReadOnlyDictionary<string, ArticleDetailRule> textRules,
        IReadOnlyDictionary<string, ArticleDetailNumericRule> numericRules)
    {
        TextRules = textRules;
        NumericRules = numericRules;
    }

    internal IReadOnlyDictionary<string, ArticleDetailRule> TextRules { get; }
    internal IReadOnlyDictionary<string, ArticleDetailNumericRule> NumericRules { get; }

    internal static ArticleRsqlRuleSet FromDetails(IEnumerable<Detail> details)
    {
        ArgumentNullException.ThrowIfNull(details);

        var textRules = new Dictionary<string, ArticleDetailRule>(StringComparer.OrdinalIgnoreCase);
        var numericRules = new Dictionary<string, ArticleDetailNumericRule>(StringComparer.OrdinalIgnoreCase);
        var knownSlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var detail in details)
        {
            if (string.IsNullOrWhiteSpace(detail.Slug))
                throw new InvalidOperationException("A detail used for RSQL filtering has an empty slug.");

            if (!knownSlugs.Add(detail.Slug))
                throw new InvalidOperationException(
                    $"Detail slug '{detail.Slug}' is duplicated when compared without case.");

            if (string.IsNullOrWhiteSpace(detail.Unit))
                textRules.Add(detail.Slug, ArticleDetailRule.Text(detail.Slug));
            else
                numericRules.Add(detail.Slug, ArticleDetailNumericRule.Number(detail.Slug));
        }

        return new ArticleRsqlRuleSet(textRules, numericRules);
    }
}

internal sealed record ArticleDetailRule(
    string Slug,
    IReadOnlySet<RsqlOperator> SupportedOperators)
{
    private static readonly IReadOnlySet<RsqlOperator> TextOperators = new HashSet<RsqlOperator>
    {
        RsqlOperator.Equal,
        RsqlOperator.NotEqual,
        RsqlOperator.In,
        RsqlOperator.NotIn
    };

    internal static ArticleDetailRule Text(string slug)
    {
        return new ArticleDetailRule(slug, TextOperators);
    }
}

internal sealed record ArticleDetailNumericRule(
    string Slug,
    IReadOnlySet<RsqlOperator> SupportedOperators)
{
    private static readonly IReadOnlySet<RsqlOperator> NumericOperators = new HashSet<RsqlOperator>
    {
        RsqlOperator.Equal,
        RsqlOperator.NotEqual,
        RsqlOperator.GreaterThanOrEqual,
        RsqlOperator.LessThanOrEqual,
        RsqlOperator.In,
        RsqlOperator.NotIn
    };

    internal static ArticleDetailNumericRule Number(string slug)
    {
        return new ArticleDetailNumericRule(slug, NumericOperators);
    }
}

internal enum RsqlOperator
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