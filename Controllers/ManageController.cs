﻿using Devdiscourse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevDiscourse.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            string userId = userManager.GetUserId(User);
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            var model = new IndexViewModel
            {
                HasPassword = HasPassword().Result,
                PhoneNumber = await userManager.GetPhoneNumberAsync(user),
                TwoFactor = await userManager.GetTwoFactorEnabledAsync(user),
                Logins = await userManager.GetLoginsAsync(user),
                BrowserRemembered = false
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            string userId = userManager.GetUserId(User);
            ApplicationUser user = await userManager.FindByIdAsync(userId);

            ManageMessageId? message;
            var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (result.Succeeded)
            {
                var _user = await userManager.FindByIdAsync(userId);
                if (_user != null)
                {
                    await signInManager.SignInAsync(_user, isPersistent: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    // Generate the token and send it
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
        //    if (UserManager.SmsService != null)
        //    {
        //        var message = new IdentityMessage
        //        {
        //            Destination = model.Number,
        //            Body = "Your security code is: " + code
        //        };
        //        await UserManager.SmsService.SendAsync(message);
        //    }
        //    return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        //}

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EnableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DisableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}

        //
        // GET: /Manage/VerifyPhoneNumber
        //public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        //{
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
        //    // Send an SMS through the SMS provider to verify the phone number
        //    return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        //}

        //
        // POST: /Manage/VerifyPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
        //    }
        //    // If we got this far, something failed, redisplay form
        //    ModelState.AddModelError("", "Failed to verify phone");
        //    return View(model);
        //}

        //
        // POST: /Manage/RemovePhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemovePhoneNumber()
        //{
        //    var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
        //    if (!result.Succeeded)
        //    {
        //        return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        //    }
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        //}

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string userId = userManager.GetUserId(User);
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var _user = await userManager.FindByIdAsync(userId);
                if (_user != null)
                {
                    await signInManager.SignInAsync(_user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = userManager.GetUserId(User);
                ApplicationUser user = await userManager.FindByIdAsync(userId);
                var result = await userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    var _user = await userManager.FindByIdAsync(userId);
                    if (_user != null)
                    {
                        await signInManager.SignInAsync(_user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        //public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
        //    var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
        //    ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
        //    return View(new ManageLoginsViewModel
        //    {
        //        CurrentLogins = userLogins,
        //        OtherLogins = otherLogins
        //    });
        //}

        //
        // POST: /Manage/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        //}

        //
        // GET: /Manage/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _userManager != null)
        //    {
        //        _userManager.Dispose();
        //        _userManager = null;
        //    }

        //    base.Dispose(disposing);
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.ToString());
            }
        }

        private async Task<bool> HasPassword()
        {
            string userId = userManager.GetUserId(User);
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private async Task<bool> HasPhoneNumber()
        {
            string userId = userManager.GetUserId(User);
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}