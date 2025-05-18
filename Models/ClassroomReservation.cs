using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ClassroomReservation
{
    public int Id { get; set; }

    public int ClassroomId { get; set; }

    public int InstructorId { get; set; }

    public int AcademicTermId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; } = null!;

    public string? RejectionReason { get; set; }

    public bool IsHoliday { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? ApprovedRejectedById { get; set; }

    public virtual AcademicTerm AcademicTerm { get; set; } = null!;

    public virtual User? ApprovedRejectedBy { get; set; }

    public virtual Classroom Classroom { get; set; } = null!;

    public virtual User Instructor { get; set; } = null!;
}
