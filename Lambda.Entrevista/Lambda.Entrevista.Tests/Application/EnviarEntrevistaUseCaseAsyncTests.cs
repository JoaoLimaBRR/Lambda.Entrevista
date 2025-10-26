using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Lambda.Entrevista.Application.Model;
using Lambda.Entrevista.Application.UseCases;
using Lambda.Entrevista.Domain.Gateways;
using Lambda.Entrevista.Domain.Model;
using Moq;
using Xunit;

namespace Lambda.Entrevista.Tests.Application.UseCases
{
    public class EnviarEntrevistaUseCaseAsyncTests
    {
        private Mock<IEntrevistaRepository> _repositorioMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IJwtService> _jwtServiceMock;
        private Mock<ILambdaContext> _lambdaContextMock;
        private CancellationToken _cancellationToken = CancellationToken.None;

        private EnviarEntrevistaUseCaseAsync _useCase;

        private EntrevistaRequest _request;
        private string _tokenFake = "token-base64-fake";

        private void Arrange(Action action = null)
        {
            _repositorioMock = new Mock<IEntrevistaRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _jwtServiceMock = new Mock<IJwtService>();
            _lambdaContextMock = new Mock<ILambdaContext>();

            _jwtServiceMock
                .Setup(j => j.GerarTokenBase64Async(It.IsAny<EntrevistaEntity>()))
                .ReturnsAsync(_tokenFake);

            _request = new EntrevistaRequest
            {
                Nome = "João",
                Email = "joao@exemplo.com",
                Cargo = "Advogado"
            };

            action?.Invoke();

            _useCase = new EnviarEntrevistaUseCaseAsync(
                _repositorioMock.Object,
                _emailServiceMock.Object,
                _jwtServiceMock.Object
            );
        }

        private async Task Act(EntrevistaRequest request) =>
            await _useCase.ExecutarAsync(request, _lambdaContextMock.Object);

        private void Asst(Action action) => action();

        [Fact]
        public async Task ExecutarAsync_ComPayloadNulo_LancaArgumentException()
        {
            Arrange();

            Asst(async () =>
            {
                await Assert.ThrowsAsync<ArgumentException>(() => Act(null));
            });
        }

        [Fact]
        public async Task ExecutarAsync_ComEmailNulo_LancaArgumentException()
        {
            Arrange(() =>
            {
                _request.Email = "   ";
            });

            Asst(async () =>
            {
                await Assert.ThrowsAsync<ArgumentException>(() => Act(_request));
            });
        }

        [Fact]
        public async Task ExecutarAsync_ExecutaCorretamente_ChamaRepositorioEEmail()
        {
            Arrange();

            await Act(_request);

            Asst(() =>
            {
                _repositorioMock.Verify(r => r.SalvarAsync(It.Is<EntrevistaEntity>(e =>
                    e.Nome == _request.Nome &&
                    e.Email == _request.Email &&
                    e.Cargo == _request.Cargo &&
                    e.Status == "PENDENTE" &&
                    e.TokenJwtBase64 == _tokenFake
                ), _cancellationToken), Times.Once);

                _emailServiceMock.Verify(e => e.EnviarEmailAsync(
                    _request.Email,
                    It.Is<string>(s => s.Contains("Entrevista")),
                    It.Is<string>(c => c.Contains(_request.Nome))
                ), Times.Once);
            });
        }
    }
}
