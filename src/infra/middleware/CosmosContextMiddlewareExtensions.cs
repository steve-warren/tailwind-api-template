using Microsoft.AspNetCore.Builder;

namespace WarrenSoft.Reminders.Infra;

public static class CosmosContextMiddlewareExtensions
{
    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app) =>
        app.UseMiddleware<CosmosContextMiddleware>();
}
