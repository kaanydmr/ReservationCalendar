using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Microsoft.AspNetCore.Authorization;
using Project.Data;

namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateClassroomModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public CreateClassroomModel(ReservationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Classroom Classroom { get; set; } = new Classroom();

        public void OnGet()
        {
            // Optionally initialize defaults
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            Classroom.CreatedAt = DateTime.Now;
            Classroom.UpdatedAt = DateTime.Now;

            _context.Classrooms.Add(Classroom);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/Index");
        }
    }
}