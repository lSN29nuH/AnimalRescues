﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Areas.Account.Models;
using CritterHeroes.Web.Common.Commands;
using CritterHeroes.Web.Common.Identity;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Commands;
using CritterHeroes.Web.Contracts.Email;
using CritterHeroes.Web.Contracts.Identity;
using CritterHeroes.Web.Contracts.Logging;
using CritterHeroes.Web.Models.Logging;

namespace CritterHeroes.Web.Areas.Account.CommandHandlers
{
    public class ForgotPasswordCommandHandler : IAsyncCommandHandler<ForgotPasswordModel>
    {
        private IUserLogger _userLogger;
        private IApplicationUserManager _appUserManager;
        private IEmailService _emailService;
        private IUrlGenerator _urlGenerator;

        public ForgotPasswordCommandHandler(IUserLogger userLogger, IApplicationUserManager userManager, IEmailService emailService, IUrlGenerator urlGenerator)
        {
            this._userLogger = userLogger;
            this._appUserManager = userManager;
            this._emailService = emailService;
            this._urlGenerator = urlGenerator;
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

            ResetPasswordEmailCommand emailCommand = new ResetPasswordEmailCommand(user.Email)
            {
                TokenLifespan = _appUserManager.TokenLifespan
            };

            emailCommand.Token = await _appUserManager.GeneratePasswordResetTokenAsync(user.Id);
            emailCommand.Url = _urlGenerator.GenerateAbsoluteUrl<AccountController>(x => x.ResetPassword(emailCommand.Token));

            await _emailService.SendEmailAsync(emailCommand);
            await _userLogger.LogActionAsync(UserActions.ForgotPasswordSuccess, user.Email);

            return CommandResult.Success();
        }
    }
}