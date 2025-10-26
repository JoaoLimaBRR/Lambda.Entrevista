namespace Lambda.Entrevista.Domain.Gateways
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string destinatario, string assunto, string corpo);
    }
}
