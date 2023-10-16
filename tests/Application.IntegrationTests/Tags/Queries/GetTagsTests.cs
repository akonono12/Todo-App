using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Tags.Commands.AddTagsCommand;
using Todo_App.Application.Tags.Queries.GetMostTagsQuery;
using Todo_App.Application.Tags.Queries.GetTagsQuery;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Domain.Entities;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.IntegrationTests.Tags.Queries;

using static Testing;

public class GetTagsTests : BaseTestFixture
{

    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new GetTagsQuery()
        {
            Id = 1,
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }



    [Test]
    public async Task ShouldReturnZeroResult()
    {
        await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await AddAsync(new TodoList
        {
            Title = "Shopping",
            Colour = Colour.Blue,
            Items =
                    {
                        new TodoItem { Title = "Apples", Done = true},
                        new TodoItem { Title = "Milk", Done = true },
                        new TodoItem { Title = "Bread", Done = true },
                        new TodoItem { Title = "Toilet paper" },
                        new TodoItem { Title = "Pasta" },
                        new TodoItem { Title = "Tissues" },
                        new TodoItem { Title = "Tuna" }
                    }
        });

        var query = new GetTagsQuery()
        {
            Id = listId
        };

        var result = await SendAsync(query);

        result.Should().HaveCount(0);
    }

    [Test]
    public async Task ShouldReturnTagOptions()
    {
        await RunAsDefaultUserAsync();
        var tags1 = new List<string>() { "OOTD", "Blessed",};
        var tags2 = new List<string>() {  "Word", "Life", "Work" };

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tasks"
        });
        var itemId2 = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tasks2"
        });

        await SendAsync(new AddTagsCommand
        {
            ItemId = itemId,
            Tags = string.Join(";", tags1)
        });

        await SendAsync(new AddTagsCommand
        {
            ItemId = itemId2,
            Tags = string.Join(";", tags2) + ";OOTD;Work"
        });
        var query = new GetTagsQuery()
        {
            Id = listId
        };

        var result = await SendAsync(query);

         tags1.AddRange(tags2);
        result.Should().HaveCount(tags1.Count());
        foreach (var item in tags1)
        {
            result.Any(x => x.Name == item).Should().BeTrue();
        }
    }


}
