using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Project.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateAdminModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public CreateAdminModel(ReservationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        [BindProperty]
        public string LastName { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                return Page();
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Email);

            if (existingUser != null)
            {
                ErrorMessage = "An account with this email already exists.";
                return Page();
            }

            // Hash the password before storing
            var passwordHashSalt = PasswordHelper.HashPassword(Password);
            var hashedPassword = passwordHashSalt.hash;
            var salt = passwordHashSalt.salt;

            // Create new admin user
            var newAdmin = new User
            {
                Email = Email,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                FirstName = FirstName,
                LastName = LastName,
                Role = "Admin",
                IsActive = true
            };

            _context.Users.Add(newAdmin);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/Index");
        }
    }
}