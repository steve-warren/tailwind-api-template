using WarrenSoft.Reminders.Infra.Converters;
using WarrenSoft.Reminders.Domain;
using WarrenSoft.Reminders.Infra;
using Microsoft.Azure.Cosmos;
using System.Text.Json;
using Warrensoft.Reminders.Infra;
using Warrensoft.Reminders.Domain;

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

builder.Services.AddScoped((sp) =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    return new CosmosContext(client, databaseName: builder.Configuration["Cosmos:DatabaseName"]!);
});

builder.Services.AddScoped((sp) =>
{
    var client = sp.GetRequiredService<CosmosClient>();
    var container = client.GetContainer(builder.Configuration["Cosmos:DatabaseName"]!, "accounts");

    return new CosmosEntityContainer<Account>(container, (account) => account.PartitionKey);
});

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CosmosContainer>());
builder.Services.AddScoped<IRepository<ReminderList>, CosmosReminderListRepository>();
builder.Services.AddScoped<IRepository<Reminder>, CosmosReminderRepository>();
builder.Services.AddScoped<IRepository<Plan>, CosmosPlanRepository>();
builder.Services.AddScoped<IRepository<AccountPlan>, CosmosAccountPlanRepository>();
builder.Services.AddScoped<CosmosContextMiddleware>();

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
