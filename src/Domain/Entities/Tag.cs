using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Todo_App.Domain.Entities;
public class Tag : BaseAuditableEntity
{
    public int ItemId { get; private set; }

    public string? Name { get; private set; }
    public TodoItem Item { get; set; } = null!;


    public Tag(int itemId, string name)
    {
        ItemId = itemId;
        Name = name;
    }


}
