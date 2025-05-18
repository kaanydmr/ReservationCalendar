using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Holiday
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
