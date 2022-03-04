using System;
using System.Collections.Generic;
using System.IO;
using Adi.FunctionApp.RulesEngine.Domain.Builder;
using Adi.FunctionApp.RulesEngine.Domain.Executor;
using Adi.FunctionApp.RulesEngine.Domain.Interfaces;
using Adi.FunctionApp.RulesEngine.Domain.Models;
using Adi.FunctionApp.RulesEngine.Domain.Rules;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Adi.FunctionApp.RulesEngine.Service.Startup))]
namespace Adi.FunctionApp.RulesEngine.Service
{

    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            FunctionsHostBuilderContext context = builder.GetContext();
            builder.ConfigurationBuilder
                    .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: true)
                    .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            // Register dependencies
            builder.Services.AddLogging();

            // Register Rules
            builder.Services.AddTransient(typeof(ForwardRule));
            builder.Services.AddTransient(typeof(EscalateRule));

            // Register Builder
            builder.Services.AddOptions<RulesConfiguration>().Configure<IConfiguration>((settings, configuration) => configuration.GetSection(nameof(RulesConfiguration)).Bind(settings));
            builder.Services.AddSingleton<IRulesBuilder<RuleContext, RuleResult>>(sp =>
            {
                var configuration = sp.GetRequiredService<IOptions<RulesConfiguration>>();
                Dictionary<string, IRule<RuleContext, RuleResult>> ruleLookup = new Dictionary<string, IRule<RuleContext, RuleResult>>
                {
                    { "Forward", sp.GetRequiredService<ForwardRule>() },
                    { "Escalate", sp.GetRequiredService<EscalateRule>() }
                };
                return new RulesBuilder(configuration, ruleLookup);
            });

            // Register Executor
            builder.Services.AddSingleton<IRulesExecutor<RuleContext, RuleResult>>(sp =>
            {
                return new RulesExecutor(sp.GetRequiredService<IRulesBuilder<RuleContext, RuleResult>>(), sp.GetRequiredService<ILogger<RulesExecutor>>());     
            });

        }
    }
}
