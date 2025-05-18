using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateInstructorModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public CreateInstructorModel(ReservationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public string FirstName { get; set; }
        [BindProperty] public string LastName { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        public string Message { get; set; }
        public List<User> Instructors { get; set; } = new();

        public async Task OnGetAsync()
        {
            Instructors = await _context.Users
                .Where(u => u.Role == "Instructor")
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (_context.Users.Any(u => u.Email == Email))
            {
                Message = "Email already exists.";
                Instructors = await _context.Users.Where(u => u.Role == "Instructor").ToListAsync();
                return Page();
            }

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var newUser = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Role = "Instructor",
                PasswordSalt = Convert.ToBase64String(hmac.Key),
                PasswordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Password))),
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            Message = "Instructor created successfully.";
            Instructors = await _context.Users.Where(u => u.Role == "Instructor").ToListAsync();
            return Page();
        }
    }
}