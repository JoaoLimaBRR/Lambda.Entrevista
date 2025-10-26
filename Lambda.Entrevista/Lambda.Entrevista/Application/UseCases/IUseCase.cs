using Amazon.Lambda.Core;

namespace Lambda.Entrevista.Application.UseCases
{
    public interface IUseCaseAsync<TRequest>
    {
        Task ExecutarAsync(TRequest request, ILambdaContext context);
    }
}
