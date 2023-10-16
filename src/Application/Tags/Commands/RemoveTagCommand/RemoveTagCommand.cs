using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Commands.RemoveTagCommand;

public record RemoveTagCommand : IRequest
{
    public int ItemId { get; init; }

    public string Tag { get; init; }
}

public class RemoveTagCommandHandler : IRequestHandler<RemoveTagCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RemoveTagCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .AsNoTracking ()
            .Include(x => x.Tags)
            .SingleOrDefaultAsync(x => x.Id == request.ItemId);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Tag), request.ItemId);
        }
        var tag = entity.Tags.FirstOrDefault(x => x.Name == request.Tag);


        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
