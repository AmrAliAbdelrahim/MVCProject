using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Razor.Language;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    
    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
			 _userManager = userManager;
			_signInManager = signInManager;
		}
        
        #region Register
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)//server side Validation
            {
                var user = new ApplicationUser()
                {
                    UserName = model.Email.Split('@')[0],
					Email = model.Email,
					FName = model.FName,
                    LName=model.LName,
                    IAgree=model.IAgree
				};
                var Result = await _userManager.CreateAsync(user, model.Password);
                if (Result.Succeeded)
                    return RedirectToAction(nameof(Login));
                else
                    foreach(var error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
            }
            return View(model);
			// password P@ssword1
		}

        #endregion
        #region Login
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)//Server Side Validation
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var flag = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (flag)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Incorrect Password");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email is not Existed");
                }
            }
            return View(model);
        }
        #endregion
        #region SingOut
        public async Task<IActionResult> SingOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        } 
        #endregion
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token =await _userManager.GeneratePasswordResetTokenAsync(user);
                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = user.Email, Token=token }, Request.Scheme);


                    var email = new Email()
                    {
                        Subject = "Reset You Password",
                        To = model.Email,
                        Body = ResetPasswordLink
					};
                    EmailSettings.SendEmail(email);
                    return RedirectToAction("CheckYourInbox");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email");
                }
            }
            return View(nameof(ForgetPassword),model);
        }
        public IActionResult CheckYourInbox()
        {
            return View();
        }

        public IActionResult ResetPassword(string email,string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model )
        {  
            if(ModelState.IsValid)
            {
                string emai = TempData["email"] as string;
                string token = TempData["token"] as string;
                var User = await _userManager.FindByEmailAsync(emai);
          var result=     await _userManager.ResetPasswordAsync(User, token, model.NewPassword);
                if(result .Succeeded)
                    return RedirectToAction(nameof(Login));
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }


	}
}
