using KsuidDotNet;
using Microsoft.AspNetCore.Mvc;
using Warrensoft.Reminders.Infra;

namespace Warrensoft.Reminders.App;

public record UpsertAccountCommand(string? Id);

[ApiController]
public sealed class CreateAndReadAccountCommandHandler
{
    [HttpPost("api/accounts")]
    public async Task<IActionResult> Handle(
        [FromBody] UpsertAccountCommand command,
        [FromServices] CosmosEntityContainer<Account> container,
        CancellationToken cancellationToken)
    {
        var account = command.Id is null ? new Account { Id = Ksuid.NewKsuid("a_"), Name = "Foo" }
                                         : (await container.FindAsync(command.Id, command.Id, cancellationToken));

        container.Add(account);

        await container.SaveChangesAsync(cancellationToken);

        account = await container.FindAsync(id: account.Id, partitionKey: "a_", cancellationToken);

        account.Name = "Bar";

        container.Update(account);

        await container.SaveChangesAsync(cancellationToken);

        container.Remove(account);

        await container.SaveChangesAsync(cancellationToken);

        return new OkResult();
    }
}