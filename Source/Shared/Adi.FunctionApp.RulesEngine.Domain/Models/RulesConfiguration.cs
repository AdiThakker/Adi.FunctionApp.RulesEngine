namespace Adi.FunctionApp.RulesEngine.Domain.Models;

public class RulesConfiguration
{
    public List<RuleConfiguration> Configurations { get; set; }
}

public class RuleConfiguration
{
    public string Criteria { get; set; }

    public List<string> Rules { get; set; }
}