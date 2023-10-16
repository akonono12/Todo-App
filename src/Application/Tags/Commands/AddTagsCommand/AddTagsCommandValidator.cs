using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;

namespace Todo_App.Application.Tags.Commands.AddTagsCommand;

public class AddTagsCommandValidator : AbstractValidator<AddTagsCommand>
{
    public AddTagsCommandValidator()
    {
        RuleFor(x => x.Tags)
            .MaximumLength(200)
            .NotEmpty();
    }
}
