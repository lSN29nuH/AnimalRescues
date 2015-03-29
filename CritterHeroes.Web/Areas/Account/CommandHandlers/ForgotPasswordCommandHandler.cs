﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Areas.Account.Models;
using CritterHeroes.Web.Common.Commands;
using CritterHeroes.Web.Common.Email;
using CritterHeroes.Web.Common.Identity;
using CritterHeroes.Web.Common.StateManagement;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Commands;
using CritterHeroes.Web.Contracts.Email;
using CritterHeroes.Web.Contracts.Identity;
using CritterHeroes.Web.Contracts.Logging;
using CritterHeroes.Web.Models;
using CritterHeroes.Web.Models.Logging;

namespace CritterHeroes.Web.Areas.Account.CommandHandlers
{
    public class ForgotPasswordCommandHandler : IAsyncCommandHandler<ForgotPasswordModel>
    {
        private IUserLogger _userLogger;
        private IApplicationUserManager _appUserManager;
        private IEmailClient _emailClient;
        private IUrlGenerator _urlGenerator;
        private OrganizationContext _organizationContext;

        public ForgotPasswordCommandHandler(IUserLogger userLogger, IApplicationUserManager userManager, IEmailClient emailClient, IUrlGenerator urlGenerator, OrganizationContext organizationContext)
        {
            this._userLogger = userLogger;
            this._appUserManager = userManager;
            this._emailClient = emailClient;
            this._urlGenerator = urlGenerator;
            this._organizationContext = organizationContext;
        }

        public async Task<CommandResult> ExecuteAsync(ForgotPasswordModel command)
        {
            IdentityUser user = await _appUserManager.FindByEmailAsync(command.ResetPasswordEmail);

            if (user == null)
            {
                // We don't want to reveal whether or not the username or email address are valid
                // so if the user isn't found just return success
                await _userLogger.LogActionAsync(UserActions.ForgotPasswordFailure, command.ResetPasswordEmail);
                return CommandResult.Success();
            }

            string code = await _appUserManager.GeneratePasswordResetTokenAsync(user.Id);
            string url = _urlGenerator.GenerateAbsoluteUrl<AccountController>(x => x.ResetPassword(code));

            EmailMessage emailMessage = new EmailMessage()
            {
                Subject = "Password Reset - " + _organizationContext.FullName,
                From = _organizationContext.EmailAddress
            };
            emailMessage.To.Add(user.Email);

            EmailBuilder
                .Begin(emailMessage)
                .AddParagraph("Your password for your account at " + _organizationContext.FullName + " has been reset. To complete resetting your password, click the link below or visit " + _organizationContext.FullName + " and copy the code into the provided form. This code will be valid for " + _appUserManager.TokenLifespan.TotalHours.ToString() + " hours.")
                .AddParagraph("Confirmation Code: " + code)
                .AddParagraph("<a href=\"" + url + "\">Reset Password</a>")
                .End();

            await _emailClient.SendAsync(emailMessage, user.Id);
            await _userLogger.LogActionAsync(UserActions.ForgotPasswordSuccess, user.Email);
            return CommandResult.Success();
        }
    }
}