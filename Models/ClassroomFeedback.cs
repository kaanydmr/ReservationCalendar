using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ClassroomFeedback
{
    public int Id { get; set; }

    public int ClassroomId { get; set; }

    public int InstructorId { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Classroom Classroom { get; set; } = null!;

    public virtual User Instructor { get; set; } = null!;
}
