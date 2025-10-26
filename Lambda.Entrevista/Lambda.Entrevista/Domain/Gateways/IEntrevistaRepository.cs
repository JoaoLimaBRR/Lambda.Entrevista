using Lambda.Entrevista.Domain.Model;

namespace Lambda.Entrevista.Domain.Gateways
{
    public interface IEntrevistaRepository
    {
        Task SalvarAsync(EntrevistaEntity entidade, CancellationToken cancellationToken = default);
    }
}
