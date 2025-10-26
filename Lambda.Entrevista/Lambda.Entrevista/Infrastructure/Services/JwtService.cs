using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Lambda.Entrevista.Domain.Gateways;
using Lambda.Entrevista.Domain.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lambda.Entrevista.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IAmazonSecretsManager _gerenciadorSegredos;
        private readonly string _nomeVariavelAmbienteDoSegredo = "NOME_SECRET_JWT";
        private static string _valorCacheadoDoSegredo;
        private static DateTime _expiraCacheEmUtc;

        private static readonly TimeSpan TempoDeVidaCache = TimeSpan.FromMinutes(5);

        public JwtService(IAmazonSecretsManager gerenciadorSegredos)
        {
            _gerenciadorSegredos = gerenciadorSegredos ?? throw new ArgumentNullException(nameof(gerenciadorSegredos));
        }

        public async Task<string> GerarTokenBase64Async(EntrevistaEntity entrevista)
        {
            if (entrevista == null)
                throw new ArgumentNullException(nameof(entrevista));

            var chaveSecreta = await ObterValorSegredoAsync();

            if (string.IsNullOrEmpty(chaveSecreta))
                throw new InvalidOperationException("Chave JWT não encontrada no Secrets Manager.");

            var claims = new List<Claim>
            {
                new Claim("id", entrevista.Id ?? string.Empty),
                new Claim("nome", entrevista.Nome ?? string.Empty),
                new Claim("email", entrevista.Email ?? string.Empty),
                new Claim("cargo", entrevista.Cargo ?? string.Empty),
                new Claim("criadoEm", entrevista.CriadoEm.ToString("o"))
            };

            var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));
            var credenciaisAssinatura = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256);

            var manipuladorToken = new JwtSecurityTokenHandler();
            var tokenJwt = new JwtSecurityToken(
                issuer: "lambda.entrevista",
                audience: "entrevista-cliente",
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(5),
                signingCredentials: credenciaisAssinatura
            );

            var tokenEmString = manipuladorToken.WriteToken(tokenJwt);

            var bytesDoToken = Encoding.UTF8.GetBytes(tokenEmString);
            var tokenEmBase64 = Convert.ToBase64String(bytesDoToken);

            return tokenEmBase64;
        }

        private async Task<string> ObterValorSegredoAsync()
        {
            if (!string.IsNullOrEmpty(_valorCacheadoDoSegredo) && DateTime.UtcNow < _expiraCacheEmUtc)
                return _valorCacheadoDoSegredo;

            var nomeDoSegredo = Environment.GetEnvironmentVariable(_nomeVariavelAmbienteDoSegredo);

            if (string.IsNullOrEmpty(nomeDoSegredo))
                throw new InvalidOperationException($"Nome do segredo não configurado. Defina a variável de ambiente '{_nomeVariavelAmbienteDoSegredo}'.");

            var requisicao = new GetSecretValueRequest { SecretId = nomeDoSegredo };
            var resposta = await _gerenciadorSegredos.GetSecretValueAsync(requisicao);

            _valorCacheadoDoSegredo = resposta.SecretString;
            _expiraCacheEmUtc = DateTime.UtcNow.Add(TempoDeVidaCache);

            return _valorCacheadoDoSegredo;
        }
    }
}
