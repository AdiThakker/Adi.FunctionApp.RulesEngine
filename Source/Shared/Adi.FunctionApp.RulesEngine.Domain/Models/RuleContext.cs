

namespace Adi.FunctionApp.RulesEngine.Domain.Models;

public class RuleContext
{
    // First class properties
    public string Source { get; set; }

    public string ContextType { get; set; }  

    // Dynamic properties
    public Dictionary<string, string> Parameters { get; init; }

    public RuleContext() => Parameters = new Dictionary<string, string>(); 
}

