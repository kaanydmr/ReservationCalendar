using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Project.Data; 
using Project.Models; 
using Project.Pages;

/* Promts:
1. Index and Calendar UI Logic
Show warning for guests: Asked how to display a warning when a guest (not logged in) clicks "View Calendar".
Button sizing: Wanted the "Login" and "View Calendar" buttons to be the same size.
Remove/hide filters: Asked to remove or hide the search/filter dropdowns from the top of the calendar.
*/

namespace Project.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public LoginModel(ReservationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                return Page();
            }

            // Email format validation
            try
            {
                var addr = new System.Net.Mail.MailAddress(Email);
                if (addr.Address != Email)
                {
                    ErrorMessage = "Please enter a valid email address.";
                    return Page();
                }
            }
            catch
            {
                ErrorMessage = "Please enter a valid email address.";
                return Page();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Email && u.IsActive);

            // Log login attempt
            _context.SystemLogs.Add(new SystemLog
            {
                UserId = user?.Id,
                ActionType = "LoginAttempt",
                Description = user == null ? $"Failed login for {Email}" : $"Successful login for {Email}",
                Timestamp = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsError = user == null
            });
            await _context.SaveChangesAsync();

            if (user == null || !VerifyPassword(Password, user.PasswordHash, user.PasswordSalt))
            {
                ErrorMessage = "Invalid credentials.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect based on role
            if (user.Role == "Admin")
                return RedirectToPage("/Admin/Index");
            else
                return RedirectToPage("/Instructor/Index");
        }

        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(storedSalt));
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(computedHash) == storedHash;
        }
    }
}