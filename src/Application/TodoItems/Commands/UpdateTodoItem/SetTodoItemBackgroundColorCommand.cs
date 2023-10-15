using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItems.Commands.UpdateTodoItem;


public record SetTodoItemBackgroundColorCommand : IRequest
{
    public int Id { get; init; }

    public string? BGColor { get; init; }
}

public class SetTodoItemBackgroundColorCommandHandler : IRequestHandler<SetTodoItemBackgroundColorCommand>
{
    private readonly IApplicationDbContext _context;

    public SetTodoItemBackgroundColorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SetTodoItemBackgroundColorCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        entity.SetBackGroundColor(request.BGColor);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
