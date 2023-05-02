using WarrenSoft.Reminders.Infra.Converters;
using WarrenSoft.Reminders.Domain;
using WarrenSoft.Reminders.Infra;
using Microsoft.Azure.Cosmos;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new ReminderPriorityJsonConverter()
));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEntityIdentityProvider, KsuidEntityIdentityProvider>();

builder.Services.AddSingleton((_) =>
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

builder.Services.AddScoped<IUnitOfWork>((sp) =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    var database = client.GetDatabase(builder.Configuration["Cosmos:DatabaseName"]);
    var container = database.GetContainer(builder.Configuration["Cosmos:ContainerName"]);

    return new CosmosUnitOfWork(container);
});

builder.Services.AddScoped((sp) =>
{
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();

    return new CosmosContainerContext((CosmosUnitOfWork)unitOfWork);
});

builder.Services.AddScoped<IReminderListRepository, CosmosReminderListRepository>();
builder.Services.AddScoped<IReminderRepository, InMemoryReminderRepository>();
builder.Services.AddScoped<IPlanRepository, InMemoryPlanRepository>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
