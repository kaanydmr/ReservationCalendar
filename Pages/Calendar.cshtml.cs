using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Project.Pages
{
    [Authorize(Roles = "Admin,Instructor")]
    public class CalendarModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public CalendarModel(ReservationDbContext context)
        {
            _context = context;
        }

        public SelectList AcademicTerms { get; set; }
        public SelectList Classrooms { get; set; }
        public SelectList Instructors { get; set; }
        public int CurrentUserId { get; set; }
        public string HiddenDayOfWeek { get; set; }
        public List<ClassroomReservation> Reservations { get; set; }
        public List<Holiday> Holidays { get; set; } // Add this property
        public string CurrentUserName { get; set; }
        public int? SelectedClassroomId { get; set; }

        public async Task OnGetAsync(int? classroomId = null)
        {
            var nameId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CurrentUserName = User.Identity?.Name;

            var userIdClaim = User.FindFirstValue("UserId");
            if (int.TryParse(userIdClaim, out int userId))
            {
                CurrentUserId = userId;
            }
            else
            {
                CurrentUserId = 0;
            }

            // Load active academic terms
            var academicTerms = await _context.AcademicTerms
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();
            AcademicTerms = new SelectList(academicTerms, "Id", "Name");

            // Load active classrooms
            var classrooms = await _context.Classrooms
                .Where(c => c.IsActive)
                .OrderBy(c => c.Building)
                .ThenBy(c => c.Name)
                .ToListAsync();
            SelectedClassroomId = classroomId;
            Classrooms = new SelectList(classrooms, "Id", "Name", SelectedClassroomId, "Building");

            // For admin, load all instructors
            if (User.IsInRole("Admin"))
            {
                var instructors = await _context.Users
                    .Where(u => u.Role == "Instructor" && u.IsActive)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new
                    {
                        u.Id,
                        FullName = $"{u.LastName}, {u.FirstName}"
                    })
                    .ToListAsync();
                Instructors = new SelectList(instructors, "Id", "FullName");
            }

            // Load all reservations
            Reservations = await _context.ClassroomReservations
                .Include(r => r.Classroom)
                .Include(r => r.AcademicTerm)
                .Include(r => r.Instructor)
                .ToListAsync();

            // Load all holidays
            Holidays = await _context.Holidays.ToListAsync(); // Add this line
        }
    }
}