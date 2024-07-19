using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Login(string returnUrl=null)
        {
            returnUrl ??= Url.Content("~/");
            LoginViewModel loginViewModel = new()
            {
                RedirectUrl = returnUrl,
            };
            return View(loginViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure:false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginViewModel.RedirectUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }   
                    else
                    {
                        return LocalRedirect(loginViewModel.RedirectUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login atteempt");
                }
            }
            return View(loginViewModel);
        }

        public IActionResult Register()
        {
            if (!roleManager.RoleExistsAsync(SD.Admin).GetAwaiter().GetResult()) {
                roleManager.CreateAsync(new IdentityRole(SD.Admin)).Wait();
                roleManager.CreateAsync(new IdentityRole(SD.Customer)).Wait();
            }
            RegisterViewModel registerViewModel = new()
            {
                RolesList = roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                })
            };
            return View(registerViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel) {

            ApplicationUser user = new() { 
                Name = registerViewModel.Name,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.PhoneNumber,
                NormalizedEmail=registerViewModel.Email.ToUpper(),
                UserName = registerViewModel.Email,
                EmailConfirmed=true,
                //CreatedAt = DateTime.Now,
            };

            var result = await userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded) {
                if (!string.IsNullOrEmpty(registerViewModel.Role)) { 
                    await userManager.AddToRoleAsync(user, registerViewModel.Role);
                    TempData["success"] = "User registered successfully";
                }
                else
                {
                    await userManager.AddToRoleAsync(user, SD.Customer);
                    TempData["success"] = "User registered successfully";
                }
                await signInManager.SignInAsync(user, isPersistent: false);
                if (string.IsNullOrEmpty(registerViewModel.RedirectUrl)) {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return LocalRedirect(registerViewModel.RedirectUrl);
                }
            }
            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error.Description);
            };

            registerViewModel.RolesList = roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });
            return View(registerViewModel);

        }

    } 
}
