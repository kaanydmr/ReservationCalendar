using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<AcademicTerm> AcademicTerms { get; set; } = new List<AcademicTerm>();

    public virtual ICollection<ClassroomFeedback> ClassroomFeedbacks { get; set; } = new List<ClassroomFeedback>();

    public virtual ICollection<ClassroomReservation> ClassroomReservationApprovedRejectedBies { get; set; } = new List<ClassroomReservation>();

    public virtual ICollection<ClassroomReservation> ClassroomReservationInstructors { get; set; } = new List<ClassroomReservation>();

    public virtual ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();

    public virtual ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
}
