using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Domain.Entities;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Tags.Commands.AddTagsCommand;

namespace Todo_App.Application.IntegrationTests.Tags.Commands;

using static Testing;

public class AddTagsTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new AddTagsCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldAddTagsInTodoItems()
    {
        var userId = await RunAsDefaultUserAsync();
        var tags = new List<string>();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId= await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tasks"
        });

        var command =new AddTagsCommand
        {
            ItemId = itemId,
            Tags = "Test;Life;Blessed"
        };

        tags = command.Tags.Split(';').ToList();

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(listId);
        item.Tags.Count.Should().Be(tags.Count);
        item.Tags.Any(x => x.Name == tags[0]).Should().BeTrue();
        item.Tags.Any(x => x.Name == tags[1]).Should().BeTrue();
        item.Tags.Any(x => x.Name == tags[2]).Should().BeTrue();
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
