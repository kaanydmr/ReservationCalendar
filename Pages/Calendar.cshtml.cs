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


/* Promts:
1. Index and Calendar UI Logic
Asked how to show a warning when "View Calendar" is clicked by a guest.
Wanted the "Login" and "View Calendar" buttons to be the same size.
Asked to remove or hide the search/filter dropdowns from the top of the calendar.
2. Calendar Filtering and API
Reported that calendar filters are not working.
Requested a fix so that filters use database queries (WHERE) and refresh the calendar.
Shared your ReservationsApiController code and discussed how to apply filters in the controller.
Encountered and fixed a DateOnly vs DateTime comparison error.
Noted that after applying filters, no events appeared on the calendar.
Discussed the event expansion logic and the impact of the DayOfWeek check.
Confirmed that using .ToListAsync() directly works, but using .AsQueryable() with filters did not seem to work as expected.
Determined that the issue was likely with how input parameters were handled or sent from the frontend.
3. Calendar Filtering and API
Filters not working: Reported that calendar filters were not functioning as expected.
Database filtering: Requested a fix so that filters use database queries (WHERE clauses) and refresh the calendar.
Shared API code: Shared your ReservationsApiController code and discussed how to apply filters in the controller.
DateOnly vs DateTime: Encountered and fixed a comparison error between DateOnly and DateTime.
No events after filtering: Noted that after applying filters, no events appeared on the calendar.
Event expansion logic: Discussed the event expansion logic and the impact of the DayOfWeek check.
.ToListAsync() vs .AsQueryable(): Confirmed that using .ToListAsync() directly worked, but using .AsQueryable() with filters did not.
Frontend parameter handling: Determined the issue was likely with how input parameters were handled or sent from the frontend.
Calendar Holiday Warning:

You asked how to make the holiday warning appear on every reservation that overlaps a holiday, not just on the exact holiday date.
Guest Calendar Access Warning:

You asked how to show a warning when a guest (not logged in) clicks "View Calendar".
Button Sizing:

You wanted the "Login" and "View Calendar" buttons to be the same size.
Calendar Filters:

You asked how to remove or hide the search/filter dropdowns from the top of the calendar.
You reported that calendar filters were not working and requested a fix so that filters use database queries (WHERE clauses) and refresh the calendar.
You shared your ReservationsApiController code and discussed how to apply filters in the controller.
You encountered and fixed a DateOnly vs DateTime comparison error.
You noted that after applying filters, no events appeared on the calendar.
You discussed the event expansion logic and the impact of the DayOfWeek check.
You confirmed that using .ToListAsync() directly worked, but using .AsQueryable() with filters did not.
You determined that the issue was likely with how input parameters were handled or sent from the frontend.
*/

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