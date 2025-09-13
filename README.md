# Adi Function App Rules Engine

A flexible and extensible rules engine built as an Azure Function App that allows you to define, configure, and execute business rules dynamically based on context and criteria.

## 🚀 Overview

This project provides a robust rules engine implementation that can:
- Execute different rules based on configurable criteria
- Handle dynamic rule resolution using expression trees
- Support multiple rule types (Escalate, Forward, etc.)
- Provide HTTP endpoints for different services
- Scale automatically with Azure Functions

## 📋 Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local) (for local development)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) with Azure Functions extension

## 🏗️ Project Structure

```
Adi.FunctionApp.RulesEngine/
├── Documentation/                          # Documentation project
│   └── Adi.FunctionApp.RulesEngine.Documentation/
├── Source/                                 # Main source code
│   ├── Adi.FunctionApp.RulesEngine.Service/    # Azure Function App
│   │   ├── RulesEngineService.cs               # Main HTTP trigger function
│   │   ├── Startup.cs                          # Dependency injection configuration
│   │   ├── appsettings.json                    # Rules configuration
│   │   └── host.json                           # Function host settings
│   └── Shared/
│       └── Adi.FunctionApp.RulesEngine.Domain/ # Core business logic
│           ├── Builder/                         # Rule expression builders
│           ├── Executor/                        # Rule execution engine
│           ├── Interfaces/                      # Abstractions and contracts
│           ├── Models/                          # Data models
│           └── Rules/                           # Rule implementations
└── Testing/                                # Test projects
    ├── Adi.FunctionApp.RulesEngine.UnitTest/
    └── Adi.FunctionApp.RulesEngine.IntegrationTest/
```

## ⚡ Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/AdiThakker/Adi.FunctionApp.RulesEngine.git
cd Adi.FunctionApp.RulesEngine
```

### 2. Build the Solution
```bash
dotnet build
```

### 3. Run Locally
```bash
cd Source/Adi.FunctionApp.RulesEngine.Service
func start
```

The function will be available at `http://localhost:7071`

## 🔧 Configuration

Rules are configured in `appsettings.json` using a flexible criteria-based system:

```json
{
  "RulesConfiguration": {
    "Configurations": [
      {
        "Criteria": "Source-ContextType == Order-Product1",
        "Rules": [ "Escalate", "Forward" ]
      },
      {
        "Criteria": "Source == Account",
        "Rules": [ "Forward" ]
      },      
      {
        "Criteria": "Source-Error == Order-QuantityError",
        "Rules": ["Escalate"]
      }
    ]
  }
}
```

### Criteria Syntax
- Use `==` for equality comparisons
- Combine properties with `-` (e.g., `Source-ContextType`)
- Access dynamic parameters using property names (e.g., `Source-Error`)

## 📡 API Endpoints

### Dispatcher Endpoint
**Endpoint:** `GET/POST /{service}`

Executes rules based on the service type and returns the results.

#### Supported Services:
- **AccountService**: `GET http://localhost:7071/AccountService`
- **OrderService**: `GET http://localhost:7071/OrderService`

#### Example Requests:

```bash
# Account Service
curl http://localhost:7071/AccountService

# Order Service  
curl http://localhost:7071/OrderService
```

#### Example Response:
```
Account Forwarded
```

## 🎯 How It Works

### 1. Rule Context
The `RuleContext` class carries the information needed for rule evaluation:

```csharp
public class RuleContext
{
    public string Source { get; set; }              // Service source (e.g., "Account", "Order")
    public string ContextType { get; set; }         // Context type for more specific matching
    public Dictionary<string, string> Parameters { get; init; }  // Dynamic parameters
}
```

### 2. Rule Execution Flow
1. HTTP request comes in with service name
2. `RulesExecutor` builds a `RuleContext` based on the service
3. Configuration criteria are evaluated against the context
4. Matching rules are retrieved and executed
5. Results are aggregated and returned

### 3. Available Rules

#### EscalateRule
```csharp
public class EscalateRule : IRule<RuleContext, RuleResult>
{
    public Task<RuleResult> ExecuteAsync(RuleContext input)
    {
        return Task.FromResult(new RuleResult(input, $"{input.Source} Escalated"));
    }
}
```

#### ForwardRule
```csharp
public class ForwardRule : IRule<RuleContext, RuleResult>
{
    public Task<RuleResult> ExecuteAsync(RuleContext input)
    {
        return Task.FromResult(new RuleResult(input, $"{input.Source} Forwarded"));
    }
}
```

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test Testing/Adi.FunctionApp.RulesEngine.UnitTest
```

### Run Integration Tests
```bash
dotnet test Testing/Adi.FunctionApp.RulesEngine.IntegrationTest
```

### Run All Tests
```bash
dotnet test
```

## 🚀 Deployment

### Deploy to Azure

1. **Create Azure Function App:**
```bash
az functionapp create --resource-group myResourceGroup \
  --consumption-plan-location westus2 \
  --runtime dotnet \
  --functions-version 4 \
  --name myFunctionApp \
  --storage-account mystorageaccount
```

2. **Deploy the function:**
```bash
func azure functionapp publish myFunctionApp
```

### Deploy using Visual Studio
1. Right-click the `Adi.FunctionApp.RulesEngine.Service` project
2. Select "Publish..."
3. Choose "Azure" and follow the wizard

## 🔨 Extending the Rules Engine

### Adding New Rules

1. **Create a new rule class:**
```csharp
public class MyCustomRule : IRule<RuleContext, RuleResult>
{
    public Task<RuleResult> ExecuteAsync(RuleContext input)
    {
        // Your custom logic here
        return Task.FromResult(new RuleResult(input, "Custom logic executed"));
    }
}
```

2. **Register the rule in Startup.cs:**
```csharp
services.AddTransient<IRule<RuleContext, RuleResult>, MyCustomRule>();
```

3. **Add configuration in appsettings.json:**
```json
{
  "Criteria": "Source == MyService",
  "Rules": ["MyCustomRule"]
}
```

### Adding New Services

Update the `Dispatcher` method in `RulesEngineService.cs`:

```csharp
var response = service switch
{
    "AccountService" => /* existing logic */,
    "OrderService" => /* existing logic */,
    "MyNewService" => rulesExecutor.Execute(new RuleContext() { Source = "MyNew" }),
    _ => "Invalid request"
};
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines
- Follow existing code style and patterns
- Add unit tests for new functionality
- Update documentation for new features
- Ensure all tests pass before submitting PR

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [Azure Functions Documentation](https://docs.microsoft.com/en-us/azure/azure-functions/)
- [.NET 6.0 Documentation](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6)
- [Expression Trees in .NET](https://docs.microsoft.com/en-us/dotnet/csharp/expression-trees)

## 📞 Support

For questions, issues, or contributions, please:
- Open an issue on GitHub
- Contact the maintainer: [AdiThakker](https://github.com/AdiThakker)

---

⭐ **Star this repository if you find it helpful!**