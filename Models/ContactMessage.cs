using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ContactMessage
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
