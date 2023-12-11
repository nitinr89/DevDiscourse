using Microsoft.AspNetCore.Identity;
using Microsoft.Owin.Security;
//using CaptchaMvc.HtmlHelpers;
using Devdiscourse.Models;
using Devdiscourse.Models.ResearchModels;
using Devdiscourse.Models.BasicModels;
using Devdiscourse.Models.ViewModel;
using Devdiscourse.Models.Others;
using Microsoft.AspNetCore.Mvc;
using Devdiscourse.Data;
using Microsoft.AspNetCore.Authorization;
using Azure;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevDiscourse.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _db;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext _db)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._db = _db;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.edition = "Global Edition";
            }
            else
            {
                ViewBag.edition = cookie ?? "Global Edition";
            }
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult AccountConfirmation()
        {
            return View();
        }


        //POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "User Account Not Found.");
                return View(model);
            }
            //Add this to check if the email was confirmed.
            //if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            //{
            //    ModelState.AddModelError("", "You need to confirm your email.");
            //    return View(model);
            //}
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.edition = "Global Edition";
            }
            else
            {
                ViewBag.edition = cookie ?? "Global Edition";
            }
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //if (this.IsCaptchaValid("Captcha is not valid"))
                //{
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = DateTime.UtcNow.AddYears(-20),
                    PhoneNumber = "",
                    Country = "",
                    ProfilePic = "/AdminFiles/Logo/img_avatar.png",
                    EmailConfirmed = true,
                    isActive = true,
                    isPRManager = true,
                    CreatedOn = DateTime.Now
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //EmailController email = new EmailController();

                    //string emailData = string.Format("<div style='padding:30px;background-color:#ececec'><div style='margin-left:32%'><img src='http://www.devdiscourse.com/AdminFiles/Logo/Dev-Logo-New.png'/></div>" +
                    //         "<div style='font-size:20px; padding-top:30px;padding-bottom:20px'><span>Please confirm " + model.Email + " account by clicking </span><a href =\"" + callbackUrl + "\">here</a><p>if above link does not work then open this url in browser :</p><p style='font-size:14px'>" + callbackUrl + "</p>" + 
                    //         "</div><div style='background-color:#4d4d4d; text-align:center; margin-top:10px; margin-bottom:10px; padding:10px; cursor:pointer;'><span style ='color:white;'> " +
                    //         "© Copyright 2019 <a href ='http://www.visionri.com' style='color:white;text-decoration:unset;'> VisionRI</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span><span><a href='http://devdiscourse.com/Home/Disclaimer' style='color:white;text-decoration:unset;'>Disclaimer</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
                    //        "<span><a href='http://devdiscourse.com/Home/TermsConditions' style='color:white;text-decoration:unset;'>Terms & Conditions</a></span></div></div>");

                    //email.SendMail(model.Email, emailData, "Confirm Devdiscourse account");

                    //using (var client = new WebClient())
                    //{
                    //    var values = new NameValueCollection
                    //    {
                    //        ["FirstName"] = model.FirstName,
                    //        ["LastName"] = model.LastName,
                    //        ["Email"] = model.Email,
                    //        ["UserName"] = model.UserName,
                    //        ["Password"] = model.Password,
                    //        ["ConfirmPassword"] = model.ConfirmPassword,
                    //    };
                    //    var response = client.UploadValues("https://localhost:44331/Account/ExternalRegister/", values);
                    //    var responseString = Encoding.Default.GetString(response);
                    //}
                    //bool roleExists = await roleManager.RoleExistsAsync("SuperAdmin");
                    //if (!roleExists)
                    //{
                    //    // Create the role "SuperAdmin"
                    //    var newRole = new IdentityRole("SuperAdmin");
                    //    await roleManager.CreateAsync(newRole);
                    //}
                    //bool roleExists2 = await roleManager.RoleExistsAsync("Admin");
                    //if (!roleExists2)
                    //{
                    //    // Create the role "SuperAdmin"
                    //    var newRole = new IdentityRole("Admin");
                    //    await roleManager.CreateAsync(newRole);
                    //}
                    await userManager.AddToRoleAsync(user, "Subscriber");
                    await userManager.AddToRoleAsync(user, "SuperAdmin");
                    // return RedirectToAction("AccountConfirmation", "Account");
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
                //}
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult InternshipRegister()
        {
            string? cookie = Request.Cookies["Edition"];
            if (cookie == null)
            {
                ViewBag.edition = "Global Edition";
            }
            else
            {
                ViewBag.edition = cookie ?? "Global Edition";
            }
            ViewBag.Sectors = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
            ViewBag.Editions = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title");

            return View();
        }

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> InternshipRegister(InternshipRegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (this.IsCaptchaValid("Captcha is not valid"))
        //        {
        //            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, FirstName = model.Name, LastName = "", DateOfBirth = DateTime.UtcNow.AddYears(-20), PhoneNumber = "", Country = model.Nationality, ProfilePic = "/AdminFiles/Logo/img_avatar.png", EmailConfirmed = true };
        //            var result = await UserManager.CreateAsync(user, model.Password);
        //            if (result.Succeeded)
        //            {

        //                if (Request.Files.Count > 0)
        //                {
        //                    for (var i = 0; i < Request.Files.Count; i++)
        //                    {
        //                        var file = Request.Files[i];
        //                        if (file == null || file.ContentLength <= 0) continue;
        //                        var fileName = RandomName();
        //                        var fileExtension = Path.GetExtension(file.FileName);
        //                        var fileKey = Request.Files.Keys[i];
        //                        if (fileKey == "CVUrl")
        //                        {
        //                            CloudBlobContainer blobContainer = GetCloudBlobContainer();
        //                            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName + fileExtension);
        //                            blob.UploadFromStream(file.InputStream);
        //                            model.CVUrl = blob.Uri.ToString();
        //                            //var path = Path.Combine(Server.MapPath("~/AdminFiles/MediaInternshipCVs/"), fileName + fileExtension);
        //                            //file.SaveAs(path);
        //                            //model.CVUrl = "/AdminFiles/MediaInternshipCVs/" + fileName + fileExtension;
        //                        }
        //                    }
        //                }
        //                MediaInternship mediaInternship = new MediaInternship()
        //                {
        //                    UserId = user.Id,
        //                    Editions = model.Editions,
        //                    Sectors = model.Sectors,
        //                    CVUrl = model.CVUrl
        //                };
        //                _db.MediaInternships.Add(mediaInternship);
        //                await _db.SaveChangesAsync();
        //                await this.UserManager.AddToRoleAsync(user.Id, "Subscriber");
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToAction("Index", "Home");
        //            }
        //            AddErrors(result);
        //        }
        //    }
        //    ViewBag.Sectors = new SelectList(_db.DevSectors.Where(a => a.Id != 8 && a.Id != 16).OrderBy(a => a.SrNo), "Id", "Title");
        //    ViewBag.Editions = new SelectList(_db.Regions.Where(a => a.Title.ToUpper() != "AFRICA".Trim() && a.Title.ToUpper() != "GLOBAL EDITION").OrderBy(a => a.SrNo), "Title", "Title");
        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}
        //[AllowAnonymous]
        //public JsonResult RegisterUser(MobileUserViewModel obj)
        //{
        //    var user = new ApplicationUser { UserName = obj.email, Email = obj.email, EmailConfirmed = true, FirstName = obj.name, LastName = " ", DateOfBirth = DateTime.UtcNow.AddYears(-20), PhoneNumber = obj.phonenumber, Country = "", ProfilePic = "/AdminFiles/Logo/img_avatar.png" };
        //    var search = _db.Users.Where(a => a.Email == obj.email).SingleOrDefault();
        //    if (search != null)
        //    {
        //        return Json(new { msg = "Email is already Registered." });
        //    }
        //    var result = UserManager.Create(user, obj.password);
        //    if (result.Succeeded)
        //    {
        //        this.UserManager.AddToRole(user.Id, "Subscriber");
        //        SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
        //        var usersearch = (from m in _db.Users
        //                          where m.UserName == obj.email
        //                          select new
        //                          {
        //                              m.FirstName,
        //                              m.Id,
        //                              m.Email,
        //                              msg = "Success"
        //                          }).SingleOrDefault();
        //        return Json(usersearch, JsonRequestBehavior.AllowGet);
        //    }
        //    var resultnew = new { Error = result.Errors };
        //    return Json(resultnew, JsonRequestBehavior.AllowGet);
        //}

        public string RandomName()
        {
            var time = DateTime.UtcNow.ToLocalTime();
            return time.ToString("dd_MM_yyyy_HH_mm_ss_FFFFFFF");
        }
        //private CloudBlobContainer GetCloudBlobContainer()
        //{
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("devdiscourse_AzureStorageConnectionString"));
        //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        //    CloudBlobContainer container = blobClient.GetContainerReference("internship");
        //    if (container.CreateIfNotExists())
        //    {
        //        container.SetPermissions(new BlobContainerPermissions
        //        {
        //            PublicAccess = BlobContainerPublicAccessType.Blob
        //        });
        //    }
        //    return container;
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<JsonResult> LoginViaJob(JobLoginView model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.UserName);
        //    if (user == null)
        //    {
        //        return Json("User Account Not Found.", JsonRequestBehavior.AllowGet);
        //    }
        //    //Add this to check if the email was confirmed.
        //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
        //    {
        //        return Json("You need to confirm your email.", JsonRequestBehavior.AllowGet);
        //    }
        //    // This doesn't count login failures towards account lockout
        //    // To enable password failures to trigger account lockout, change to shouldLockout: true
        //    var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, bool.Parse(model.RememberMe), shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //            return Json("Login Successfull.", JsonRequestBehavior.AllowGet);
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //            return Json("Invalid login attempt.", JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<JsonResult> RegisterViaJob(JobRegisterView model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, DateOfBirth = DateTime.UtcNow.AddYears(-20), PhoneNumber = "", Country = "", ProfilePic = "/AdminFiles/Logo/img_avatar.png" };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            await this.UserManager.AddToRoleAsync(user.Id, "Subscriber");
        //            return Json("Success", JsonRequestBehavior.AllowGet);
        //        }
        //        var resultnew = new { Error = result.Errors };
        //        return Json(resultnew, JsonRequestBehavior.AllowGet);
        //    }
        //    // If we got this far, something failed, redisplay form
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //public JsonResult RegisterMobileUser(MobileUserViewModel obj )
        //{
        //    var user = new ApplicationUser { UserName = obj.email, Email = obj.email, EmailConfirmed = true, FirstName = obj.name, LastName = " ", DateOfBirth = DateTime.UtcNow.AddYears(-20), PhoneNumber = obj.phonenumber, Country = "", ProfilePic = "/AdminFiles/Logo/img_avatar.png" };
        //    var search = _db.Users.Where(a => a.Email == obj.email).Select(a => new {a.FirstName, a.Id, a.Email }).SingleOrDefault();
        //    if(search != null)
        //    {
        //        return Json(search);
        //    }
        //    var result = UserManager.Create(user, obj.password);
        //    if (result.Succeeded)
        //    {
        //         this.UserManager.AddToRole(user.Id, "Subscriber");
        //         SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);              
        //         var usersearch = (from m in _db.Users
        //                            where m.UserName == obj.email
        //                            select new
        //                            {
        //                                m.FirstName,
        //                                m.Id,
        //                                m.Email
        //                            }).SingleOrDefault();
        //        return Json(usersearch, JsonRequestBehavior.AllowGet);
        //    }
        //    var resultnew = new { Error = result.Errors };
        //    return Json(resultnew, JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //public JsonResult LoginMobileUser(LoginMobileViewModel obj)
        //{
        //    string username = "";
        //    username = _db.Users.Where(a => a.Email == obj.email).FirstOrDefault().UserName;
        //    var result = SignInManager.PasswordSignIn(username, obj.password, false, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            var user = (from m in _db.Users
        //                        where m.UserName == username
        //                        select new
        //                        {
        //                            m.FirstName,
        //                            m.Id,
        //                            m.PhoneNumber,
        //                            m.DateOfBirth,
        //                            m.Email,
        //                            m.ProfilePic,
        //                            msg = "Success"
        //                        }).SingleOrDefault();
        //            return Json(user, JsonRequestBehavior.AllowGet);
        //        case SignInStatus.Failure:
        //        default:
        //            var resultnew = new { msg = "Invalid login attempt" };
        //            return Json(resultnew, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //// To Join SDG if not Register on Devdiscourse then register first and Join SDG
        //[AllowAnonymous]
        //public JsonResult RegisterSDGs(string name,string email)
        //{
        //    string sdgCode = RandomString();
        //    var userSearch = _db.Users.FirstOrDefault(a => a.Email == email);
        //    if(userSearch == null)
        //    {
        //        string password = "password123";
        //        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FirstName = name, LastName = " ", DateOfBirth = DateTime.UtcNow.AddYears(-20), PhoneNumber = "", Country = "", ProfilePic = "/AdminFiles/Logo/img_avatar.png" };
        //        var result = UserManager.Create(user, password);
        //        if (result.Succeeded)
        //        {
        //            this.UserManager.AddToRole(user.Id, "Subscriber");
        //            SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);

        //            var search = _db.SDGSamurais.FirstOrDefault(a => a.Email == email);
        //            // Join SDGs
        //            SDGSamurai newObj = new SDGSamurai
        //            {
        //                Name = name,
        //                Profession = "",
        //                Description = "",
        //                Email = email,
        //                Nationality = "",
        //                Gender = "",
        //                IsActive = true,
        //                SDGCode = sdgCode,
        //                ReferenceCode = "",
        //                SDGPoints = 1.0,
        //                SDGPosition = 1,
        //                Creator = user.Id
        //            };
        //            _db.SDGSamurais.Add(newObj);
        //            _db.SaveChanges();
        //            CreateLog("SDG Samurai", "Volunteer", newObj.Id.ToString(), newObj.Creator, "/Research/");
        //            var usersearch = from m in _db.SDGSamurais
        //                             where m.Email == email
        //                             select new
        //                             {
        //                                 ReferenceCode = m.ReferenceCode,
        //                                 SDGCode = sdgCode,
        //                             };
        //            return Json(usersearch.ToList(), JsonRequestBehavior.AllowGet);
        //        }
        //        var resultnew = new { Error = result.Errors };
        //        return Json(resultnew, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var search = _db.SDGSamurais.FirstOrDefault(a => a.Email == email);
        //        if (search == null)
        //        {
        //            SDGSamurai newObj = new SDGSamurai
        //            {
        //                Name = name,
        //                Profession = "",
        //                Description = "",
        //                Email = email,
        //                Nationality = "",
        //                Gender = "",
        //                IsActive = true,
        //                SDGCode = sdgCode,
        //                ReferenceCode = "",
        //                SDGPoints = 1.0,
        //                SDGPosition = 1,
        //                Creator = userSearch.Id
        //            };
        //            _db.SDGSamurais.Add(newObj);
        //            _db.SaveChanges();
        //            CreateLog("SDG Samurai", "Volunteer", newObj.Id.ToString(), newObj.Creator, "/Research/");
        //            var usersearch = from m in _db.SDGSamurais
        //                             where m.Email == email
        //                             select new
        //                             {
        //                                 Id = m.ApplicationUsers.Id,
        //                                 ReferenceCode = m.ReferenceCode,
        //                                 SDGCode = m.SDGCode,
        //                             };
        //            return Json(usersearch, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var resultnew = from m in _db.SDGSamurais
        //                            where m.Email == email
        //                            select new
        //                            {
        //                                Id = m.ApplicationUsers.Id,
        //                                ReferenceCode = m.ReferenceCode,
        //                                SDGCode = m.SDGCode,
        //                            };
        //            return Json(resultnew, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //}
        public string RandomString()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            var FormNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
            return FormNumber.ToString("X");
        }
        //[AllowAnonymous]
        //public JsonResult LoginSDGs(LoginMobileViewModel obj)
        //{
        //    string username = "";
        //    username = _db.Users.Where(a => a.Email == obj.email).FirstOrDefault().UserName;
        //    var result = SignInManager.PasswordSignIn(username, obj.password, false, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            var user = (from m in _db.SDGSamurais
        //                        where m.ApplicationUsers.Email == username
        //                        select new
        //                        {
        //                            m.ApplicationUsers.FirstName,
        //                            m.ApplicationUsers.Id,
        //                            m.ApplicationUsers.PhoneNumber,
        //                            m.ApplicationUsers.DateOfBirth,
        //                            m.ApplicationUsers.Email,
        //                            m.ApplicationUsers.ProfilePic,
        //                            m.SDGCode,
        //                            m.ReferenceCode
        //                        }).SingleOrDefault();
        //            return Json(user, JsonRequestBehavior.AllowGet);
        //        case SignInStatus.Failure:
        //        default:
        //            var resultnew = new { Error = "Invalid login attempt" };
        //            return Json(resultnew, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //// Press Release Register
        // GET: /Account/Registration
        [AllowAnonymous]
        public ActionResult Registration()
        {
            return View();
        }

        //// POST: /Account/Registration
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Registration(RegistrationViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (this.IsCaptchaValid("Captcha is not valid"))
        //        {
        //            string address = model.Street + "," + model.City + "," + model.State + "," + model.ZipCode;
        //            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email,EmailConfirmed = true, FirstName = model.FirstName, LastName = model.LastName, DateOfBirth = DateTime.UtcNow.AddYears(-20), PhoneNumber = model.PhoneNumber, Country = model.Country, ProfilePic = "/AdminFiles/Logo/img_avatar.png", CompanyName = model.CompanyName, Position = model.Position, Address = address,OrganizationType = model.OrganizationType };
        //            var result = await UserManager.CreateAsync(user, model.Password);
        //            if (result.Succeeded)
        //            {
        //                //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
        //                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //                // Send an email with this link
        //                //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //                //EmailController email = new EmailController();

        //                //string emailData = string.Format("<div style='padding:30px;background-color:#ececec'><div style='margin-left:32%'><img src='http://www.devdiscourse.com/AdminFiles/Logo/Dev-Logo-New.png'/></div>" +
        //                //         "<div style='font-size:20px; padding-top:30px;padding-bottom:20px'><span>Please confirm " + model.Email + " account by clicking </span><a href =\"" + callbackUrl + "\">here</a><p>if above link does not work then open this url in browser :</p><p style='font-size:14px'>" + callbackUrl + "</p>" +
        //                //         "</div><div style='background-color:#4d4d4d; text-align:center; margin-top:10px; margin-bottom:10px; padding:10px; cursor:pointer;'><span style ='color:white;'> " +
        //                //         "© Copyright 2019 <a href ='http://www.visionri.com' style='color:white;text-decoration:unset;'> VisionRI</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span><span><a href='http://devdiscourse.com/Home/Disclaimer' style='color:white;text-decoration:unset;'>Disclaimer</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
        //                //        "<span><a href='http://devdiscourse.com/Home/TermsConditions' style='color:white;text-decoration:unset;'>Terms & Conditions</a></span></div></div>");

        //                //email.SendMail(model.Email, emailData, "Confirm Devdiscourse account");
        //                await this.UserManager.AddToRoleAsync(user.Id, "PressRelease");
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToAction("Index", "NewsWire");
        //            }
        //            AddErrors(result);
        //        }
        //    }
        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}
        //// Id : AdoptSDGTool Id
        //public ActionResult Permission(Guid id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AdoptSDGTool adoptSDGTool = _db.AdoptSDGTools.Find(id);
        //    TempData["isActive"] = adoptSDGTool.IsActive;
        //    if (adoptSDGTool == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(adoptSDGTool);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Permission([Bind(Include = "Id,Name,Institution,Location,Email,Contact,NeededData,GeographicalData,ThemeticArea,Message,IsActive,CreatedOn")] AdoptSDGTool adoptSDGTool)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Register Partners at Devdiscourse
        //        var username = adoptSDGTool.Name.Replace(" ","").ToLower();
        //        var firstName = adoptSDGTool.Name.Split(' ')[0];
        //        string password = firstName + "@123";

        //        if(bool.Parse(TempData["isActive"].ToString()) == false && adoptSDGTool.IsActive == true)
        //        {
        //            string userId = InstitutionalPartnerRegister(username, password, adoptSDGTool.Name, adoptSDGTool.Email, adoptSDGTool.Contact);
        //            if(userId != "")
        //            {
        //                adoptSDGTool.UserId = userId;
        //                _db.Entry(adoptSDGTool).State = EntityState.Modified;
        //                await _db.SaveChangesAsync();

        //                EmailController email = new EmailController();
        //                string emailData = string.Format("<div style='padding:30px;background-color:#ececec'><div style='margin-left:32%'><img src='http://www.devdiscourse.com/AdminFiles/Logo/Dev-Logo-New.png'/></div>" +
        //                         "<div style='font-size:20px; padding-top:30px;'><span>Congratulation! Your account has been approved for Devdiscourse Institutional Partnership.</span></div><div style='padding-bottom:20px'><p> Your Devdiscourse Login details - </p><p>user name : " + username + "</p><p>password : " + password + "</p><a href ='https://www.devdiscourse.com'>www.devdiscourse.com</a></div>" +
        //                         "<div style='background-color:#4d4d4d; text-align:center; margin-top:10px; margin-bottom:10px; padding:10px; cursor:pointer;'><span style ='color:white;'> " +
        //                         "© Copyright 2019 <a href ='http://www.visionri.com' style='color:white;text-decoration:unset;'> VisionRI</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span><span><a href='http://devdiscourse.com/Home/Disclaimer' style='color:white;text-decoration:unset;'>Disclaimer</a></span><span style='margin-left:10px;margin-right:10px;height:30px;border-left:2px solid white;'></span>" +
        //                        "<span><a href='http://devdiscourse.com/Home/PrivacyPolicy' style='color:white;text-decoration:unset;'>Terms & Conditions</a></span></div></div>");

        //               await email.SendEmailAsync(adoptSDGTool.Email, emailData, "Institutional Partners");
        //            }
        //        }
        //        else
        //        {
        //            return View(adoptSDGTool);
        //        }
        //        return RedirectToAction("Partners", "Institutional");
        //    }
        //    return View(adoptSDGTool);
        //}
        //[AllowAnonymous]
        //public string InstitutionalPartnerRegister(string username, string password, string name, string email, string contact)
        //{
        //    var user = new ApplicationUser { UserName = username, Email = email, FirstName = name, LastName = " ", DateOfBirth = DateTime.UtcNow.AddYears(-20), PhoneNumber = contact, Country = "", ProfilePic = "/AdminFiles/Logo/img_avatar.png" };
        //    var result = UserManager.Create(user, password);
        //    if (result.Succeeded)
        //    {
        //        this.UserManager.AddToRole(user.Id, "InstitutionalPartner");
        //        return user.Id;
        //    }
        //    return "";
        //}
        //public void CreateLog(string title, string logFor, string creator, string activityToUser, string activityUrl)
        //{
        //    ActivityLog logs = new ActivityLog
        //    {
        //        LogTitle = title,
        //        LogDescription = title,
        //        CreatorId = creator,
        //        ActivityUserId = activityToUser,
        //        Activity = logFor,
        //        ActivityUrl = activityUrl,
        //        ActivityDate = DateTime.Now,
        //        IsRead = false
        //    };
        //    _db.ActivityLogs.Add(logs);
        //    _db.SaveChanges();
        //}
        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByEmailAsync(model.Email);
        //        if (user == null)
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            ModelState.AddModelError("", "Please enter your valid email.");
        //            return View(model);
        //        }else if(!(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            ModelState.AddModelError("", "Email is not verified.");
        //            return View(model);
        //        }
        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //        EmailController emailObj = new EmailController();
        //        string FilePath = Server.MapPath("~/Content/email-templates/ResetPassword.html");
        //        string Emailbody;
        //        using (var sr = new StreamReader(FilePath))
        //        {
        //            Emailbody = sr.ReadToEnd();
        //        }
        //        Emailbody = Emailbody.Replace("{0}", user.FirstName);
        //        Emailbody = Emailbody.Replace("{1}", callbackUrl);
        //        await emailObj.SendEmailAsync(model.Email, Emailbody, "Reset Password");
        //        return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult> AppForgotPassword(ForgotPasswordViewModel model)
        //{
        //    var user = await UserManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return Json(new { msg = "Please Enter Valid Email." }, JsonRequestBehavior.AllowGet);
        //    }
        //    var digitCode = GenerateRandomNo();
        //    var search = _db.Users.FirstOrDefault(a => a.Email == model.Email);
        //    search.DigitCode = digitCode;
        //    _db.Entry(search).State = EntityState.Modified;
        //    _db.SaveChanges();

        //    EmailController emailObj = new EmailController();
        //    string FilePath = Server.MapPath("~/Content/email-templates/AppResetPassword.html");
        //    string Emailbody;
        //    using (var sr = new StreamReader(FilePath))
        //    {
        //        Emailbody = sr.ReadToEnd();
        //    }
        //    Emailbody = Emailbody.Replace("{0}", user.FirstName);
        //    Emailbody = Emailbody.Replace("{1}", digitCode);
        //    await emailObj.SendEmailAsync(model.Email, Emailbody, "Reset Password");
        //    //return RedirectToAction("ForgotPasswordConfirmation", "Account");

        //    // If we got this far, something failed, redisplay form
        //    return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
        //}
        //public string GenerateRandomNo()
        //{
        //    int _min = 1000;
        //    int _max = 9999;
        //    Random _rdm = new Random();
        //    return _rdm.Next(_min, _max).ToString();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult> AppResetPassword(ResetPasswordViewModel model)
        //{
        //    var user = await UserManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return Json(new { status ="Error", msg = "User not found" });
        //    }
        //    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //    var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return Json(new { status = "Success", msg = "Password has been changed Successfully" });
        //    }
        //    else
        //    {
        //        return Json(new { status = "Error", msg = "Somethig went wrong"});
        //    }
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        ModelState.AddModelError("", "Please enter your valid email.");
        //        return View(model);
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, Name = loginInfo.DefaultUserName });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.Name, LastName = " ", DateOfBirth = DateTime.UtcNow.AddYears(-20), PhoneNumber = "", Country = "", ProfilePic = "/AdminFiles/Logo/img_avatar.png" };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LogOff()
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    return RedirectToAction("Index", "Home");
        //}

        ////
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_userManager != null)
        //        {
        //            _userManager.Dispose();
        //            _userManager = null;
        //        }

        //        if (_signInManager != null)
        //        {
        //            _signInManager.Dispose();
        //            _signInManager = null;
        //        }
        //        if (_db != null)
        //        {
        //            _db.Dispose();
        //            _db = null;
        //        }
        //    }

        //    base.Dispose(disposing);
        //}

        //#region Helpers
        // Used for XSRF protection when adding external logins
        //private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        private void AddErrors(IdentityResult result)
        {
            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError("", error.ToString());
            //}
            var message = string.Join(", ", result.Errors.Select(x => "Code " + x.Code + " Description" + x.Description));
            ModelState.AddModelError("", message);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        //internal class ChallengeResult : HttpUnauthorizedResult
        //{
        //    public ChallengeResult(string provider, string redirectUri)
        //        : this(provider, redirectUri, null)
        //    {
        //    }

        //    public ChallengeResult(string provider, string redirectUri, string userId)
        //    {
        //        LoginProvider = provider;
        //        RedirectUri = redirectUri;
        //        UserId = userId;
        //    }

        //    public string LoginProvider { get; set; }
        //    public string RedirectUri { get; set; }
        //    public string UserId { get; set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        //        if (UserId != null)
        //        {
        //            properties.Dictionary[XsrfKey] = UserId;
        //        }
        //        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        //    }
        //}
        //#endregion
    }
}