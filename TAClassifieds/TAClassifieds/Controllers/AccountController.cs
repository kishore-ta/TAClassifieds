using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using System.Security.Principal;
using System.Configuration;
using System.Net;
using System.IO;
using Facebook;
using Newtonsoft.Json;
using System.Web.Security;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using TAClassifieds.BAL;
using TAClassifieds.Model;
using TAClassifieds.Models;

namespace TAClassifieds.Controllers
{
    public class AccountController : Controller
    {
        public string UserNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

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
            if(Request.IsAuthenticated)
            { ViewBag.error = "You are already Logged In.!"; }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(User model, string returnUrl, bool Rememberme = false)
        {
            if (model.Email != null && model.UPassword != null)
            {
                ModelState.Clear();
                //var user = await UserManager.FindAsync(model.Email, model.UPassword);
                AccountBL userverification = new AccountBL();
                User usermodel = userverification.UserVerification(model);
                ApplicationUser appUser = addClaims(usermodel.Email, usermodel.UserId.ToString(), usermodel.First_Name!=null?usermodel.First_Name:usermodel.Email);
                //ApplicationUser appUser = new ApplicationUser();
                //appUser.UserName = usermodel.First_Name != null ? usermodel.First_Name : string.Empty;
                //appUser.Id = usermodel.UserId.ToString();
                //appUser.SecurityStamp = Guid.NewGuid().ToString();
                //appUser.Claims.Add(new IdentityUserClaim() { ClaimType = "email", ClaimValue = usermodel.Email });
                await SignInAsync(appUser, Rememberme);

                if (usermodel != null)
                {

                    if (TryValidateModel(usermodel))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                            
                    else
                    {
                        return RedirectToAction("UpdateProfile", "Account");
                    }
                }
                else
                {
                    ViewBag.ErrorMsg = "Invalid credentials";
                }
            }
              
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
            //ModelState.Clear();
            if (model.Email != null && model.UPassword != null && Terms != false)
            {
                if (model.UPassword.Equals(ConfirmPassword))
                {
                    AccountBL newuser = new AccountBL();
                    bool status = newuser.UserRegistration(model);
                    if (status)
                    {
                        newuser.Registration(model,false);
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
            //var identity = (ClaimsIdentity)User.Identity;
            //IEnumerable<Claim> claims = identity.Claims;
            //var guid = claims.Where(m => m.Type == "guid");
            //var UserId = guid.FirstOrDefault().Value;
            Guid TokenId = Guid.Parse(Request.QueryString["id"]);
            AccountBL confirmeduser = new AccountBL();
            if (confirmeduser.Confirmation(TokenId))
            {
                ViewBag.success = "Account Activated.";
                return View();
            }
            else
            {
                ViewBag.error = "Activation Link Expired. Please try again";
                return View();
            }
        }
        
        [HttpGet]
        public ActionResult UpdateProfile()
        {
            //AccountBL updateprofile = new AccountBL();
            //Guid userId = Guid.Parse(Session["UserId"].ToString());
            //User user=updateprofile.FetchUserInfo(userId);

            String Userguid = getClaims("email");
            //var identity = (ClaimsIdentity)User.Identity;
            //IEnumerable<Claim> claims = identity.Claims;
            //var userguidArray = claims.Where(m => m.Type == "email");
            //Userguid = userguidArray.FirstOrDefault().Value;
            AccountBL obj = new AccountBL();
            User resUser = obj.FetchUser(Userguid);
            return View(resUser);
        }
        
        [HttpPost]
        public ActionResult UpdateProfile(User profile)
        {
            //Guid userId = Guid.Parse(Session["UserId"].ToString());
            //profile.UserId = userId;
            String Userguid = getClaims("guid");
            //var identity = (ClaimsIdentity)User.Identity;
            //IEnumerable<Claim> claims = identity.Claims;
            //var userguidArray = claims.Where(m => m.Type == "guid");
            //Userguid = userguidArray.FirstOrDefault().Value;
            profile.UserId = Guid.Parse(Userguid);
            AccountBL updateuser = new AccountBL();
            updateuser.UpdateProfile(profile);
            //update claim
            if (!string.IsNullOrEmpty(profile.First_Name))
            {
                AddUpdateClaim(UserNameClaimType,profile.First_Name);
            }
            return RedirectToAction("GetAd", "Home");
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


        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("GetAd", "Home");
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
        public async Task<ActionResult> gplusLogin()
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
                    new BaseClientService.Initializer());
                Oauth2Service.TokeninfoRequest request = service.Tokeninfo();
                request.AccessToken = token.AccessToken;

                Tokeninfo info = request.Execute();
                string gplus_id = info.UserId;

                UserCredential gplusUserCredential = new UserCredential(flow, "me", token);
                ps = new PlusService(
                    new BaseClientService.Initializer()
                    {
                        ApplicationName = "TA-Classifieds",
                        HttpClientInitializer = gplusUserCredential
                    });

                Person some = ps.People.Get("me").Execute();

                userDetails.First_Name = some.Name.GivenName;
                userDetails.Last_Name = some.Name.FamilyName;
                userDetails.Email = some.Emails[0].Value;

                User userModel = new User();
                userModel.IsActive = true;
                userModel.IsVerified = true;
                userModel.Email = some.Emails[0].Value;
                userModel.UPassword = "Dummy Password";
                AccountBL bl = new AccountBL();
                if (bl.UserRegistration(userModel))
                {
                    bl.Registration(userModel, true);
                }

                AccountBL loggedinuser = new AccountBL();
                User resUser = loggedinuser.FetchUser(userModel.Email);
                ApplicationUser appUser = addClaims(some.Emails[0].Value,resUser.UserId.ToString(),resUser.First_Name);
                //appUser.Claims.Add(new IdentityUserClaim() { ClaimType = "guid", ClaimValue = resUser.UserId.ToString() });
                //appUser.UserName = some.Name.GivenName != null ? some.Name.GivenName : resUser.Email;
                //appUser.SecurityStamp = Guid.NewGuid().ToString();
                //appUser.Claims.Add(new IdentityUserClaim() { ClaimType = "email", ClaimValue = some.Emails[0].Value });
                
                await SignInAsync(appUser, false);
                if (TryValidateModel(resUser))
                {
                    return RedirectToAction("GetAd", "Home");
                }
            }

            return RedirectToAction("UpdateProfile", "Account");
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

                    User userModel = new User();
                    userModel.Email = Email;
                    userModel.UPassword = "Dummy Password";
                    AccountBL bl = new AccountBL();
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

        private string getClaims(string claimType)
        {
            string returnClaimValue = string.Empty;
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            switch (claimType)
            {
                case "email":  var resClaimArray = claims.Where(m => m.Type == "email");
                               returnClaimValue = resClaimArray.FirstOrDefault().Value;
                               break;
                case "guid":  resClaimArray = claims.Where(m => m.Type == "guid");
                              returnClaimValue = resClaimArray.FirstOrDefault().Value;
                              break;
                default:      returnClaimValue= string.Empty;
                              break;
            }

            return returnClaimValue;
        }

        private ApplicationUser addClaims(string email,string guid,string username)
        {
            ApplicationUser resUser = new ApplicationUser();
            resUser.UserName = username;
            resUser.SecurityStamp = Guid.NewGuid().ToString();
            resUser.Claims.Add(new IdentityUserClaim() { ClaimType = "email", ClaimValue = email });
            resUser.Claims.Add(new IdentityUserClaim() { ClaimType = "guid", ClaimValue = guid });
            return resUser;
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public  void AddUpdateClaim(string key, string value)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
            //var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
        }
        private async Task  SignInAsync(ApplicationUser user, bool isPersistent)
        {
           
            string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            string UserIdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            
            var emailUser = user.Claims.Where(m=>m.ClaimType=="email").FirstOrDefault().ClaimValue!=null?user.Claims.Where(m=>m.ClaimType=="email").FirstOrDefault().ClaimValue:string.Empty;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, UserNameClaimType,RoleClaimType);
            identity.AddClaim(new Claim(UserIdClaimType, user.Id, "http://www.w3.org/2001/XMLSchema#string"));
            identity.AddClaim(new Claim(UserNameClaimType, user.UserName!=null?user.UserName:emailUser, "http://www.w3.org/2001/XMLSchema#string"));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"));

            foreach (var item in user.Claims)
            {
                identity.AddClaim(new Claim(item.ClaimType, item.ClaimValue));
            }
            var prop = new AuthenticationProperties();
            prop.IsPersistent = isPersistent;
            if (isPersistent)
            {
                prop.ExpiresUtc = DateTime.Now.AddHours(5);
            }
            AuthenticationManager.SignIn(prop, identity);
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
                return RedirectToAction("GetAd", "Home");
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
