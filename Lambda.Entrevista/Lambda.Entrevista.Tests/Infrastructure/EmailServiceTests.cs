using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Lambda.Entrevista.Infrastructure.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lambda.Entrevista.Tests.Infrastructure
{
    public class EmailServiceTests
    {
        private Mock<IAmazonSimpleEmailService> _sesMock;
        private EmailService _emailService;
        private string destinatario = "dest@exemplo.com";
        private string assunto = "Assunto teste";
        private string corpo = "Corpo do email";

        private void Arrange(Action action = null)
        {
            _sesMock = new Mock<IAmazonSimpleEmailService>();

            // Mock padrão: responde com SendEmailResponse
            _sesMock.Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), default))
                    .ReturnsAsync(new SendEmailResponse());

            _emailService = new EmailService(_sesMock.Object);

            action?.Invoke();
        }

        private async Task Act(string destinatario, string assunto, string corpo)
        {
            await _emailService.EnviarEmailAsync(destinatario, assunto, corpo);
        }

        // Asst síncrono
        private void Asst(Action action) => action();


        [Fact]
        public async Task EnviarEmailAsync_DeveChamarSesComParametrosCorretos()
        {
            Arrange(() =>
            {
                Environment.SetEnvironmentVariable("Remetente", "a");
                _emailService = new EmailService(_sesMock.Object);
            });

            await Act(destinatario, assunto, corpo);

            Asst(() =>
            {
                _sesMock.Verify(x => x.SendEmailAsync(It.Is<SendEmailRequest>(req =>
                    req.Source == "a" &&
                    req.Destination != null &&
                    req.Destination.ToAddresses != null &&
                    req.Destination.ToAddresses.Count == 1 &&
                    req.Destination.ToAddresses[0] == destinatario &&
                    req.Message != null &&
                    req.Message.Subject != null &&
                    req.Message.Subject.Data == assunto &&
                    req.Message.Body != null &&
                    req.Message.Body.Text != null &&
                    req.Message.Body.Text.Data == corpo
                ), default), Times.Once);
            });
        }

        [Fact]
        public async Task EnviarEmailAsync_NaoLanca_QuandoParametrosSaoValidos()
        {
            Arrange();

            Asst(async () =>
            {
                var ex = await Record.ExceptionAsync(() => Act(destinatario, assunto, corpo));
                Assert.Null(ex);
            });
        }

        [Fact]
        public void Construtor_DeveLancarArgumentNullException_QuandoClienteNulo()
        {
            Arrange();

            Asst(() =>
            {
                Assert.Throws<ArgumentNullException>(() => new EmailService(null));
            });
        }
    }
}
