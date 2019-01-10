using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using HRSystem.Models;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Data.Entity;

namespace HRSystem.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        NubiDBEntities db = new NubiDBEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
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
            bool IsDeleted = db.AspNetUsers.Where( y => y.Email == model.Email && y.IsDeleted == true).Any();
            if(IsDeleted)
            {
                ModelState.AddModelError("", "This account has been removed");
                return View(model);
            }
            else
            {
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
            
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View(Resources.NubiHR.Error);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        [Authorize(Roles ="Admin")]
        public ActionResult Register()
        {

            // ViewBag.EmpNo = new SelectList( db.Employees.ToList(), "Id", "FirstName");
             ViewBag.EmpNo = db.Employees.Where(x => x.HasAccount == false && x.IsDeleted == false).OrderBy(x => x.FirstName + x.LastName).Select(x => x.FirstName + " " + x.LastName).ToList();

            //ViewBag.JobTitleRoleNo = new SelectList( db.JobTitleRoles.ToList(), "ID", "JobTitle");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var emp = db.Employees.Where(x => x.FirstName + " " + x.LastName == model.EmpNo && x.IsDeleted == false).FirstOrDefault();
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmpNo = emp.Id };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var e = db.Employees.Find(emp.Id);
                    e.HasAccount = true;
                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    //UserManager.AddToRole(user.Id, model.RoleNo);
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    TempData["chec"] = string.Format(Resources.NubiHR.AccountHasBeenCreatedSuccessfully, "Index");
                    return RedirectToAction("Index", "VacationRequest");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form


            //if error 
            //ViewBag.EmpNo = new SelectList(db.Employees.ToList(), "Id", "FirstName");
            ViewBag.EmpNo = db.Employees.Where(x => x.HasAccount == false && x.IsDeleted == false).OrderBy(x => x.FirstName + x.LastName).Select(x => x.FirstName + " " + x.LastName).ToList();

            //ViewBag.JobTitleRoleNo = new SelectList(db.JobTitleRoles.ToList(), "ID", "JobTitle");
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View(Resources.NubiHR.Error);
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : Resources.NubiHR.Error);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var IsArabic = Request.Cookies["culture"].Value == "ar" ? true : false;
            if (ModelState.IsValid)
            {
                var UserEmail = db.AspNetUsers.Where(x => x.Email == model.Email).Select(x => x.Email).FirstOrDefault();

                if (UserEmail != null)
                {
                    var ID = db.AspNetUsers.Where(x => x.Email == model.Email).Select(x => x.Id).FirstOrDefault();
                    var EmpNo = db.AspNetUsers.Where(x => x.Id == ID).Select(y => y.EmpNo).FirstOrDefault();
                    var EmployeeName = db.Employees.Where(x => x.Id == EmpNo).Select(y => y.FirstName + " " + y.LastName).FirstOrDefault();
                    // Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(ID);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = ID, code = code }, protocol: Request.Url.Scheme);
                    
                    Alert _Alert = new Alert();

                    _Alert.Email = UserEmail;
                    //_Alert.MobilePhone = Researcher.MobilePhone;

                    var MsgEmailText = "Dear "+ EmployeeName + ", Please reset your password by clicking <a href=\"" + callbackUrl + "\">Here</a>";

                    //var MsgSmsText = ResearcherTemplate.MsgSmsText;
                    // MsgSmsText = MsgSmsText.Replace("{{test}}", test);


                    _Alert.EmailSubject = "Reset Password";
                    _Alert.MsgEmailText = MsgEmailText;
                    // _Alert.MsgSmsText = MsgSmsText;
                    _Alert.UserId = ID;
                    _Alert.Link = callbackUrl;
                    _Alert.MobilePhone = db.Employees.Where(x => x.Id == EmpNo).Select(y => y.Mobile1).FirstOrDefault();

                    var AlertId = Create(_Alert);
                    if (AlertId > 0)
                    {
                        HRSystem.Manager.Helper.SendEmail(AlertId);
                       
                        //var IsSendEmail = db.Alerts.Find(AlertId);
                        var IsSendEmail = HRSystem.Manager.Helper.FunSendEmail(_Alert.MsgEmailText, _Alert.Email, _Alert.EmailSubject, true, _Alert.AttachmentPath);
                        if (IsSendEmail == true)
                        {
                            _Alert.IsSendEmail = true;
                            _Alert.DateSendEmail = DateTime.Now;
                            db.Entry(_Alert).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            TempData["chec"] = Resources.NubiHR.ForgetPasswordConfirmationMsg;
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            TempData["check"] = Resources.NubiHR.FailedSendingForgetPasswordConfirmationMsg;
                            
                            return RedirectToAction("Login", "Account");
                        }

                    }

                }
                else
                {
                    TempData["check"] = Resources.NubiHR.EmailValidation;
                    return View("ForgotPassword");
                }

                /*
                 * var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                    // Don't reveal that the user does not exist or is not confirmed
                   // return View("ForgotPasswordConfirmation");

                }
                */

            }


            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public static int Create(Alert alert)
        {
            using (NubiDBEntities db = new NubiDBEntities())
            {
                alert.CreationDate = DateTime.Now;
                db.Alerts.Add(alert);
                db.SaveChanges();
                return alert.Id;
            }
            //  return 0;
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string userid)
        {
            ViewBag.Email = db.AspNetUsers.Where(x => x.Id == userid).Select(y => y.Email).FirstOrDefault();
            
            return code == null ? View("Login") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model, string email)
        {
            
            model.Email = email;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            
            if (user == null)
            {
                // Don't reveal that the user does not exist
                TempData["check"] = Resources.NubiHR.EmailValidation;
                return View(model);
            }
            var alert = db.Alerts.Where(x => x.UserId == user.Id).OrderByDescending(z => z.Id).FirstOrDefault();
            if (alert.IsOpen == true)
            {
                TempData["check"] = Resources.NubiHR.YouHaveAlreadyOpenedThisLink;
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    alert.IsOpen = true;
                    db.Entry(alert).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["chec"] = Resources.NubiHR.PasswordHasBeenResetSuccessfully;
                    //AddErrors(result);
                    return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
            }
            ViewBag.Email = db.AspNetUsers.Where(x => x.Id == user.Id).Select(y => y.Email).FirstOrDefault();
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View(Resources.NubiHR.Error);
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View(Resources.NubiHR.Error);
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpGet]
       // [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
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
            return RedirectToAction("Index", "VacationRequest");
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