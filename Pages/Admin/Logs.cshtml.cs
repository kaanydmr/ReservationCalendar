using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class LogsModel : PageModel
    {
        private readonly ReservationDbContext _context;
        public LogsModel(ReservationDbContext context) => _context = context;

        public List<SystemLog> Logs { get; set; }

        public async Task OnGetAsync()
        {
            Logs = await _context.SystemLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}