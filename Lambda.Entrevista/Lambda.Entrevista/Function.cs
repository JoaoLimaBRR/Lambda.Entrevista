using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Lambda.Entrevista.Application.Model;
using Lambda.Entrevista.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda.Entrevista;

public class Function
{

    private readonly IUseCaseAsync<EntrevistaRequest> _enviarEntrevistaUseCase;

    // Construtor que cria o container de serviços e injeta as dependências
    public Function()
    {
        var services = new ServiceCollection();
        Infrastructure.Extensions.RegisterServicesExtensions.RegisterServices(services);

        var serviceProvider = services.BuildServiceProvider();

        _enviarEntrevistaUseCase = serviceProvider.GetRequiredService<IUseCaseAsync<EntrevistaRequest>>();
    }

   
    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            try
            {
                var entrevista = JsonSerializer.Deserialize<EntrevistaRequest>(record.Body);

                context.Logger.LogInformation($"Nome: {entrevista.Nome}, Email: {entrevista.Email}, Cargo: {entrevista.Cargo}");
                await _enviarEntrevistaUseCase.ExecutarAsync(entrevista, context);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Erro ao processar registro SQS: {ex.Message}");
            }
        }
    }

}
