

namespace Adi.FunctionApp.RulesEngine.Domain.Models;

internal class ExecutionContext
{
    // First class properties
    public string Source { get; set; }

    public string ContextType { get; set; }  

    // Dynamic properties
    public Dictionary<string, string> Parameters { get; init; }

    public ExecutionContext() => Parameters = new Dictionary<string, string>(); 
}

