using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Enums;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public record GetTodosQuery : IRequest<TodosVm>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var list = new List<TodoListDto>();
        var result = await _context.TodoLists
                .AsNoTracking()
                .AsQueryable()
                .Include(x => x.Items.Where(i => i.Deleted == null))
                .Where(x => x.Deleted == null)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken);

        foreach ( var todoList in result)
        {
            var items = new List<TodoItemDto>();

            foreach (var todoItem in todoList.Items)
            {
                items.Add(new TodoItemDto() 
                { 
                    Title = todoItem.Title,
                    Done = todoItem.Done,
                    Id = todoItem.Id,
                    ListId = todoItem.ListId,
                    Note = todoItem.Note,
                    Priority = (int)todoItem.Priority
                });
            }
            list.Add(new TodoListDto()
            {
                Colour = todoList.Colour,
                Id = todoList.Id,
                Title = todoList.Title,
                Items = items
            });

           
        }

        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),

            Lists = list
        };
    }
}
