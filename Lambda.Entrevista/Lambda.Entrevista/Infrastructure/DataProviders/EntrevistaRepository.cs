using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Lambda.Entrevista.Domain.Gateways;
using Lambda.Entrevista.Domain.Model;

namespace Lambda.Entrevista.Infrastructure.DataProviders
{
    public class EntrevistaRepository : IEntrevistaRepository
    {
        private readonly IAmazonDynamoDB _clienteDynamo;
        private readonly IDynamoDBContext _contexto;


        public EntrevistaRepository(IAmazonDynamoDB clienteDynamo)
        {
            _clienteDynamo = clienteDynamo ?? throw new ArgumentNullException(nameof(clienteDynamo));
            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            _contexto = new DynamoDBContext(_clienteDynamo, config);
        }

        // Construtor adicional para testes (injeção do contexto)
        public EntrevistaRepository(IDynamoDBContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }


        public async Task SalvarAsync(EntrevistaEntity entidade, CancellationToken cancellationToken = default)
        {
            return;
            if (entidade == null) throw new ArgumentNullException(nameof(entidade));

            await _contexto.SaveAsync(entidade, cancellationToken).ConfigureAwait(false);
        }
    }
}
