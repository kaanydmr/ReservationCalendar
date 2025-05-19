using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Security.Claims;
using Project.Helpers;

/* Promts:
Calendar Filtering & API

You asked how to add a link from the classroom list to show a filtered version of the calendar for a specific classroom.
You wanted to confirm if the calendar supports filtering by classroomId via the query string.
You requested improvements to the appearance of the "Details" and "View Calendar" links in the classroom list.
API Filtering: You asked about filtering the calendar API by classroomId and other parameters.
*/


[Route("api/reservations")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly ReservationDbContext _context;
    public ReservationsController(ReservationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReservations(
        [FromQuery] int? academicTermId = null,
        [FromQuery] int? classroomId = null,
        [FromQuery] string status = null,
        [FromQuery] int? instructorId = null
    )
    {
        var query = _context.ClassroomReservations
            .Include(r => r.Classroom)
            .Include(r => r.AcademicTerm)
            .Include(r => r.Instructor)
            .AsQueryable();

        if (academicTermId.HasValue)
            query = query.Where(r => r.AcademicTermId == academicTermId.Value);
        if (classroomId.HasValue)
            query = query.Where(r => r.ClassroomId == classroomId.Value);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);
        if (instructorId.HasValue)
            query = query.Where(r => r.InstructorId == instructorId.Value);

        var reservations = await query.ToListAsync();

        var events = new List<object>();

        foreach (var r in reservations)
        {
            // Expand to all matching days between StartDate and EndDate
            for (var date = r.StartDate; date <= r.EndDate; date = date.AddDays(1))
            {
                if ((byte)date.DayOfWeek == r.DayOfWeek)
                {
                    // Check for conflicts: same classroom, same date, overlapping time, not rejected, different reservation
                    bool hasConflict = reservations.Any(other =>
                        other.Id != r.Id &&
                        other.ClassroomId == r.ClassroomId &&
                        other.Status != "Rejected" &&
                        // This date must be in other's range and same day of week
                        date >= other.StartDate && date <= other.EndDate &&
                        (byte)date.DayOfWeek == other.DayOfWeek &&
                        // Overlapping time
                        other.StartTime < r.EndTime &&
                        other.EndTime > r.StartTime
                    );

                    events.Add(new
                    {
                        id = r.Id,
                        title = r.Title,
                        start = date.ToString("yyyy-MM-dd") + "T" + r.StartTime.ToString(@"HH\:mm"),
                        end = date.ToString("yyyy-MM-dd") + "T" + r.EndTime.ToString(@"HH\:mm"),
                        description = r.Description,
                        classroom = r.Classroom?.Name,
                        instructor = r.Instructor?.FirstName + " " + r.Instructor?.LastName,
                        instructorId = r.InstructorId,
                        dayOfWeek = ((System.DayOfWeek)r.DayOfWeek).ToString(),
                        startDate = r.StartDate.ToString("yyyy-MM-dd"),
                        endDate = r.EndDate.ToString("yyyy-MM-dd"),
                        startTime = r.StartTime.ToString(@"HH\:mm"),
                        endTime = r.EndTime.ToString(@"HH\:mm"),
                        status = r.Status,
                        rejectionReason = r.RejectionReason,
                        hasConflict = hasConflict,
                        isHoliday = false,
                        holidayName = ""
                    });
                }
            }
        }

        return Ok(events);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveReservation(int id)
    {
        var reservation = await _context.ClassroomReservations
            .Include(r => r.Classroom)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null)
            return NotFound();

        // Check for conflicts with other non-rejected, non-this reservations
        var conflicting = await _context.ClassroomReservations
            .Where(r =>
                r.Id != reservation.Id &&
                r.ClassroomId == reservation.ClassroomId &&
                r.Status != "Rejected" &&
                // Overlapping date range and day of week
                reservation.StartDate <= r.EndDate &&
                reservation.EndDate >= r.StartDate &&
                r.DayOfWeek == reservation.DayOfWeek &&
                // Overlapping time
                r.StartTime < reservation.EndTime &&
                r.EndTime > reservation.StartTime
            )
            .AnyAsync();

        if (conflicting)
        {
            return BadRequest(new { message = "Cannot approve: this reservation conflicts with another reservation." });
        }

        reservation.Status = "Approved";
        reservation.RejectionReason = null;

        _context.SystemLogs.Add(new SystemLog
        {
            UserId = int.TryParse(User.FindFirst("UserId")?.Value, out var uid) ? uid : (int?)null,
            ActionType = "ReservationApproved",
            Description = $"Reservation ID {reservation.Id} approved by admin.",
            Timestamp = DateTime.Now,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            IsError = false
        });
        await _context.SaveChangesAsync();

        // Notify instructor
        await MailHelper.SendMessageAsync(
            _context,
            reservation.InstructorId,
            "Reservation Approved",
            $"Your reservation '{reservation.Title}' for {reservation.StartDate:yyyy-MM-dd} ({reservation.StartTime}-{reservation.EndTime}) in classroom '{reservation.Classroom?.Name}' has been approved."
        );


        return Ok();
    }

    public class RejectDto
    {
        public string RejectionReason { get; set; }
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectReservation(int id, [FromBody] RejectDto dto)
    {
        var reservation = await _context.ClassroomReservations
            .Include(r => r.Classroom)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null)
            return NotFound();

        reservation.Status = "Rejected";
        reservation.RejectionReason = dto.RejectionReason;

        _context.SystemLogs.Add(new SystemLog
        {
            UserId = int.TryParse(User.FindFirst("UserId")?.Value, out var uid) ? uid : (int?)null,
            ActionType = "ReservationRejected",
            Description = $"Reservation ID {reservation.Id} rejected by admin. Reason: {dto.RejectionReason}",
            Timestamp = DateTime.Now,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            IsError = false
        });
        await _context.SaveChangesAsync();

        // Notify instructor
        await MailHelper.SendMessageAsync(
            _context,
            reservation.InstructorId,
            "Reservation Rejected",
            $"Your reservation '{reservation.Title}' for {reservation.StartDate:yyyy-MM-dd} ({reservation.StartTime}-{reservation.EndTime}) in classroom '{reservation.Classroom?.Name}' was rejected. Reason: {reservation.RejectionReason}"
        );

        return Ok();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        try
        {
            var reservation = await _context.ClassroomReservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            var userIdClaim = User.FindFirstValue("UserId");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                if (string.IsNullOrEmpty(userIdClaim))
                    return Forbid();

                if (!int.TryParse(userIdClaim, out var userId))
                    return Forbid();

                if (reservation.InstructorId != userId)
                    return Forbid();
            }

            _context.ClassroomReservations.Remove(reservation);

            _context.SystemLogs.Add(new SystemLog
            {
                UserId = reservation.InstructorId,
                ActionType = "ReservationDeleted",
                Description = $"Reservation ID {reservation.Id} deleted by user {reservation.InstructorId}.",
                Timestamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsError = false
            });
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error cancelling reservation: " + ex.Message);
            return StatusCode(500, new { success = false, error = "An error occurred while cancelling the reservation." });
        }
    }
}