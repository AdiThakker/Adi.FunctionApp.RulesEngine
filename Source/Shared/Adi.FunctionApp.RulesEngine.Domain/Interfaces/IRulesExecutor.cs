namespace Adi.FunctionApp.RulesEngine.Domain.Interfaces;

public interface IRulesExecutor<TInput, TResult>
{
    public IEnumerable<Task<TResult>> Execute(TInput input);
}

