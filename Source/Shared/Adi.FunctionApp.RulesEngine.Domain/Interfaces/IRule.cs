namespace Adi.FunctionApp.RulesEngine.Domain.Interfaces;

public interface IRule<TInput, TResult>
{
    public Task<TResult> ExecuteAsync(TInput input);
}