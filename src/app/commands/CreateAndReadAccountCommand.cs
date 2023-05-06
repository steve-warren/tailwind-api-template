using KsuidDotNet;
using Microsoft.AspNetCore.Mvc;
using Warrensoft.Reminders.Infra;

namespace app.commands
{
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
            Account? account  = new Account { Id = Ksuid.NewKsuid("a_"), Name = "Foo", PartitionKey = "foo" };

            account.Foo();

            container.Add(account);

            await container.SaveChangesAsync(cancellationToken);

            return new OkResult();
        }
    }
}