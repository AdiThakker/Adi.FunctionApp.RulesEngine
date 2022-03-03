
namespace Adi.FunctionApp.RulesEngine.Domain.Models;

public class RuleResult
{
    public RuleContext Context { get; }

    public string Status { get; set; }

    public RuleResult(RuleContext context, string status)
    {
        Context = context;
        Status = status;
    }
}

