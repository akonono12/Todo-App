using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.Tags.Commands.AddTagsCommand;
using Todo_App.Application.Tags.Commands.RemoveTagCommand;
using Todo_App.Application.Tags.DTOs;
using Todo_App.Application.Tags.Queries.GetMostTagsQuery;
using Todo_App.Application.Tags.Queries.GetTagsQuery;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;

namespace Todo_App.WebUI.Controllers;

public class TagsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Unit>> Add(AddTagsCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Unit>> Remove(RemoveTagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("get-tag-options")]
    public async Task<ActionResult<List<TagDto>>> GetTagOptions([FromQuery] GetTagsQuery command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("get-most-used-tag")]
    public async Task<ActionResult<List<TagDto>>> GetMostUsedTag([FromQuery] GetMostUsedTagsQuery command)
    {
        return await Mediator.Send(command);
    }
}
