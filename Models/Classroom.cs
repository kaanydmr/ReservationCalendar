using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Classroom
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Building { get; set; } = null!;

    public int Floor { get; set; }

    public int Capacity { get; set; }

    public bool HasProjector { get; set; }

    public bool HasComputers { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ClassroomFeedback> ClassroomFeedbacks { get; set; } = new List<ClassroomFeedback>();

    public virtual ICollection<ClassroomReservation> ClassroomReservations { get; set; } = new List<ClassroomReservation>();
}
