//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Auth.OAuth2.Flows;
//using Google.Apis.Auth.OAuth2.Responses;
//using Google.Apis.Plus.v1;
//using Newtonsoft.Json.Linq;
//using Google.Apis.Oauth2;
//using Google.Apis.Oauth2.v2;
//using Google.Apis.Oauth2.v2.Data;
//using Google.Apis.Plus.v1.Data;
//using System.Net;
//using System.IO;
//using System.Web.Mvc;
//using System.Web.Security;
//using TAClassifieds.Data;
////using TAClassifieds.Model;


//namespace TAClassifieds.Controllers
//{
    
//    public class socialLoginController : Controller
//    {
//        public ActionResult gplusLogin()
//        {
//            User userDetails = new User();
//            ClientSecrets secrets = new ClientSecrets() {
//                ClientId = "465360611509-vamhmig2ki8ba7t10ljq7c3p8bq7l06f.apps.googleusercontent.com",
//                ClientSecret = "1-kfNNOJCbza3gdreFwLWbUs"
//            };
//            string[] SCOPES = { PlusService.Scope.PlusLogin, PlusService.Scope.UserinfoEmail };
//            TokenResponse token;
//            PlusService ps = null;

//            IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer { 
//                ClientSecrets=secrets,
//                Scopes = SCOPES
//            });

//            if (Request["code"] == null)
//            {
//                string url =
//                    "https://accounts.google.com/o/oauth2/auth?redirect_uri=http://localhost:4567/socialLogin/gplusLogin&response_type=code&scope=https://www.googleapis.com/auth/userinfo.email&openid.realm=&client_id=465360611509-vamhmig2ki8ba7t10ljq7c3p8bq7l06f.apps.googleusercontent.com&access_type=offline&approval_prompt=force";
//                Response.Redirect(url);
//            }
//            else
//            {
//                token = flow.ExchangeCodeForTokenAsync("", Request["code"], "http://localhost:4567/socialLogin/gplusLogin",
//                          CancellationToken.None).Result;
//                // Get tokeninfo for the access token if you want to verify.
//                Oauth2Service service = new Oauth2Service(
//                    new Google.Apis.Services.BaseClientService.Initializer());
//                Oauth2Service.TokeninfoRequest request = service.Tokeninfo();
//                request.AccessToken = token.AccessToken;

//                Tokeninfo info = request.Execute();
//                string gplus_id = info.UserId;

//                UserCredential gplusUserCredential = new UserCredential(flow, "me", token);
//                ps = new PlusService(
//                    new Google.Apis.Services.BaseClientService.Initializer()
//                    {
//                        ApplicationName = "TA-Classifieds",
//                        HttpClientInitializer = gplusUserCredential
//                    });

//                Person some = ps.People.Get("me").Execute();
                
//                userDetails.First_Name = some.Name.GivenName;
//                userDetails.Last_Name = some.Name.FamilyName;
//                userDetails.Email = some.Emails[0].Value;

//                //ViewBag.name = some.DisplayName;
//                //ViewBag.email = some.Emails[0].Value;
//                //ViewBag.img = some.Image.Url;

//                FormsAuthentication.SetAuthCookie(some.DisplayName, true);
//            }

//            return RedirectToAction("insertUser", "Home", new { fn = userDetails.First_Name, ln = userDetails.Last_Name,email=userDetails.Email });
//        }
//    }
//}

