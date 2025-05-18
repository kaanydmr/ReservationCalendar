using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ClassroomListModel : PageModel
    {
        private readonly ReservationDbContext _context;
        public ClassroomListModel(ReservationDbContext context) => _context = context;

        public List<Classroom> Classrooms { get; set; }

        public async Task OnGetAsync()
        {
            Classrooms = await _context.Classrooms
                .Include(c => c.ClassroomFeedbacks)
                .ToListAsync();
        }
    }
}