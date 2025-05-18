using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using Project.Models;
using System.Linq;
using System.Threading.Tasks;
using Project.Pages;

namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditInstructorModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public EditInstructorModel(ReservationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public int Id { get; set; }
        [BindProperty] public string FirstName { get; set; }
        [BindProperty] public string LastName { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            if (_context.Users.Any(u => u.Email == Email && u.Id != Id))
            {
                Message = "Email already exists.";
                return Page();
            }

            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;

            if (!string.IsNullOrEmpty(Password))
            {
                using var hmac = new System.Security.Cryptography.HMACSHA512();
                user.PasswordSalt = Convert.ToBase64String(hmac.Key);
                user.PasswordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password)));
            }

            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            Message = "Instructor updated successfully.";
            return Page();
        }
    }
}