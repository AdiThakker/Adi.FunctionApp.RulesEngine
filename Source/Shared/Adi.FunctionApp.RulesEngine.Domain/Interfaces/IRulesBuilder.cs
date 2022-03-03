namespace Adi.FunctionApp.RulesEngine.Domain.Interfaces;

public interface IRulesBuilder<TInput, TResult>
{
    IDictionary<Func<TInput, bool>, (bool, IEnumerable<IRule<TInput, TResult>>)> Build();
}
