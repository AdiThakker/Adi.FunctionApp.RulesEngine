using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Adi.FunctionApp.RulesEngine.Domain.Interfaces;
using Adi.FunctionApp.RulesEngine.Domain.Models;
using Microsoft.Extensions.Options;

namespace Adi.FunctionApp.RulesEngine.Domain.Builder;

public class RulesBuilder : IRulesBuilder<RuleContext, RuleResult>
{
    public RulesConfiguration Configuration { get; }
    public IDictionary<string, IRule<RuleContext, RuleResult>> Rules { get; }

    public RulesBuilder(IOptions<RulesConfiguration> rulesConfiguration, IDictionary<string, IRule<RuleContext, RuleResult>> rules)
    {
        if (rulesConfiguration is null)
            throw new ArgumentNullException(nameof(rulesConfiguration));
        
        if (rules is null)
            throw new ArgumentNullException(nameof(rules));
        
        this.Configuration = rulesConfiguration.Value;
        this.Rules = rules;

    }

    public IDictionary<Func<RuleContext, bool>, (bool, IEnumerable<IRule<RuleContext, RuleResult>>)> Build()
    {
        return this.Configuration.Configurations.Aggregate(new ConcurrentDictionary<Func<RuleContext, bool>, (bool, IEnumerable<IRule<RuleContext, RuleResult>>)>(), (rules, config) =>
        {
            rules.TryAdd(this.GenerateRuleCriteria(config.Criteria),(false, config.Rules.Select(rule => this.Rules[rule])));
            return rules;
        });
    }

    private Func<RuleContext, bool> GenerateRuleCriteria(string criteria)
    {
        var paramExpression = Expression.Parameter(typeof(RuleContext), "context");
        var criteriaBody = criteria.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        (string[] Properties, string[] Values) parseExpressions = (criteriaBody[0].Split("-", StringSplitOptions.RemoveEmptyEntries), criteriaBody[2].Split("-", StringSplitOptions.RemoveEmptyEntries));
        Expression bodyExpression = default;
        for (int i = 0; i < parseExpressions.Properties.Length; i++)
        {
            Expression propertyExpression;
            (Expression Left, Expression Right) expression = BuildPropertyAccessExpression(typeof(RuleContext), paramExpression, parseExpressions.Properties[i], parseExpressions.Values[i]);
            if (criteriaBody[1].Equals("==", StringComparison.InvariantCulture))
                propertyExpression = Expression.Equal(expression.Left, expression.Right);
            else
                propertyExpression = Expression.NotEqual(expression.Left, expression.Right);

            bodyExpression = i == 0 ? propertyExpression : Expression.AndAlso(bodyExpression, propertyExpression);
        }
        
        return Expression.Lambda<Func<RuleContext, bool>>(bodyExpression, paramExpression).Compile();

        (Expression, Expression) BuildPropertyAccessExpression(Type type, ParameterExpression paramExpression, string propertyName, string propertyValue)
        {
            return (type.GetProperty(propertyName) != null)
                ? (Expression.Property(paramExpression, propertyName), Expression.Constant(propertyValue, typeof(string)))
                : (Expression.Property(Expression.Property(paramExpression, "Parameters"), "Item", Expression.Constant(propertyName, typeof(string))), Expression.Constant(propertyValue, typeof(string)));
        }
    }
}
