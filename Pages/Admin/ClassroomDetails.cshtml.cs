using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ClassroomDetailsModel : PageModel
    {
        private readonly ReservationDbContext _context;
        public ClassroomDetailsModel(ReservationDbContext context)
        {
            _context = context;
        }

        public Classroom Classroom { get; set; }
        public List<ClassroomFeedback> Feedbacks { get; set; } = new();
        public double AverageRating { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Classroom = await _context.Classrooms
                .Include(c => c.ClassroomFeedbacks)
                    .ThenInclude(f => f.Instructor)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Classroom == null)
                return NotFound();

            Feedbacks = Classroom.ClassroomFeedbacks.ToList();
            AverageRating = Feedbacks.Any() ? Feedbacks.Average(f => f.Rating) : 0;
            return Page();
        }
    }
}