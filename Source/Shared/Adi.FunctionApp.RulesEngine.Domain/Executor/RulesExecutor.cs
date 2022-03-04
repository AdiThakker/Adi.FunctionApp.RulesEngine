

using System.Collections.Concurrent;
using Adi.FunctionApp.RulesEngine.Domain.Interfaces;
using Adi.FunctionApp.RulesEngine.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Adi.FunctionApp.RulesEngine.Domain.Executor;

public class RulesExecutor : IRulesExecutor<RuleContext, RuleResult>
{

    private ConcurrentDictionary<Func<RuleContext, bool>, (bool, IEnumerable<IRule<RuleContext, RuleResult>>)> rulesConfiguration;

    private readonly ILogger<RulesExecutor> logger;

    public RulesExecutor(IRulesBuilder<RuleContext, RuleResult> rulesBuilder, ILogger<RulesExecutor> loger)
    {
        if (rulesBuilder is null)
            throw new ArgumentNullException(nameof(rulesBuilder));

        logger = loger ?? throw new ArgumentNullException(nameof(loger));

        if (rulesConfiguration is null)
            rulesConfiguration = (ConcurrentDictionary<Func<RuleContext, bool>, (bool, IEnumerable<IRule<RuleContext, RuleResult>>)>)rulesBuilder.Build();    
        
    }

    public IEnumerable<Task<RuleResult>> ExecuteAsync(RuleContext input)
    {
        IEnumerable<IRule<RuleContext,RuleResult>>? GetRulesToExecute(RuleContext input)
        {
            // Get Rules to execute
            var matches = rulesConfiguration
                            .Where(configuration => configuration.Key(input))
                            .Select(criteria => new { Priority = criteria.Value.Item1, Rules = criteria.Value.Item2 });

            if (matches.Any())
                return matches.FirstOrDefault(match => match.Priority == true)?.Rules ?? matches.FirstOrDefault()?.Rules;

            return default;
        };

        var rules = GetRulesToExecute(input);
        if(rules != null && rules.Any())
            return rules.Select(rule => rule.ExecuteAsync(input));

        throw new InvalidOperationException($"No rule to execute for {input.Source} with {input.ContextType}");
    }
}

