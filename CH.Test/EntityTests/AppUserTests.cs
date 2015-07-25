﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Common.Identity;
using CritterHeroes.Web.Data.Contexts;
using CritterHeroes.Web.Data.Models.Identity;
using CritterHeroes.Web.Data.Storage;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOTD.EntityFramework;

namespace CH.Test.EntityTests
{
    [TestClass]
    public class AppUserTests : BaseEntityTest
    {
        [TestMethod]
        public async Task CanReadAndWriteAppUser()
        {
            // Use a separate context for saving vs retrieving to prevent any caching

            AppUser appUser = new AppUser("email@email.com");
            string password = "Password1!";

            using (AppUserStorageContext userContext = new AppUserStorageContext())
            {
                EntityTestHelper.FillWithTestData(userContext, appUser, "Id", "PasswordHash", "Email");

                userContext.Users.Add(appUser);
                await userContext.SaveChangesAsync();

                AppUserManager userManager = new AppUserManager(new AppUserStore(userContext));
                string token = await userManager.GeneratePasswordResetTokenAsync(appUser.Id);
                IdentityResult resetResult = await userManager.ResetPasswordAsync(appUser.Id, token, password);
            }

            using (AppUserStorageContext userContext = new AppUserStorageContext())
            {
                AppUser result = await userContext.GetAsync(x => x.Id == appUser.Id);
                result.Should().NotBeNull();

                result.FirstName.Should().Be(appUser.FirstName);
                result.LastName.Should().Be(appUser.LastName);
                result.Email.Should().Be(appUser.Email);
                result.PreviousEmail.Should().Be(appUser.PreviousEmail);
                result.PhoneNumber.Should().Be(appUser.PhoneNumber);

                AppUserManager userManager = new AppUserManager(new AppUserStore(userContext));
                (await userManager.CheckPasswordAsync(result, password)).Should().BeTrue();
            }
        }
    }
}