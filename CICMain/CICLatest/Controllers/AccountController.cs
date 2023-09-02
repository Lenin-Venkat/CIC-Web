using CICLatest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.codec.wmf;
using Org.BouncyCastle.Asn1.Crmf;
using System.Net;
using RestSharp;
using CICLatest.Contracts;

namespace CICLatest.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _loginManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache memoryCache;
        private readonly AzureStorageConfiguration _azureConfig;
        private readonly ApplicationContext _context;
        public readonly IAppSettingsReader _appSettingsReader;
        public static string accessToken;
        public readonly IBlobStorageService _blobStorageService;


        public AccountController(AzureStorageConfiguration azureConfig ,IMemoryCache memoryCache,ILogger<HomeController> logger, ApplicationContext context, UserManager<UserModel> userManager
            , SignInManager<UserModel> loginManager, RoleManager<IdentityRole> roleManager, IAppSettingsReader appSettingsReader, IBlobStorageService blobStorageService)
        {
            _logger = logger;
            _appSettingsReader = appSettingsReader;
            _userManager = userManager;
            _loginManager = loginManager;
            _roleManager = roleManager;
            this.memoryCache = memoryCache;
            _azureConfig = azureConfig;
            _context = context;
            _blobStorageService = blobStorageService;
        }

        public IActionResult Registration()
        {
            //loadRoleDropdown();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(RegisterViewModel obj, string Id, string Name)
        {

            if (ModelState.IsValid)
            {
                UserModel user = new UserModel();
                user.Firstname = obj.FirstName;
                user.LastName = obj.LastName;
                user.BusinessName = obj.BusinessName;
                user.Email = obj.Email;
                user.PhoneNumber = obj.PhoneNumber.ToString();
                user.UserName = obj.Email;

                IdentityResult result = _userManager.CreateAsync(user, obj.Password).Result;

                if (result.Succeeded)
                {
                    string custno = AddCustinERP(user);
                    user.CustNo = custno;
                    result= _userManager.UpdateAsync(user).Result;

                    if(result.Succeeded)
                    {
                        var isFound = _roleManager.FindByNameAsync("NormalUser").Result;
                        string rolename = "";
                        if (isFound == null)
                        {
                            IdentityRole role = new IdentityRole();
                            role.Name = "NormalUser";
                            rolename = "NormalUser";
                            IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
                            if (!roleResult.Succeeded)
                            {
                                ModelState.AddModelError(nameof(obj.err), "Error while creating role!");
                                return View(obj);
                            }
                        }
                        else
                        {
                            rolename = isFound.Name;
                        }
                        string domain = _appSettingsReader.Read("Domain");
                        string body = "<p>Hi " + obj.FirstName + ",<br/><br/>Thanks for creating an account on CIC portal. Your username is <b>" + obj.Email +
                                         "</b>. Customer Number- " +custno + ". </br>You can access CIC portal at: <a href='" + domain + "'>CIC Portal</a> <br/><br/>Thank you,<br/>CIC Team</p>";
                        _userManager.AddToRoleAsync(user, rolename).Wait();
                        //Email email = new Email();
                        ViewForm1Controller viewForm1 = new ViewForm1Controller(memoryCache, _azureConfig, _context,_userManager, _appSettingsReader, _blobStorageService);
                        viewForm1.sendNotification(user.Email, "Your CIC account has been created!", body);
                        return RedirectToAction("RegistrationResult", "Account", new { text = custno });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(nameof(obj.err), error.Description);
                        //loadRoleDropdown();
                    }

                }
                else
                {
                    foreach (var error in result.Errors)
                    {                        
                        ModelState.AddModelError(nameof(obj.err), error.Description);
                        //loadRoleDropdown();
                    }
                }
            }
            //loadRoleDropdown();
            return View(obj);
        }

        public void loadRoleDropdown()
        {   
            ViewBag.AllRoles = new SelectList(_roleManager.Roles, "Id", "Name");
        }

        public IActionResult Login()
        {
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel obj)
        {
            
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            if (ModelState.IsValid)
            {
                var result = _loginManager.PasswordSignInAsync(obj.UserName, obj.Password,false, true).Result;

                
                if (result.Succeeded)
                {
                    string RoleName = "";
                    var user = await _userManager.FindByNameAsync(obj.UserName);                    
                    var role = await _userManager.GetRolesAsync(user);
                    //var phonenumber = user.PhoneNumber;
                    //HttpContext.Session.SetString("phonenumber", phonenumber);
                    if (role != null)
                    {
                        RoleName = role[0];
                    }
                    HttpContext.Session.SetString("UserRole", RoleName);
                    HttpContext.Session.SetString("CustNo", user.CustNo);

                    switch (RoleName)
                    {
                        case "Clerk":
                        case "Compliance Officer":                            
                            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");

                        case "Compliance Analyst":
                            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
                        case "CIC Compliance":
                            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");
                        
                        case "Ops Manager":
                            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");

                        case "CEO":
                            return RedirectToAction("ReviewerDashboard", "ReviewerDashboard");

                        case "Admin":
                            return RedirectToAction("AdminDashboard", "Admin");

                        case "NormalUser":
                            return RedirectToAction("Index", "Home");

                    }
                   
                }
                else
                {
                    ModelState.AddModelError("CustomError", "Invalid UserName or Password");
                    return View();
                }

                if (result.IsLockedOut)
                {
                    //var forgotPassLink = Url.Action("ResetPassword", "Account", new { }, Request.Scheme);
                  //  var content = string.Format("Your account is locked out, to reset your password, please click this link: {0}", forgotPassLink);
                  //  var message = new Message(new string[] { userModel.Email }, "Locked out account information", content, null);
                   // await _emailSender.SendEmailAsync(message);
                    ModelState.AddModelError("", "The account is locked out");                    
                    return View();
                }
                else
                {
                    //attemptLogin = attemptLogin + 1;
                    ModelState.AddModelError("CustomError", "Invalid Login Attempt:");
                    return View();
                }
                
            }

            return View(obj);
        }


        public IActionResult ResetPassword(string email)
        {
            var model = new ResetPasswordModel { EmailOrPhone = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);

            //resetPasswordModel.Email = "test@test.com";
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.EmailOrPhone);

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPassResult = await _userManager.ResetPasswordAsync(user, token, resetPasswordModel.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return View();
                }
               // var result = await _userManager.rem(user, claim);
            }
            
            return RedirectToAction("ResetPasswordResult", "Account");
        }

        public IActionResult ResetPasswordResult()
        {
            return View();
        }
        public IActionResult RegistrationResult(string text)
        {
            ViewBag.cust = text;
            return View();
        }

        //[HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _loginManager.SignOutAsync();            
           // memoryCache.Dispose();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            bool isEmail = false;
            if (forgotPasswordModel.EmailPhoneNumber.Contains("@"))
            {
                isEmail = true;
                ViewBag.isEmail = true;
            }
            else
            {
                ViewBag.isEmail = false;
            }

            if (!ModelState.IsValid)
                return View(forgotPasswordModel);
            
            Random rnd = new Random();
            string randomNumber = (rnd.Next(100000, 999999)).ToString();

            var claim = new Claim("OTPEmail", randomNumber);
            

            if (isEmail)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.EmailPhoneNumber);
                if(user!=null)
                {                    
                    var result = await _userManager.AddClaimAsync(user, claim);
                    string body = "<p>Hi " + user.Firstname + ",<br/><br/>Please enter this One Time Password:" + randomNumber + "</br> <br/>Thank you,<br/>CIC Team</p>";
                    ViewForm1Controller view1Controller = new ViewForm1Controller(memoryCache,_azureConfig, _context,_userManager, _appSettingsReader, _blobStorageService);
                    view1Controller.sendNotification(forgotPasswordModel.EmailPhoneNumber, "Reset password OTP", body);
                }
                else
                {
                    ModelState.AddModelError("CustomError", "Email address not found");
                    return View();
                }
               
            }
            else
            {
                var users = _userManager.Users;
                var email = "";
                foreach (var item in users)
                {
                    if (item.PhoneNumber == forgotPasswordModel.EmailPhoneNumber)
                    {
                        email = item.Email;
                    }
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                  
                    var result = await _userManager.AddClaimAsync(user, claim);
           
                    string body = "CIC - Please enter this One Time Password:" + randomNumber + "  Thank you,CIC Team";
                    sendOTPSMS(forgotPasswordModel.EmailPhoneNumber, body);
                    //var c = new FormUrlEncodedContent(new[]
                    //    {
                    //    new KeyValuePair<string, string>("from", "CIC"),
                    //    new KeyValuePair<string, string>("body", body),
                    //    new KeyValuePair<string, string>("touser", forgotPasswordModel.EmailPhoneNumber),

                    //});

                    //using (var httpClient = new HttpClient())
                    //{

                    //    httpClient.DefaultRequestHeaders.Clear();
                    //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //    HttpResponseMessage response = httpClient.PostAsync("https://ciccommunicationapi.azurewebsites.net/api/Sms?from=CIC&body= " + body + "&touser=" + forgotPasswordModel.EmailPhoneNumber, c).Result;
                    //    response.EnsureSuccessStatusCode();

                    //}
                }
                else
                {
                    ModelState.AddModelError("CustomError", "Phone number not found");
                    return View();
                }
            }

            return RedirectToAction(nameof(ForgotPasswordOTPConfirmation), new { email = forgotPasswordModel.EmailPhoneNumber});
        }

        public IActionResult ForgotPasswordOTPConfirmation(string email)
        {
            if (email.Contains("@"))
            {
                ViewBag.isEmail = true;
            }
            else
            {
                ViewBag.isEmail = false;
            }
            var model = new ResetPasswordModel { EmailOrPhone = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordOTPConfirmation(ResetPasswordModel model)
        {
            string emailid = "";
            bool isEmail = false;
            if (model.EmailOrPhone.Contains("@"))
            {
                isEmail = true;
                ViewBag.isEmail = true;
            }
            else
            {
                ViewBag.isEmail = false;
            }

            if (model.Token ==null)
            {
                ModelState.AddModelError("CustomError", "Please enter OTP");
                return View();
            }            

            if(isEmail)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailOrPhone);
                if (user != null)
                {
                    var Claims = await _userManager.GetClaimsAsync(user);
                    bool OTLFlag = false;
                    if (Claims != null)
                    {
                        foreach (var item in Claims)
                        {
                            if (model.Token == item.Value)
                            {
                                OTLFlag = true;
                                break;
                            }
                        }

                        if (OTLFlag)
                        {
                            return RedirectToAction(nameof(ResetPassword), new { email = model.EmailOrPhone });
                        }
                        else
                        {
                            ModelState.AddModelError("CustomError", "Invalid OTP");
                        }
                    }
                }
                
            }
            else
            {
                var users = _userManager.Users;
                
                foreach (var item in users)
                {
                    if (item.PhoneNumber == model.EmailOrPhone)
                    {
                        emailid = item.Email;
                    }
                }

                var user = await _userManager.FindByEmailAsync(emailid);
                if (user != null)
                {
                    var Claims = await _userManager.GetClaimsAsync(user);
                    bool OTLFlag = false;
                    if (Claims != null)
                    {
                        foreach (var item in Claims)
                        {
                            if (model.Token == item.Value)
                            {
                                OTLFlag = true;
                                break;
                            }
                        }

                        if (OTLFlag)
                        {
                            return RedirectToAction(nameof(ResetPassword), new { email = emailid });
                        }
                        else
                        {
                            ModelState.AddModelError("CustomError", "Invalid OTP");
                        }
                    }
                }
            }
            return View();
        }


        public string AddCustinERP(UserModel user)
        {
            GetTokenAndConnectAzure();
            string custno = "";
            try
            {
                var data1 = JObject.FromObject(new
                {
                    displayName = user.Firstname + " " + user.LastName,
                    businessName = user.BusinessName,
                    email = user.Email,
                    phoneNumber = user.PhoneNumber,
                    genBusPostingGrp = "CIC",
                    customerPostingGrp= "CIC",
                    vatBusPostingGrp= "CIC"
                });

                var json = JsonConvert.SerializeObject(data1);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = httpClient.PostAsync(@_azureConfig.BCURL + "/createCustomers", data).Result;
                    if (response.IsSuccessStatusCode)
                    {
                       string str = response.Content.ReadAsStringAsync().Result;
                        JObject myJObject = JObject.Parse(str);
                        custno = (string)myJObject["customerNumber"];
                    }
                    string phno = sendSMS(user.PhoneNumber, custno);                    
                }
                return custno;
            }
            catch
            { return ""; }

        }

        protected void GetTokenAndConnectAzure()
        {
            //Get new token from Azure for BC
            string url = _azureConfig.TokenURL;
       
            //ConfigurationSettings.AppSettings
            Uri uri = new Uri(_azureConfig.Authority.Replace("{AadTenantId}", _azureConfig.AadTenantId));
            Dictionary<string, string> requestBody = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials" },
                    {"client_id" , _azureConfig.ClientId },
                    {"client_secret", _azureConfig.ClientSecret },
                    {"scope", @"https://api.businesscentral.dynamics.com/.default" }
                };

            var content = new FormUrlEncodedContent(requestBody);
            HttpClient client = new HttpClient();
            var response = client.PostAsync(url, content);
            var rescontent = response.Result.Content.ReadAsStringAsync();

            dynamic jsonresult = JsonConvert.DeserializeObject(rescontent.Result);
            accessToken = jsonresult.access_token;
        }


        public static string sendSMS(string textToEncode, string custno)
        {
            try
            {
                var client = new RestClient("https://rest.smsportal.com");
                var authToken = "";

                var apiKey = "9a5713f1-2b58-4aeb-96ca-6c7ea803a0c6";
                //  var apiSecret = "kmohN4EvajUyvWKsTuG2aT/WSZp6XawD";
                var apiSecret = "Cicpass2019#";

                var accountApiCredentials = $"{apiKey}:{apiSecret}";

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(accountApiCredentials);
                var base64Credentials = Convert.ToBase64String(plainTextBytes);

                var authRequest = new RestRequest("/v1/Authentication", Method.Get);

                authRequest.AddHeader("Authorization", $"Basic {base64Credentials}");

                var authResponse = client.Execute(authRequest);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    var authResponseBody = JObject.Parse(authResponse.Content);
                    authToken = authResponseBody["token"].ToString();
                }
                else
                {
                    Console.WriteLine(authResponse.ErrorMessage);
                }
                var sendRequest = new RestRequest("/v1/bulkmessages", Method.Post);

                var authHeader = $"Bearer {authToken}";
                sendRequest.AddHeader("Authorization", $"{authHeader}");

                sendRequest.AddJsonBody(new
                {
                    Messages = new[]
                    {
                  new
                     {
                      content = "Thanks for creating an account on CIC portal. Your Customer Number - " + custno + ".",
                     destination = textToEncode
                     }
                 }
                });

                var sendResponse = client.Execute(sendRequest);

                return "";
            }
            catch
            {
                return "";
            }
        }

        public static string sendOTPSMS(string textToEncode, string body)
        {
            try
            {
                var client = new RestClient("https://rest.smsportal.com");
                var authToken = "";

                //var apiKey = "9a5713f1-2b58-4aeb-96ca-6c7ea803a0c6";
                //var apiSecret = "kmohN4EvajUyvWKsTuG2aT/WSZp6XawD";

                var apiKey = "9a5713f1-2b58-4aeb-96ca-6c7ea803a0c6";
                var apiSecret = "Cicpass2019#";

                var accountApiCredentials = $"{apiKey}:{apiSecret}";

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(accountApiCredentials);
                var base64Credentials = Convert.ToBase64String(plainTextBytes);

                var authRequest = new RestRequest("/v1/Authentication", Method.Get);

                authRequest.AddHeader("Authorization", $"Basic {base64Credentials}");

                var authResponse = client.Execute(authRequest);
                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    var authResponseBody = JObject.Parse(authResponse.Content);
                    authToken = authResponseBody["token"].ToString();
                }
                else
                {
                    Console.WriteLine(authResponse.ErrorMessage);
                }
                var sendRequest = new RestRequest("/v1/bulkmessages", Method.Post);

                var authHeader = $"Bearer {authToken}";
                sendRequest.AddHeader("Authorization", $"{authHeader}");

                sendRequest.AddJsonBody(new
                {
                    Messages = new[]
                    {
                  new
                     {
                      content = body,
                     destination = textToEncode
                     }
                 }
                });

                var sendResponse = client.Execute(sendRequest);

                return "";
            }
            catch
            {
                return "";
            }
        }



        public static string Base64Encode(string textToEncode)
        {
            byte[] textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
            return Convert.ToBase64String(textAsBytes);
        }
    }
}
