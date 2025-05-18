using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;


namespace Project.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AcademicTermsModel : PageModel
    {
        private readonly ReservationDbContext _context;

        public AcademicTermsModel(ReservationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputAcademicTerm NewAcademicTerm { get; set; } = new InputAcademicTerm();

        [BindProperty]
        public InputAcademicTerm EditAcademicTerm { get; set; } = new InputAcademicTerm();

        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; }

        public class AcademicTermViewModel
        {
            public AcademicTerm Term { get; set; }
            public bool HasReservations { get; set; }
        }

        public List<AcademicTermViewModel> AcademicTerms { get; set; } = new();

        // Create a separate input model class to handle form submissions
        public class InputAcademicTerm
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Term name is required")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Start date is required")]
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; } = DateTime.Today; // Default to today

            [Required(ErrorMessage = "End date is required")]
            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; } = DateTime.Today;   // Default to today

            public bool IsActive { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Console.WriteLine("Entering OnGetAsync");
            await LoadTermsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAddTermAsync()
        {
            Console.WriteLine("1. Entering OnPostAddTermAsync");

            // Only validate the NewAcademicTerm property
            ModelState.Clear();
            ModelState.ClearValidationState(nameof(NewAcademicTerm));
            if (!TryValidateModel(NewAcademicTerm, nameof(NewAcademicTerm)))
            {
                Console.WriteLine("2. NewAcademicTerm model invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                await LoadTermsAsync();
                return Page();
            }

            Console.WriteLine($"3. Term info: Name={NewAcademicTerm.Name}, Start={NewAcademicTerm.StartDate}, End={NewAcademicTerm.EndDate}, Active={NewAcademicTerm.IsActive}");

            try
            {
                // Create a new AcademicTerm entity from the input model
                var academicTerm = new AcademicTerm
                {
                    Name = NewAcademicTerm.Name,
                    StartDate = NewAcademicTerm.StartDate,
                    EndDate = NewAcademicTerm.EndDate,
                    IsActive = NewAcademicTerm.IsActive,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedById = 1 // TODO: Replace with actual user ID
                };

                Console.WriteLine("4. Looking up user");

                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                {
                    Console.WriteLine("5. Invalid user ID in claims");
                    ModelState.AddModelError(string.Empty, "Invalid user session.");
                    await LoadTermsAsync();
                    return Page();
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    Console.WriteLine("5. User not found");
                    ModelState.AddModelError(string.Empty, "User not found");
                    await LoadTermsAsync();
                    return Page();
                }

                academicTerm.CreatedBy = user;

                Console.WriteLine("6. Adding term to context");
                _context.AcademicTerms.Add(academicTerm);

                Console.WriteLine("7. Saving changes");
                await _context.SaveChangesAsync();

                Console.WriteLine("8. Success! Redirecting");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"STACK TRACE: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"INNER EXCEPTION: {ex.InnerException.Message}");
                }

                ModelState.AddModelError(string.Empty, $"Error saving: {ex.Message}");
                await LoadTermsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteTermAsync(int id)
        {
            var hasReservations = await _context.ClassroomReservations.AnyAsync(r => r.AcademicTermId == id);
            if (hasReservations)
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this term because it has assigned classes.");
                await LoadTermsAsync();
                return Page();
            }

            var term = await _context.AcademicTerms.FindAsync(id);
            if (term != null)
            {
                _context.AcademicTerms.Remove(term);
                await _context.SaveChangesAsync();
            }

            await LoadTermsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEditTermAsync()
        {
            // Only validate the EditAcademicTerm property
            ModelState.Clear();
            ModelState.ClearValidationState(nameof(EditAcademicTerm));
            if (!TryValidateModel(EditAcademicTerm, nameof(EditAcademicTerm)))
            {
                await LoadTermsAsync();
                return Page();
            }

            var term = await _context.AcademicTerms.FindAsync(EditAcademicTerm.Id);
            if (term == null)
            {
                ModelState.AddModelError(string.Empty, "Term not found.");
                await LoadTermsAsync();
                return Page();
            }

            term.Name = EditAcademicTerm.Name;
            term.StartDate = EditAcademicTerm.StartDate;
            term.EndDate = EditAcademicTerm.EndDate;
            term.IsActive = EditAcademicTerm.IsActive;
            term.UpdatedAt = DateTime.Now;




            await _context.SaveChangesAsync();
            await LoadTermsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            // Prevent validation of unrelated properties
            ModelState.Clear();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                var terms = await _context.AcademicTerms
                    .Where(t => t.Name.Contains(SearchKeyword))
                    .OrderByDescending(t => t.StartDate)
                    .ToListAsync();

                var termIds = terms.Select(t => t.Id).ToList();
                var usedTermIds = await _context.ClassroomReservations
                    .Where(r => termIds.Contains(r.AcademicTermId))
                    .Select(r => r.AcademicTermId)
                    .Distinct()
                    .ToListAsync();

                AcademicTerms = terms.Select(t => new AcademicTermViewModel
                {
                    Term = t,
                    HasReservations = usedTermIds.Contains(t.Id)
                }).ToList();
            }
            else
            {
                await LoadTermsAsync();
            }

            return Page();
        }

        private async Task LoadTermsAsync()
        {
            var terms = await _context.AcademicTerms
                .OrderByDescending(t => t.StartDate)
                .ToListAsync();

            var termIds = terms.Select(t => t.Id).ToList();
            var usedTermIds = await _context.ClassroomReservations
                .Where(r => termIds.Contains(r.AcademicTermId))
                .Select(r => r.AcademicTermId)
                .Distinct()
                .ToListAsync();

            AcademicTerms = terms.Select(t => new AcademicTermViewModel
            {
                Term = t,
                HasReservations = usedTermIds.Contains(t.Id)
            }).ToList();
        }
    }
}