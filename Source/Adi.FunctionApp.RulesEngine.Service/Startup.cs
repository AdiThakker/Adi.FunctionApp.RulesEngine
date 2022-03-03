using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly:FunctionsStartup(typeof(Adi.FunctionApp.RulesEngine.Service.Startup))]
namespace Adi.FunctionApp.RulesEngine.Service
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // TODO Register Dependencies
            throw new System.NotImplementedException();
        }
    }
}
