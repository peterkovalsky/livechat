﻿using Hangfire;
using Kookaburra.Models;
using Kookaburra.Models.Account;
using Kookaburra.Services;
using Kookaburra.Services.Accounts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly IAccountService _accountService;


        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        [Route("profile")]
        public async Task<ActionResult> UserProfile()
        {
            var profile = await _accountService.GetOperatorAsync(User.Identity.GetUserId());

            var model = new ProfileViewModel
            {
                UserDetails = new UserDetailsViewModel
                {
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Email = profile.Email
                }
            };

            if (TempData["ProfileUpdated"] != null)
            {
                model.Alert = new AlertViewModel
                {
                    IsSuccess = true,
                    SuccessMessage = TempData["ProfileUpdated"].ToString()
                };
            }

            return View(model);
        }

        [HttpPost]
        [Route("profile")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfile(UserDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var viewModel = new ProfileViewModel
            {
                UserDetails = model,
            };

            // update asp.net Identity user
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            user.Email = model.Email;
            user.UserName = model.Email;

            var updateResult = await UserManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                // update Operator
                await _accountService.UpdateProfileAsync(User.Identity.GetUserId(), model.FirstName, model.LastName, model.Email);

                TempData["ProfileUpdated"] = "Your details were updated successfully.";
                return RedirectToAction("UserProfile");
            }
            else
            {
                viewModel.Alert = new AlertViewModel
                {
                    IsSuccess = false,
                    Errors = updateResult.Errors.Where(e => !(e.Contains("Name") && e.Contains("is already taken."))).ToList()
                };
                return View(viewModel);
            }
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [HttpGet, Route("reset-password")]
        public ActionResult ResetPassword(string code, string email)
        {
            return code == null 
                ? View("Error") 
                : View(new ResetPasswordViewModel { Code = HttpUtility.UrlDecode(code), Email = email });
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost, Route("reset-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetUserPassword(ResetUserPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserProfile");
            }

            var token = await UserManager.GeneratePasswordResetTokenAsync(User.Identity.GetUserId());
            var result = await UserManager.ResetPasswordAsync(User.Identity.GetUserId(), token, model.Password);
            if (result.Succeeded)
            {
                TempData["ProfileUpdated"] = "Your password was updated successfully.";
                return RedirectToAction("UserProfile");
            }
            else
            {
                var profile = await _accountService.GetOperatorAsync(User.Identity.GetUserId());

                return View("UserProfile", new ProfileViewModel
                {
                    UserDetails = new UserDetailsViewModel
                    {
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        Email = profile.Email
                    },
                    Alert = new AlertViewModel
                    {
                        IsSuccess = false,
                        Errors = result.Errors.ToList()
                    }
                });
            }
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        [Route("sign-up")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("sign-up")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _accountService.SignUpAsync(new SignUpRequest
                    {
                        ClientName = model.ClientName,
                        Email = model.Email,
                        Website = model.Website,
                        OperatorIdentity = user.Id,
                        TrialPeriodDays = AppSettings.TrialPeriodDays
                    });

                    BackgroundJob.Enqueue<IEmailService>(email => email.SendSignUpWelcomeEmailAsync(user.Id));

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return Redirect("/installation");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        [Route("forgot-password")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var encodedToken = HttpUtility.UrlEncode(code);

                BackgroundJob.Enqueue<IEmailService>(email => email.SendForgorPasswordEmail(user.Email, encodedToken, AppSettings.UrlPortal));

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            await _accountService.ResetOperatorActivityAsync(User.Identity.GetUserId());

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return Redirect("/");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            if (result.Errors.Any(e => e.Contains("is already taken")))
            {
                ModelState.AddModelError("", "Email is already taken");
                return;
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}