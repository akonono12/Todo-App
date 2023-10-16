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
using Todo_App.Application.Tags.Queries.GetMostTagsQuery;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Application.TodoLists.Queries.GetTodos;
using Todo_App.Domain.Entities;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Application.IntegrationTests.Tags.Queries;


using static Testing;

public class GetMostUsedTagsTests : BaseTestFixture
{

    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new GetMostUsedTagsQuery()
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

        var query = new GetMostUsedTagsQuery()
        {
            Id = listId
        };

        var result = await SendAsync(query);

        result.Should().HaveCount(0);
    }

    [Test]
    public async Task ShouldReturnMostUsedTags()
    {
        await RunAsDefaultUserAsync();
        var tags = new List<string>() { "Happy","Sad","Balance", "Sad", "Word", "Work", "OOTD", "Angry", "Love", "Sample" };
        var tagString1 =string.Join(';',tags);
        var tagString2 = tagString1+ ";Test;Life;Blessed";

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
            Tags = tagString1
        });

        await SendAsync(new AddTagsCommand
        {
            ItemId = itemId2,
            Tags = tagString2
        });
        var query = new GetMostUsedTagsQuery()
        {
            Id = listId
        };

        var result = await SendAsync(query);


        result.Should().HaveCount(10);
        foreach(var item in tags)
        {
         result.Any(x =>x.Name == item).Should().BeTrue();
        }
    }


}
