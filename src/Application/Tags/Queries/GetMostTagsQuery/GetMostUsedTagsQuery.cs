using System;
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
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Queries.GetMostTagsQuery;

public record GetMostUsedTagsQuery : IRequest<List<TagDto>>
{
    public int Id { get; init; }
}
public class GetMostTagsQueryHandler : IRequestHandler<GetMostUsedTagsQuery, List<TagDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMostTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TagDto>> Handle(GetMostUsedTagsQuery request, CancellationToken cancellationToken)
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
            .GroupBy(t => t.Name)
            .OrderByDescending(group => group.Count())
            .Select(group => new
            {
                Name = group.Key,
                Count = group.Count()
            })
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var tag in result)
        {
            tags.Add(new TagDto() { Id = 0, Name = tag.Name });
        }

        return tags;
    }
}