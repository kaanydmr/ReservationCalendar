using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Pages.Instructor
{
    [Authorize(Roles = "Admin,Instructor")]
    public class IndexModel : PageModel
    {
        public void OnGet() { }
    }
}