using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using TAClassifieds.Data;
using TAClassifieds.Model;
using TAClassifieds.Models;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using TAClassifieds.BAL;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using System.Threading;
using Google.Apis.Oauth2.v2;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Oauth2.v2.Data;
using System.Security.Principal;
using System.Configuration;
using System.Net;
using System.IO;
using Facebook;
using Newtonsoft.Json;

namespace TAClassifieds.Controllers
{
    public class jsonProfile
    {
        public string name { get; set; }
        public string id { get; set; }
        public string email { get; set; }
    }

    public class AccountController : Controller
    {

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
           
            //comment added123sdfsdf
            //comment from test
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

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
        public ActionResult Login(User model, string returnUrl, bool Rememberme = false)
        {
            if (model.Email != null && model.UPassword != null && Rememberme != false)
            {
                //var user = await UserManager.FindAsync(model.Email, model.UPassword);
                AccountBL userverification = new AccountBL();
                if (userverification.UserVerification(model))
                {
                    AccountBL loggedinuser = new AccountBL();
                    bool status = loggedinuser.UserProfileStatus(model.Email);
                    if (status)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        // ViewBag.UpdationMessage = "Please update your profile to proceed further";
                        return RedirectToAction("UpdateProfile", "Account", new { email = model.Email });
                    }
                }
                else
                {
                    ViewBag.ErrorMsg = "Invalid credentials";
                }

                //if (user != null)
                //{
                //    await SignInAsync(user, model.RememberMe);
                //AccountBL loggedinuser = new AccountBL();
                //bool status = loggedinuser.UserProfileStatus(model.Email);
                //if (status)
                //{
                //    return RedirectToLocal(returnUrl);
                //}
                //else
                //{
                //    ViewBag.UpdationMessage = "Please update your profile to proceed further";
                //    return RedirectToAction("UpdateProfile",model.Email);
                //}
                //    return RedirectToLocal(returnUrl);
                //}

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(User model, string ConfirmPassword, bool Terms = false)
        {
            if (model.Email != null && model.UPassword != null && Terms != false)
            {
                if (model.UPassword.Equals(ConfirmPassword))
                {
                    AccountBL newuser = new AccountBL();
                    bool status = newuser.UserRegistration(model);
                    if (status)
                    {
                        ViewBag.Email = "Confirmation mail has been sent to your given Email.";
                        return View();
                    }
                    else
                    {
                        ViewBag.UserExists = "Email Id is already registered.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.PwdMatch = "Password and Repeat Password doesnt match.";
                    return View(model);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Confirmation()
        {
            Guid UserId = Guid.Parse(Request.QueryString["id"]);
            AccountBL confirmeduser = new AccountBL();
            confirmeduser.Confirmation(UserId);
            return RedirectToAction("Login", "Account");
        }
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]

        [HttpGet]
        public ActionResult UpdateProfile(String email)
        {
            ViewData["email"] = email;
            return View();
        }


        [HttpPost]
        public ActionResult UpdateProfile(User profile, string email)
        {
            profile.Email = email;
            AccountBL updateuser = new AccountBL();
            updateuser.UpdateProfile(profile);
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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
        // GET: /Account/ExternalLoginCallback
        
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            if (result == null || result.Identity == null)
            {
                return RedirectToAction("Login");
            }

            var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                return RedirectToAction("Login");
            }

            var login = new UserLoginInfo(idClaim.Issuer, idClaim.Value);
            var name = result.Identity.Name == null ? "" : result.Identity.Name.Replace(" ", "");

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = name });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
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
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public ActionResult gplusLogin()
        {
            User userDetails = new User();
            ClientSecrets secrets = new ClientSecrets()
            {
                ClientId = ConfigurationManager.AppSettings["gplus-client-id"],
                ClientSecret = ConfigurationManager.AppSettings["gplus-client-secret"]
            };
            string[] SCOPES = { PlusService.Scope.PlusLogin, PlusService.Scope.UserinfoEmail };
            TokenResponse token;
            PlusService ps = null;

            IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = SCOPES
            });

            if (Request["code"] == null)
            {
                string url =
                    "https://accounts.google.com/o/oauth2/auth?redirect_uri=http://localhost:57864/Account/gplusLogin&response_type=code&scope=https://www.googleapis.com/auth/userinfo.email&openid.realm=&client_id=465360611509-vamhmig2ki8ba7t10ljq7c3p8bq7l06f.apps.googleusercontent.com&access_type=offline&approval_prompt=force";
                Response.Redirect(url);
            }
            else
            {
                token = flow.ExchangeCodeForTokenAsync("", Request["code"], "http://localhost:57864/Account/gplusLogin",
                          CancellationToken.None).Result;
                // Get tokeninfo for the access token if you want to verify.
                Oauth2Service service = new Oauth2Service(
                    new Google.Apis.Services.BaseClientService.Initializer());
                Oauth2Service.TokeninfoRequest request = service.Tokeninfo();
                request.AccessToken = token.AccessToken;

                Tokeninfo info = request.Execute();
                string gplus_id = info.UserId;

                UserCredential gplusUserCredential = new UserCredential(flow, "me", token);
                ps = new PlusService(
                    new Google.Apis.Services.BaseClientService.Initializer()
                    {
                        ApplicationName = "TA-Classifieds",
                        HttpClientInitializer = gplusUserCredential
                    });

                Person some = ps.People.Get("me").Execute();

                userDetails.First_Name = some.Name.GivenName;
                userDetails.Last_Name = some.Name.FamilyName;
                userDetails.Email = some.Emails[0].Value;

                User userModel = new Model.User();
                userModel.Email = some.Emails[0].Value;
                userModel.UPassword = "Dummy Password";
                AccountBL bl = new BAL.AccountBL();
                bl.UserRegistration(userModel);

            }

            return RedirectToAction("UpdateProfile", "Account", new { email = userDetails.Email, firstName = userDetails.First_Name, lastName = userDetails.Last_Name });
        }

        
        [AllowAnonymous]
        public ActionResult fbLogin()
        {

            if (Request["code"] != null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string url =
                    string.Format(
                        "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri=http://localhost:49540/Account/fbLogin&scope={1}&code={2}&client_secret={3}",
                        ConfigurationManager.AppSettings["fb-app-id"],
                        ConfigurationManager.AppSettings["fb-scope"],
                        Request["code"].ToString(),
                        ConfigurationManager.AppSettings["fb-app-secret"]);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string vals = reader.ReadToEnd();

                    foreach (var token in vals.Split('&'))
                    {
                        dic.Add(token.Substring(0, token.IndexOf("=")), token.Substring(token.IndexOf("=") + 1, token.Length - token.IndexOf("=") - 1));
                    }
                    string access_token = dic["access_token"];
                    var client = new FacebookClient(access_token);
                    var obj = client.Get("/me");
                    jsonProfile user = JsonConvert.DeserializeObject<jsonProfile>(obj.ToString());
                    var profClient = new FacebookClient(access_token);
                    var profDetails = profClient.Get("/me?fields=email");
                    jsonProfile email = JsonConvert.DeserializeObject<jsonProfile>(profDetails.ToString());
                    var name = user.name;
                    var Email = email.email;

                    User userModel = new Model.User();
                    userModel.Email = Email;
                    userModel.UPassword = "Dummy Password";
                    AccountBL bl = new BAL.AccountBL();
                    bl.UserRegistration(userModel); 

                    return RedirectToAction("UpdateProfile", "Account", new { email = Email, firstName = name,lastName=""});
                }
            }

            Response.Redirect(string.Format("https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
            ConfigurationManager.AppSettings["fb-app-id"], Request.Url.AbsoluteUri, ConfigurationManager.AppSettings["fb-scope"]));

            return RedirectToAction("login", "Account");
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

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
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
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
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
