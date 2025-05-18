using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class SystemLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string ActionType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string? IpAddress { get; set; }

    public bool IsError { get; set; }

    public virtual User? User { get; set; }
}
