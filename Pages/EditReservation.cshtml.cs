using System;
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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Reservation = await _context.ClassroomReservations.FindAsync(id);
            if (Reservation == null)
                return NotFound();

            // Prevent editing if the reservation is approved.
            if (Reservation.Status == "Approved")
            {
                return RedirectToPage("/Calendar");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var reservationToUpdate = await _context.ClassroomReservations.FindAsync(Reservation.Id);
            if (reservationToUpdate == null)
                return NotFound();

            if (reservationToUpdate.Status == "Approved")
                return RedirectToPage("/Calendar");

            // Remove required fields that won't be changed
            ModelState.Remove("Reservation.Status");
            ModelState.Remove("Reservation.AcademicTerm");
            ModelState.Remove("Reservation.Classroom");
            ModelState.Remove("Reservation.Instructor");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update only the Title and Description
            reservationToUpdate.Title = Reservation.Title;
            reservationToUpdate.Description = Reservation.Description;
            reservationToUpdate.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToPage("/Calendar");
        }
    }
}