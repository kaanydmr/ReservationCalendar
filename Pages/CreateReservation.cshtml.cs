using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Data;
using Project.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Project.Helpers;

namespace Project.Pages
{
    [Authorize(Roles = "Instructor")]
    public class CreateReservationModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public CreateReservationModel(ReservationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClassroomReservation Reservation { get; set; } = new ClassroomReservation();

        // Not bound, just for display
        public DayOfWeek? CalculatedDayOfWeek { get; set; }

        public SelectList Classrooms { get; set; }
        public SelectList AcademicTerms { get; set; }

        [BindProperty]
        public string HiddenDayOfWeek { get; set; } // For POST

        public List<Project.Models.AcademicTerm> AllTerms { get; set; }

        public List<Holiday> Holidays { get; set; }

        public async Task<IActionResult> OnGetAsync(DateTime? startDate, TimeSpan? startTime, TimeSpan? endTime)
        {
            var classrooms = await _context.Classrooms
                .Where(c => c.IsActive)
                .OrderBy(c => c.Building)
                .ThenBy(c => c.Name)
                .ToListAsync();
            Classrooms = new SelectList(classrooms, "Id", "Name", null, "Building");

            AllTerms = await _context.AcademicTerms
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();
            AcademicTerms = new SelectList(AllTerms, "Id", "Name");

            Holidays = await _context.Holidays.ToListAsync();


            if (startDate.HasValue)
            {
                Reservation.StartDate = DateOnly.FromDateTime(startDate.Value);
                CalculatedDayOfWeek = startDate.Value.DayOfWeek;
                HiddenDayOfWeek = ((byte)startDate.Value.DayOfWeek).ToString(); // <-- FIXED
            }

            if (Reservation.StartDate == default)
            {
                Reservation.StartDate = DateOnly.FromDateTime(DateTime.Today);
            }

            if (startTime.HasValue)
                Reservation.StartTime = TimeOnly.FromTimeSpan(startTime.Value);
            if (endTime.HasValue)
                Reservation.EndTime = TimeOnly.FromTimeSpan(endTime.Value);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Always reload dropdowns for redisplay on validation error
            var classrooms = await _context.Classrooms
                .Where(c => c.IsActive)
                .OrderBy(c => c.Building)
                .ThenBy(c => c.Name)
                .ToListAsync();
            Classrooms = new SelectList(classrooms, "Id", "Name", null, "Building");

            AllTerms = await _context.AcademicTerms
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();
            AcademicTerms = new SelectList(AllTerms, "Id", "Name");

            // FIX: Load holidays for POST
            Holidays = await _context.Holidays.ToListAsync();

            // Parse the hidden day of week
            Console.WriteLine($"HiddenDayOfWeek: '{HiddenDayOfWeek}', Type: {HiddenDayOfWeek?.GetType()}");
            if (!byte.TryParse(HiddenDayOfWeek, out var dayOfWeekByte))
            {
                Console.WriteLine("Invalid day of week (not a byte).");
                ModelState.AddModelError("", "Invalid day of week.");
                return Page();
            }
            Console.WriteLine($"HiddenDayOfWeek: '{HiddenDayOfWeek}', Type: {HiddenDayOfWeek?.GetType()}");

            var term = await _context.AcademicTerms.FirstOrDefaultAsync(t => t.Id == Reservation.AcademicTermId);
            if (term == null)
            {
                Console.WriteLine("Invalid academic term.");
                ModelState.AddModelError("", "Invalid academic term.");
                return Page();
            }

            var instructorIdStr = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(instructorIdStr))
            {
                Console.WriteLine("Instructor ID claim is missing!");
                ModelState.AddModelError("", "Instructor ID is missing. Please log in again.");
                return Page();
            }

            var newReservation = new ClassroomReservation
            {
                Title = Reservation.Title,
                Description = Reservation.Description,
                ClassroomId = Reservation.ClassroomId,
                AcademicTermId = Reservation.AcademicTermId,
                StartDate = DateOnly.FromDateTime(term.StartDate), // <-- FIXED
                EndDate = DateOnly.FromDateTime(term.EndDate),     // <-- FIXED
                StartTime = Reservation.StartTime,
                EndTime = Reservation.EndTime,
                InstructorId = int.Parse(instructorIdStr),
                CreatedAt = DateTime.Now,
                Status = "Pending",
                DayOfWeek = dayOfWeekByte
            };

            _context.ClassroomReservations.Add(newReservation);

            _context.SystemLogs.Add(new SystemLog
            {
                UserId = int.Parse(instructorIdStr),
                ActionType = "ReservationCreated",
                Description = $"Reservation '{newReservation.Title}' submitted by user {instructorIdStr}",
                Timestamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsError = false
            });

            await _context.SaveChangesAsync();
            Console.WriteLine("Reservation saved to database.");

            // Check if reservation falls on a holiday
            var holiday = Holidays.FirstOrDefault(h =>
                h.Date >= newReservation.StartDate &&
                h.Date <= newReservation.EndDate &&
                h.Date.DayOfWeek == (DayOfWeek)newReservation.DayOfWeek);

            if (holiday != null)
            {
                await MailHelper.SendMessageAsync(
                    _context,
                    newReservation.InstructorId,
                    "Reservation Falls on Official Holiday",
                    $"Your reservation '{newReservation.Title}' for {newReservation.StartDate:yyyy-MM-dd} to {newReservation.EndDate:yyyy-MM-dd} in classroom '{classrooms.FirstOrDefault(c => c.Id == newReservation.ClassroomId)?.Name}' falls on an official holiday: {holiday.Name} ({holiday.Date:yyyy-MM-dd})."
                );
            }

            return RedirectToPage("./Calendar");
        }
    }
}

