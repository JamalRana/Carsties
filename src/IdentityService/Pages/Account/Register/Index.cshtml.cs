using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public Index(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; }
        [BindProperty]
        public bool RegisterSucceeded { get; set; }


        public IActionResult OnGet(string returnurl)
        {
            Input = new RegisterViewModel
            {
                ReturnUrl = returnurl
            };

            return Page();

        }

        public async Task<IActionResult> OnPost()
        {
            if (Input.Button != "register") return Redirect("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = Input.Email,
                    UserName = Input.Username,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddClaimsAsync(user, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, Input.Fullname)
                    });
                    RegisterSucceeded = true;
                }

            }

            return Page();

        }


    }
}
