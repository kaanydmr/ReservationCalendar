using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
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
            
            ContactMessages = await _context.ContactMessages
                .Include(m => m.User)
                .Where(m => m.UserId == 1)
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