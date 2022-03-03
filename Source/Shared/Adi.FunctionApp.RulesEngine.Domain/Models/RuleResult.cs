
namespace Adi.FunctionApp.RulesEngine.Domain.Models;

public class RuleResult
{
    public RuleContext Context { get; }

    public RuleResult(RuleContext context) => Context = context;

}

