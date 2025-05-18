using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class AcademicTerm
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int CreatedById { get; set; }

    public virtual ICollection<ClassroomReservation> ClassroomReservations { get; set; } = new List<ClassroomReservation>();

    public virtual User CreatedBy { get; set; } = null!;
}
