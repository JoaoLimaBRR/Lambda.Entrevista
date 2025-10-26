using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Lambda.Entrevista.Domain.Gateways;

namespace Lambda.Entrevista.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailService _clienteSes;
        private readonly string _remetente = Environment.GetEnvironmentVariable("Remetente");


        public EmailService(IAmazonSimpleEmailService clienteSes)
        {
            _clienteSes = clienteSes ?? throw new ArgumentNullException(nameof(clienteSes));
        }


        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpo)
        {
            var requisicaoEnvio = new SendEmailRequest
            {
                Source = _remetente,
                Destination = new Destination { ToAddresses = new System.Collections.Generic.List<string> { destinatario } },
                Message = new Message
                {
                    Subject = new Content(assunto),
                    Body = new Body { Text = new Content(corpo) }
                }
            };

            await _clienteSes.SendEmailAsync(requisicaoEnvio);
        }
    }
}
