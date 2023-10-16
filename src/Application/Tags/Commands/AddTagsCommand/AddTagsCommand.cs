using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Events;

namespace Todo_App.Application.Tags.Commands.AddTagsCommand;
public record AddTagsCommand : IRequest
{
    public int ItemId { get; init; }

    public string? Tags { get; init; }
}

public class AddTagsCommandHandler : IRequestHandler<AddTagsCommand>
{
    private readonly IApplicationDbContext _context;

    public AddTagsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AddTagsCommand request, CancellationToken cancellationToken)
    {

        foreach (var tag in request.Tags.Trim().Split(';'))
        {
            var tempTag = new Tag(request.ItemId, tag);
             _context.Tags.Add(tempTag);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
