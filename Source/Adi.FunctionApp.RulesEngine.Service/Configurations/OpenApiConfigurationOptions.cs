using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Adi.FunctionApp.RulesEngine.Service.Configurations;

public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
{
    public override OpenApiInfo Info { get => base.Info; set => base.Info = value; }

    public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;

}