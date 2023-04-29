using WarrenSoft.Reminders.Domain;
using WarrenSoft.Reminders.Infra;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// singleton repository for testing purposes
builder.Services.AddSingleton<IReminderListRepository, InMemoryReminderListRepository>();
builder.Services.AddSingleton<IReminderRepository, InMemoryReminderRepository>();

builder.Services.AddSingleton<IEntityIdentityProvider, KsuidEntityIdentityProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
