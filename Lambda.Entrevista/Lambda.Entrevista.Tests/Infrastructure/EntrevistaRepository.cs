using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Lambda.Entrevista.Domain.Model;
using Lambda.Entrevista.Infrastructure.DataProviders;
using Moq;

namespace Lambda.Entrevista.Tests.Infrastructure
{
    public class EntrevistaRepositoryTests
    {
        private Mock<IDynamoDBContext> _dynamoMock;
        private EntrevistaRepository _repository;

        private void Arrange(Action action = null)
        {
            _dynamoMock = new Mock<IDynamoDBContext>();

            _dynamoMock
                .Setup(x => x.SaveAsync(It.IsAny<EntrevistaEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _repository = new EntrevistaRepository(_dynamoMock.Object);

            action?.Invoke();
        }

        private async Task Act(EntrevistaEntity entidade, CancellationToken cancellationToken = default)
        {
            await _repository.SalvarAsync(entidade, cancellationToken);
        }

        private void Asst(Action action) => action();

        [Fact]
        public void Construtor_DeveLancarArgumentNullException_QuandoClienteDynamoNulo()
        {
            Arrange();

            // Act & Assert dentro do Asst (síncrono)
            Asst(() =>
            {
                Assert.Throws<ArgumentNullException>(() => new EntrevistaRepository(null));
            });
        }

        [Fact]
        public async Task SalvarAsync_DeveLancarArgumentNullException_QuandoEntidadeNula()
        {
            Arrange();

            Asst(async () =>
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => Act(null));
            });
        }

        [Fact]
        public async Task SalvarAsync_DeveChamarPutItemNoDynamo_QuandoEntidadeValida()
        {
            Arrange();

            var entidade = new EntrevistaEntity
            {
                Id = "id-1",
                Nome = "João",
                Email = "joao@exemplo.com",
                Cargo = "Desenvolvedor",
                CriadoEm = DateTime.UtcNow
            };

            await Act(entidade);

            Asst(() =>
            {
                _dynamoMock.Verify(x => x.SaveAsync(It.IsAny<EntrevistaEntity>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            });
        }

        [Fact]
        public async Task SalvarAsync_NaoDeveLancar_QuandoClienteDynamoRetornaComSucesso()
        {
            Arrange();

            var entidade = new EntrevistaEntity
            {
                Id = "id-2",
                Nome = "Maria",
                Email = "maria@exemplo.com",
                Cargo = "Analista",
                CriadoEm = DateTime.UtcNow
            };

            Asst(async () =>
            {
                var ex = await Record.ExceptionAsync(() => Act(entidade));
                Assert.Null(ex);
            });
        }

        [Fact]
        public async Task SalvarAsync_PropagaExcecaoDoCliente_DynamoFalha()
        {
            Arrange(() =>
            {
                _dynamoMock = new Mock<IDynamoDBContext>();
                _dynamoMock
                    .Setup(x => x.SaveAsync(It.IsAny<EntrevistaEntity>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception("Erro Dynamo"));

                _repository = new EntrevistaRepository(_dynamoMock.Object);

            });
            
            var entidade = new EntrevistaEntity
            {
                Id = "id-3",
                Nome = "Erro",
                Email = "erro@exemplo.com",
                Cargo = "Ops",
                CriadoEm = DateTime.UtcNow
            };

            Asst(async () =>
            {
                var ex = await Assert.ThrowsAsync<Exception>(() => _repository.SalvarAsync(entidade));
                Assert.Contains("Erro Dynamo", ex.Message);
            });
        }
    }
}
