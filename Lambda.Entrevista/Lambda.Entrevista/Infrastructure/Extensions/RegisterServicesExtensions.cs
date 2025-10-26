using Amazon;
using Amazon.DynamoDBv2;
using Amazon.SecretsManager;
using Amazon.SimpleEmail;
using Lambda.Entrevista.Application.Model;
using Lambda.Entrevista.Application.UseCases;
using Lambda.Entrevista.Domain.Gateways;
using Lambda.Entrevista.Infrastructure.DataProviders;
using Lambda.Entrevista.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lambda.Entrevista.Infrastructure.Extensions
{
    public static class RegisterServicesExtensions
    {
        public static void RegisterServices(this ServiceCollection services)
        {

            services.AddScoped<IAmazonDynamoDB>(sp => new AmazonDynamoDBClient(RegionEndpoint.SAEast1));
            services.AddScoped<IAmazonSimpleEmailService>(sp => new AmazonSimpleEmailServiceClient(RegionEndpoint.SAEast1));
            services.AddScoped<IAmazonSecretsManager>(sp => new AmazonSecretsManagerClient(RegionEndpoint.SAEast1));
            services.AddScoped<IUseCaseAsync<EntrevistaRequest>, EnviarEntrevistaUseCaseAsync>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEntrevistaRepository, EntrevistaRepository>();
        }
    }
}
