using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Lambda.Entrevista.Domain.Gateways;
using Lambda.Entrevista.Domain.Model;
using Lambda.Entrevista.Infrastructure.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambda.Entrevista.Tests.Infrastructure
{
    public class JwtServiceTests
    {
        private Mock<IAmazonSecretsManager> _secretMock;
        private JwtService _jwtService;
        private EntrevistaEntity _entrevistaEntity;


        private void Arrange(Action action = null)
        {
            _secretMock = new Mock<IAmazonSecretsManager>();
            _jwtService = new JwtService(_secretMock.Object);

            var segredo = "MinhaChaveSecreta1234567890123456";
            Environment.SetEnvironmentVariable("a", "SegredoFake");

            _secretMock.Setup(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), default))
           .ReturnsAsync(new GetSecretValueResponse { SecretString = segredo });

            action?.Invoke();

        }

        private async Task<string> Act(EntrevistaEntity entrevistaEntity)
        {
            return  await _jwtService.GerarTokenBase64Async(entrevistaEntity);
        }

        private void Asst(Action action = null)
        {
            action.Invoke();
        }

        [Fact]
        public async Task GerarTokenBase64Async_DeveGerarToken_QuandoEntrevistaValida()
        {
            Arrange(() =>
            {
                Environment.SetEnvironmentVariable("NOME_SECRET_JWT", "nome123");
            });

            var entrevista = new EntrevistaEntity
            {
                Id = "123",
                Nome = "João",
                Email = "joao@email.com",
                Cargo = "Desenvolvedor",
                CriadoEm = DateTime.UtcNow
            };
            // Act
            var tokenBase64 = await Act(entrevista);

            // Assert
            Asst(() =>
            {
                Assert.False(string.IsNullOrEmpty(tokenBase64));
                var tokenBytes = Convert.FromBase64String(tokenBase64);
                var tokenString = Encoding.UTF8.GetString(tokenBytes);
                var manipuladorToken = new JwtSecurityTokenHandler();
                var token = manipuladorToken.ReadJwtToken(tokenString);
                Assert.Contains("lambda.entrevista", token.Issuer);
                Assert.Equal(DateTime.Now.AddDays(5).Date, token.ValidTo.Date);
                Assert.Equal(DateTime.Now.Date, token.ValidFrom.Date);
            });
        }

        [Fact]
        public async Task GerarTokenBase64Async_DeveLancarArgumentNullException_QuandoEntrevistaNula()
        {
            Arrange();

            await Assert.ThrowsAsync<ArgumentNullException>(() => Act(null));
        }

        [Fact]
        public async Task ObterValorSegredoAsync_DeveLancarInvalidOperationException_QuandoSegredoNaoConfigurado()
        {
            Arrange(() =>
            {
                Environment.SetEnvironmentVariable("a", null);
                Environment.SetEnvironmentVariable("NOME_SECRETO_JWT", null);
            });
           
            var entrevista = new EntrevistaEntity
            {
                Id = "123",
                Nome = "João",
                Email = "joao@email.com",
                Cargo = "Desenvolvedor",
                CriadoEm = DateTime.UtcNow
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Act(entrevista));

            Asst(() =>
            {
                Assert.Contains("Nome do segredo não configurado", exception.Message);
            });
        }

    }
}
