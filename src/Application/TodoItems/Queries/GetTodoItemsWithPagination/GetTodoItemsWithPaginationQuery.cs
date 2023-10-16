using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Mappings;
using Todo_App.Application.Common.Models;
using Todo_App.Application.TodoLists.Queries.GetTodos;

namespace Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsWithPaginationQuery : IRequest<List<TodoItemDto>>
{
    public int ListId { get; init; }
    public string? ItemName { get; init; }
    public string? TagName { get; init; }
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, List<TodoItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TodoItemDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TodoItems
               .AsNoTracking()
               .AsQueryable()
               .Where(x => x.ListId == request.ListId);

        if (!string.IsNullOrEmpty(request.ItemName))
        {
            query = query.Where(x => x.Title.Contains(request.ItemName));

        }

        if (!string.IsNullOrEmpty(request.TagName))
        {
            query = query.Where(x => x.Tags.Any(x => x.Name.ToLower() == request.TagName.ToLower()));

        }

        return await query
            .OrderBy(x => x.Title)
            .ProjectTo<TodoItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
