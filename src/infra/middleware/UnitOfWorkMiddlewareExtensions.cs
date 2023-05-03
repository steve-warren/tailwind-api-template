using Microsoft.AspNetCore.Builder;

namespace WarrenSoft.Reminders.Infra;

public static class UnitOfWorkMiddlewareExtensions
{
    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app) =>
        app.UseMiddleware<UnitOfWorkMiddleware>();
}
