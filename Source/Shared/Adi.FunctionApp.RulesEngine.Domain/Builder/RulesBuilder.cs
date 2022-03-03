using System.Collections.Concurrent;
using System.Linq;
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
            rules.TryAdd(this.GenerateRule)
            return rules;
        });
    }
}
