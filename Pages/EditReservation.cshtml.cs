using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Data;
using Project.Models;
using Microsoft.AspNetCore.Authorization;

namespace Project.Pages
{
    [Authorize(Roles = "Admin,Instructor")]
    public class EditReservationModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public EditReservationModel(ReservationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClassroomReservation Reservation { get; set; }

        public int AcademicTermId { get; set; }
        public int ClassroomId { get; set; }
        public int InstructorId { get; set; }

        public SelectList Classrooms { get; set; }
        public SelectList AcademicTerms { get; set; }
        public SelectList Instructors { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Reservation = await _context.ClassroomReservations.FindAsync(id);
            if (Reservation == null)
                return NotFound();

            await PopulateDropdowns(); // <-- Always call this!
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("OnPostAsync called");
            await PopulateDropdowns();

            // Remove navigation property errors
            ModelState.Remove("Reservation.Classroom");
            ModelState.Remove("Reservation.Instructor");
            ModelState.Remove("Reservation.AcademicTerm");

            Console.WriteLine($"Posted AcademicTermId: {Reservation.AcademicTermId}");
            Console.WriteLine($"Posted ClassroomId: {Reservation.ClassroomId}");
            Console.WriteLine($"Posted InstructorId: {Reservation.InstructorId}");
            Console.WriteLine($"Posted StartDate: {Reservation.StartDate}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"ModelState error for {key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            var reservation = await _context.ClassroomReservations.FindAsync(Reservation.Id);
            if (reservation == null)
            {
                Console.WriteLine("Reservation not found in DB");
                return NotFound();
            }

            // Update fields
            reservation.Title = Reservation.Title;
            reservation.Description = Reservation.Description;
            reservation.ClassroomId = Reservation.ClassroomId;
            reservation.AcademicTermId = Reservation.AcademicTermId;
            reservation.DayOfWeek = Reservation.DayOfWeek;
            reservation.StartDate = Reservation.StartDate;
            reservation.EndDate = Reservation.EndDate;
            reservation.StartTime = Reservation.StartTime;
            reservation.EndTime = Reservation.EndTime;
            reservation.Status = Reservation.Status;
            reservation.RejectionReason = Reservation.RejectionReason;
            reservation.IsHoliday = Reservation.IsHoliday;
            reservation.InstructorId = Reservation.InstructorId;
            reservation.UpdatedAt = System.DateTime.Now;

            _context.SystemLogs.Add(new SystemLog
            {
                UserId = reservation.InstructorId,
                ActionType = "ReservationEdited",
                Description = $"Reservation ID {reservation.Id} edited by user {reservation.InstructorId}.",
                Timestamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsError = false
            });
            await _context.SaveChangesAsync();

            Console.WriteLine("Reservation updated and saved.");
            return RedirectToPage("/Calendar");
        }

        private async Task PopulateDropdowns()
        {
            Classrooms = new SelectList(await _context.Classrooms.ToListAsync(), "Id", "Name", Reservation?.ClassroomId);
            AcademicTerms = new SelectList(await _context.AcademicTerms.ToListAsync(), "Id", "Name", Reservation?.AcademicTermId);
            var instructors = await _context.Users
                .Where(u => u.Role == "Instructor" && u.IsActive)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    u.Id,
                    FullName = u.LastName + ", " + u.FirstName
                })
                .ToListAsync();
            Instructors = new SelectList(instructors, "Id", "FullName", Reservation?.InstructorId);
        }
    }
}