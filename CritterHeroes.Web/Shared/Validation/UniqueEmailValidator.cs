﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CritterHeroes.Web.Data.Models.Identity;
using CritterHeroes.Web.Domain.Contracts;
using CritterHeroes.Web.Domain.Contracts.Identity;
using FluentValidation;
using FluentValidation.Validators;

namespace CritterHeroes.Web.Shared.Validation
{
    public static class UniqueEmailValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> MustHaveUniqueEmail<T>(this IRuleBuilder<T, string> ruleBuilder, IAppUserManager userManager, IHttpUser httpUser)
        {
            return ruleBuilder.SetValidator(new UniqueEmailValidator(userManager, httpUser));
        }
    }

    public class UniqueEmailValidator : AsyncValidatorBase
    {
        private IAppUserManager _userManager;
        private IHttpUser _httpUser;

        public UniqueEmailValidator(IAppUserManager userManager, IHttpUser httpUser)
            : base("{PropertyName} must be unique.")
        {
            this._userManager = userManager;
            this._httpUser = httpUser;
        }

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            string email = context.PropertyValue as string;

            if (!_httpUser.Username.Equals(email, StringComparison.InvariantCultureIgnoreCase))
            {
                AppUser dupeUser = await _userManager.FindByEmailAsync(email).ConfigureAwait(continueOnCapturedContext: false);
                if (dupeUser != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
