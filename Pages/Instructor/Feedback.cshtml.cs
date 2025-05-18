// filepath: /Users/kaan/Desktop/untitled folder 3/Project/Pages/Instructor/Feedback.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Project.Helpers;

namespace Project.Pages.Instructor
{
    [Authorize(Roles = "Admin,Instructor")]
    public class FeedbackModel : PageModel
    {
        private readonly ReservationDbContext _context;
        public FeedbackModel(ReservationDbContext context) => _context = context;

        [BindProperty]
        public ClassroomFeedback Feedback { get; set; } = new();

        public SelectList Classrooms { get; set; }
        public string Message { get; set; }

        public async Task OnGetAsync()
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            // Only classrooms the instructor has reservations for
            var classroomIds = await _context.ClassroomReservations
                .Where(r => r.InstructorId == userId)
                .Select(r => r.ClassroomId)
                .Distinct()
                .ToListAsync();

            var classrooms = await _context.Classrooms
                .Where(c => classroomIds.Contains(c.Id))
                .ToListAsync();

            Classrooms = new SelectList(classrooms, "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            Feedback.InstructorId = userId;
            Feedback.CreatedAt = DateTime.Now;
            _context.ClassroomFeedbacks.Add(Feedback);
            await _context.SaveChangesAsync();

            _context.SystemLogs.Add(new SystemLog
            {
                UserId = userId,
                ActionType = "FeedbackSubmitted",
                Description = $"Feedback submitted for classroom {Feedback.ClassroomId} by user {userId}.",
                Timestamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsError = false
            });
            await _context.SaveChangesAsync();

            // Find an admin user to send the message to
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
            if (adminUser != null)
            {
                await MailHelper.SendMessageAsync(
                    _context,
                    senderUserId: 1, 
                    subject: "New Classroom Feedback Submitted",
                    message: $"A new feedback has been submitted for classroom ID {Feedback.ClassroomId}.\n\nFeedback: {Feedback.Comment}, By Instructor ID {userId}"
                );
            }

            Message = "Feedback sent!";
            await OnGetAsync();
            return Page();
        }
    }
}