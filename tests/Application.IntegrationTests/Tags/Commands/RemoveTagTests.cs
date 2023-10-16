using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Tags.Commands.AddTagsCommand;
using Todo_App.Application.Tags.Commands.RemoveTagCommand;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoItems.Commands.DeleteTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.Tags.Commands;

using static AutoMapper.Internal.CollectionMapperExpressionFactory;
using static Testing;

public class RemoveTagTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new RemoveTagCommand()
        {
            ItemId = 1,
            Tag = "test"
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldRemoveTagsInTodoItems()
    {
        var userId = await RunAsDefaultUserAsync();
        var tags = new List<string>();
        var tagString = "Test;Life;Blessed";

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tasks"
        });

        var addTag = await SendAsync(new AddTagsCommand
        {
            ItemId = itemId,
            Tags = tagString
        });

        tags = tagString.Split(';').ToList();

        await SendAsync(new RemoveTagCommand()
        {
            ItemId = itemId,
            Tag = tags[0]
        });

        var item = await FindAsync<TodoItem>(itemId);


        item.Should().NotBeNull();
        item!.ListId.Should().Be(listId);
        item.Tags.Count.Should().Be(tags.Count - 1);
        item.Tags.Any(x => x.Name == tags[0]).Should().BeFalse();
        item.Tags.Any(x => x.Name == tags[1]).Should().BeTrue();
        item.Tags.Any(x => x.Name == tags[2]).Should().BeTrue();
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}