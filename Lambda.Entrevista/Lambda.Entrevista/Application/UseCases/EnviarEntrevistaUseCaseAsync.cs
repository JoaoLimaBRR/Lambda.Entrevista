using Amazon.Lambda.Core;
using Lambda.Entrevista.Application.Model;
using Lambda.Entrevista.Domain.Constants;
using Lambda.Entrevista.Domain.Gateways;
using Lambda.Entrevista.Domain.Model;

namespace Lambda.Entrevista.Application.UseCases
{
    public class EnviarEntrevistaUseCaseAsync : IUseCaseAsync<EntrevistaRequest>
    {
        private readonly IEntrevistaRepository _repositorio;
        private readonly IEmailService _servicoEmail;
        private readonly IJwtService _jwtService;


        public EnviarEntrevistaUseCaseAsync(IEntrevistaRepository repositorio, IEmailService servicoEmail, IJwtService jwtService)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            _servicoEmail = servicoEmail ?? throw new ArgumentNullException(nameof(servicoEmail));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(servicoEmail));
        }

        public async Task ExecutarAsync(EntrevistaRequest request, ILambdaContext context)
        {

            if (request == null)
                throw new ArgumentException("Payload inválido: corpo não pode ser desserializado");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Payload inválido: e-mail ausente");

            var entidade = new EntrevistaEntity
            {
                Id = Guid.NewGuid().ToString(),
                Nome = request.Nome,
                Email = request.Email,
                Cargo = request.Cargo,
                Status = "PENDENTE",
                CriadoEm = DateTime.UtcNow,
            };

            var tokenBase64 = await _jwtService.GerarTokenBase64Async(entidade);

            entidade.TokenJwtBase64 = tokenBase64;

            await _repositorio.SalvarAsync(entidade);

            var link = $"https://link-falso.entrevista/perguntas/{entidade.Id}";

            var assunto = $"Entrevista: perguntas sobre processo jurídico";
            var corpo = EmailMensagens.CorpoEmail(entidade.Nome, link);

            await _servicoEmail.EnviarEmailAsync(request.Email, assunto, corpo);
        }
    }
}
