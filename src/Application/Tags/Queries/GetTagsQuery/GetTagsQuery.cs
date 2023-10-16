using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Tags.DTOs;
using Todo_App.Application.TodoLists.Queries.GetTodos;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Queries.GetTagsQuery;
public record GetTagsQuery : IRequest<List<TagDto>>
{
    public int Id { get; init; }
}
public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery,List<TagDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = new List<TagDto>();
        var entity = await _context.TodoLists
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoLists), request.Id);
        }

        var result = await _context.TodoLists
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .SelectMany(x => x.Items)
            .SelectMany(x => x.Tags)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        foreach ( var tag in result )
        {
            tags.Add(new TagDto() { Id = 0, Name = tag.Name });
        }

        return tags.DistinctBy(x => x.Name).ToList();
    }
}

