using FluentAssertions.AspNetCore.Mvc;
using mock;

using WarrenSoft.Reminders.Http;

namespace test;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var addListCommand = new AddListCommand(OwnerId: "", Name: "", ColorId: "", EmojiId: "");

        var handler = new AddListCommandHandler();

        var result = await handler.Handle(command: addListCommand, reminderLists: new MockReminderListRepository(), ids: new MockEntityIdentityProvider(), cancellationToken: default);

        result.Should().BeOkObjectResult();
    }
}