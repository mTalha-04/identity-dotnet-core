using dot_net_core_identity_udy.Models;
using dot_net_core_identity_udy.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dot_net_core_identity_udy.Controllers
{

    public class IdentityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        public IdentityController(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Signup()
        {
            var model = new SignupViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                if ((await _userManager.FindByEmailAsync(model.Email) == null))
                {
                    var user = new IdentityUser
                    {
                        Email = model.Email,
                        UserName = model.Email,
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    user = await _userManager.FindByEmailAsync(model.Email);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    if (result.Succeeded)
                    {
                        var conformatinLink = Url.ActionLink("ConfirmEmail", "Identity", new { userId = user.Id, @token = token });
                        await _emailSender.SendEmailAsync("mrtalha207a@gmail.com", user.Email,"Confirm your Email Address!", conformatinLink);
                        return RedirectToAction("Signin");
                    }
                    ModelState.AddModelError("Signup", string.Join("", result.Errors.Select(x => x.Description)));
                    return View(model);
                }
                ModelState.AddModelError("Signup", "A user with this email already exists.");
            }
            return View(model);
        }
    
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = _userManager.FindByIdAsync(userId);
            var result = await _userManager.ConfirmEmailAsync(await user, token);
            if (result.Succeeded)
            {
                return RedirectToAction("Signin");
            }
            return new NotFoundResult();
        }
        public async Task<IActionResult> Signin()
        {
            return View();
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    }
}
