// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Exwhyzee.AANI.Web.Helper.TokenHelper;

namespace Exwhyzee.AANI.Host.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        public ForgotPasswordModel(UserManager<Participant> userManager, IEmailSender emailSender, IConfiguration config)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _config = config;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                //var callbackUrl = Url.Page(
                //    "/Account/ResetPassword",
                //    pageHandler: null,
                //    values: new { area = "Identity", code },
                //    protocol: Request.Scheme);
                //await _emailSender.SendEmailAsync(
                //    Input.Email,
                //    "Reset Password",
                //string email =   $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

                //try
                //{
                //    await _emailSender.SendEmailAsync(Input.Email, "Reset Password", email);
                //}
                //catch (Exception ex)
                //{
                //    // Handle or log error
                //}


                // Generate OTP
                var otp = OtpHelper.GenerateOtp();

                user.PasswordResetOtp = otp;
                user.PasswordResetOtpExpiry = DateTime.UtcNow.AddDays(1);
                user.IsPasswordResetOtpUsed = false;

                await _userManager.UpdateAsync(user);

                // OTP Reset Page URL
                var resetUrl = $"{Request.Scheme}://{Request.Host}/Identity/Account/ResetPasswordOtp";

                var emailBody = $@"
<p>Dear {user.Fullname},</p>

<p>You requested to reset your AANI Portal password.</p>

<p><strong>Your One-Time Password (OTP):</strong></p>

<h2 style='color:#d32f2f'>{otp}</h2>

<p>This OTP is valid for <strong>24 hour</strong>.</p>

<p>
<a href='{resetUrl}'
style='display:inline-block;padding:12px 25px;
background:#d32f2f;color:#ffffff;
text-decoration:none;border-radius:5px'>
Click here to reset your password
</a>
</p>

<p>If you did not request this, kindly ignore this email.</p>

<p>— AANI Support</p>";

                try
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "AANI Password Reset – OTP Verification",
                        emailBody
                    );
                }
                catch
                {
                    // log if needed
                }

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
