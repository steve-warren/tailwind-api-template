using Microsoft.AspNetCore.Http;
using WarrenSoft.Reminders.Domain;

namespace WarrenSoft.Reminders.Infra;

public sealed class UnitOfWorkMiddleware : IMiddleware
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkMiddleware(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        await _unitOfWork.SaveChangesAsync(context.RequestAborted);
    }
}
