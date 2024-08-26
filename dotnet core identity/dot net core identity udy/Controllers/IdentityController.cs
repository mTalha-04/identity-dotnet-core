using dot_net_core_identity_udy.Models;
using dot_net_core_identity_udy.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dot_net_core_identity_udy.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        public IActionResult Signup()
        {
            var model = new SignupViewModel() { Role = "Member" };
            return View(model);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists
                if (await _userManager.FindByEmailAsync(model.Email) == null)
                {
                    var user = new IdentityUser
                    {
                        Email = model.Email,
                        UserName = model.Email,
                    };
                    var userResult = await _userManager.CreateAsync(user, model.Password);

                    if (userResult.Succeeded)
                    {
                        // Check if the role exists, and create it if not
                        if (!await _roleManager.RoleExistsAsync(model.Role))
                        {
                            var role = new IdentityRole { Name = model.Role };
                            var roleResult = await _roleManager.CreateAsync(role);
                            if (!roleResult.Succeeded)
                            {
                                var errors = roleResult.Errors.Select(s => s.Description);
                                ModelState.AddModelError("Role", string.Join(",", errors));
                                return View(model);
                            }
                        }

                        // Assign the role to the user
                        var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
                        if (!addRoleResult.Succeeded)
                        {
                            var errors = addRoleResult.Errors.Select(e => e.Description);
                            ModelState.AddModelError("RoleAssignment", string.Join(",", errors));
                            return View(model);
                        }

                        // Generate email confirmation token and send confirmation email
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Identity", new { userId = user.Id, token }, Request.Scheme);
                        await _emailSender.SendEmailAsync("info@gmail.com", user.Email, "Confirm your Email Address!", confirmationLink);

                        return RedirectToAction("Signin");
                    }

                    // If user creation fails, add errors to ModelState
                    ModelState.AddModelError("Signup", string.Join(",", userResult.Errors.Select(e => e.Description)));
                }
                else
                {
                    ModelState.AddModelError("Signup", "A user with this email already exists.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("Signin");
            }

            return new BadRequestResult();
        }

        public IActionResult Signin()
        {
            return View(new SigninViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Signin(SigninViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("Login", "Cannot Login!!");
            }

            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
