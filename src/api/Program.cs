using WarrenSoft.Reminders.App.TypeConverters;
using WarrenSoft.Reminders.Domain;
using WarrenSoft.Reminders.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new ReminderPriorityJsonConverter()
));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// singleton repository for testing purposes
builder.Services.AddSingleton<IReminderListRepository, InMemoryReminderListRepository>();
builder.Services.AddSingleton<IReminderRepository, InMemoryReminderRepository>();

builder.Services.AddSingleton<IEntityIdentityProvider, KsuidEntityIdentityProvider>();

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
