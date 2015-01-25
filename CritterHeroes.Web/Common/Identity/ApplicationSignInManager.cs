﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace CritterHeroes.Web.Common.Identity
{
    public class ApplicationSignInManager : SignInManager<IdentityUser, string>, IApplicationSignInManager
    {
        public ApplicationSignInManager(IApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager.UserManager, authenticationManager)
        {
        }

        public async Task<SignInStatus> PasswordSignInAsync(string userName, string password)
        {
            return await PasswordSignInAsync(userName, password, isPersistent: false, shouldLockout: false);
        }
    }
}
