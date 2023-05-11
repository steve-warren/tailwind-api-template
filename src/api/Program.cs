using Warrensoft.Reminders.Infra.Converters;
using Warrensoft.Reminders.Domain;
using Warrensoft.Reminders.Infra;
using Microsoft.Azure.Cosmos;
using System.Text.Json;
using Warrensoft.Reminders.Infra;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddCosmosContext();
services.AddEventProcessorWorkers();

services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new ReminderPriorityJsonConverter()
));

services.AddSingleton<IEntityIdentityProvider, KsuidEntityIdentityProvider>();
services.AddSingleton<CosmosClient>((_) =>
    new CosmosClient(builder.Configuration["Cosmos:ConnectionString"], new CosmosClientOptions()
    {
        Serializer = new CosmosJsonSerializer(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new ReminderPriorityJsonConverter()
            }
        })
    })
);
services.AddScoped<CosmosContextMiddleware>();

if (builder.Environment.IsDevelopment())
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseUnitOfWork();

app.Run();
