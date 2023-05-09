using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warrensoft.Reminders.Workers;

namespace Warrensoft.Reminders.Infra;

public static class ServiceExtensions
{
    public static IServiceCollection AddEventProcessorWorkers(this IServiceCollection services)
    {
        services.AddHostedService<CosmosChangeFeedWorker>();
        services.AddHostedService<AzureServiceBusWorker>();
        return services;
    }

    public static IServiceCollection AddCosmosContext(this IServiceCollection services)
    {
        services.AddScoped((sp) =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            return new CosmosContext(client, databaseName: sp.GetRequiredService<IConfiguration>()["Cosmos:DatabaseName"]!);
        });

        return services;
    }
}