using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Pages.Instructor
{
    [Authorize(Roles = "Instructor")]
    public class EmailModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public EmailModel(ReservationDbContext context)
        {
            _context = context;
        }

        public IList<ContactMessage> ContactMessages { get; set; }

        public async Task OnGetAsync()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            ContactMessages = await _context.ContactMessages
                .Include(m => m.User)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostMarkAsReadAsync(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
            // Refresh the list
            await OnGetAsync();
            return Page();
        }
    }
}