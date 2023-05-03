using Microsoft.AspNetCore.Http;
using Warrensoft.Reminders.Infra;

namespace WarrenSoft.Reminders.Infra;

public sealed class CosmosContextMiddleware : IMiddleware
{
    private readonly CosmosContext _cosmosContext;

    public CosmosContextMiddleware(CosmosContext cosmosContext)
    {
        _cosmosContext = cosmosContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        await _cosmosContext.SaveChangesAsync(context.RequestAborted);
    }
}
