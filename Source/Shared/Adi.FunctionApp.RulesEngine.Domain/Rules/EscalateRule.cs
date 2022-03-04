﻿
using Adi.FunctionApp.RulesEngine.Domain.Interfaces;
using Adi.FunctionApp.RulesEngine.Domain.Models;

namespace Adi.FunctionApp.RulesEngine.Domain.Rules;
public class EscalateRule : IRule<RuleContext, RuleResult>
{
    public Task<RuleResult> ExecuteAsync(RuleContext input)
    {
        return Task.FromResult(new RuleResult(input, $"{input.Source} Escalated"));
    }
}
