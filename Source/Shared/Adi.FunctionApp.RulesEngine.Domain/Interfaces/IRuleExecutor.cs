namespace Adi.FunctionApp.RulesEngine.Domain.Interfaces;

public interface IRuleExecutor<TInput, TResult>
{
    public IEnumerable<Task<TResult>> ExecuteAsync(TInput input);
}

