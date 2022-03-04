using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Adi.FunctionApp.RulesEngine.Domain.Models;
using Adi.FunctionApp.RulesEngine.Domain.Interfaces;
using Adi.FunctionApp.RulesEngine.Domain.Executor;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adi.FunctionApp.RulesEngine.Service
{
    public class RulesEngineService
    {
        private ILogger<RulesEngineService> logger;
        private IRulesExecutor<RuleContext, RuleResult> rulesExecutor;
        private IOptions<RulesConfiguration> rulesConfiguration;

        public RulesEngineService(ILogger<RulesEngineService> log, IRulesExecutor<RuleContext, RuleResult> ruleExecutor, IOptions<RulesConfiguration> ruleConfiguration)
        {
            this.logger = log ?? throw new ArgumentNullException(nameof(log));
            this.rulesExecutor = ruleExecutor ?? throw new ArgumentNullException(nameof(ruleExecutor));
            this.rulesConfiguration = ruleConfiguration ?? throw new ArgumentNullException(nameof(ruleConfiguration));
        }

        [FunctionName("Dispatcher")]
        public async Task<IActionResult> Dispatcher(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "/{service}")] HttpRequest req, string service, ILogger log)
        {
            log.LogInformation($"Received request for {service}");

            var response = service switch
            {
                "AccountService" => rulesExecutor.Execute(new RuleContext() { Source = "AlarmService" }).Aggregate(new StringBuilder(), (results, result) => results.AppendLine(result.Result.Status)).ToString(),
                "OrderService" => rulesExecutor.Execute(new RuleContext() { Source = "OrderService", Parameters = new Dictionary<string, string> { { "Error", "QuantityError" } } }).Aggregate(new StringBuilder(), (results, result) => results.AppendLine(result.Result.Status)).ToString(),
                _ => "Invalid request"
            };

            return new OkObjectResult(response);
        }
    }
}
