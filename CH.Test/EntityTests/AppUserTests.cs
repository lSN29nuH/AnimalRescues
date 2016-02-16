﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Common.Identity;
using CritterHeroes.Web.Contracts.Logging;
using CritterHeroes.Web.Data.Contexts;
using CritterHeroes.Web.Data.Extensions;
using CritterHeroes.Web.Data.Models.Identity;
using CritterHeroes.Web.Data.Storage;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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

            Mock<IHistoryLogger> mockLogger = new Mock<IHistoryLogger>();

            using (AppUserStorageContext userContext = new AppUserStorageContext(mockLogger.Object))
            {
                userContext.FillWithTestData(appUser, "Id", "PasswordHash", "Email", "UserName");

                userContext.Users.Add(appUser);
                await userContext.SaveChangesAsync();

                AppUserManager userManager = new AppUserManager(new AppUserStore(userContext));
                string token = await userManager.GeneratePasswordResetTokenAsync(appUser.Id);
                IdentityResult resetResult = await userManager.ResetPasswordAsync(appUser.Id, token, password);
                resetResult.Succeeded.Should().BeTrue(string.Join(", ", resetResult.Errors));
            }

            using (AppUserStorageContext userContext = new AppUserStorageContext(mockLogger.Object))
            {
                AppUser result = await userContext.Entities.FindByIDAsync(appUser.Id);
                result.Should().NotBeNull();

                result.Email.Should().Be(appUser.Email);

                AppUserManager userManager = new AppUserManager(new AppUserStore(userContext));
                (await userManager.CheckPasswordAsync(result, password)).Should().BeTrue();

                userContext.Delete(result);
                await userContext.SaveChangesAsync();

                userContext.Entities.MatchingID(appUser.Id).SingleOrDefault().Should().BeNull();
            }
        }

        [TestMethod]
        public void UsernameAndEmailAreTheSame()
        {
            AppUser user = new AppUser("email@email.com");
            user.Email.Should().Be("email@email.com");
            user.UserName.Should().Be(user.Email);

            user.UserName = "new@new.com";
            user.Email.Should().Be(user.UserName);
        }

        [TestMethod]
        public async Task CanReadAndWritePerson()
        {
            AppUser appUser = new AppUser("email@email.com");
            appUser.Person.FirstName = "FirstName";
            appUser.Person.LastName = "LastName";

            Mock<IHistoryLogger> mockLogger = new Mock<IHistoryLogger>();

            using (AppUserStorageContext userContext = new AppUserStorageContext(mockLogger.Object))
            {
                userContext.Users.Add(appUser);
                await userContext.SaveChangesAsync();
            }

            using (AppUserStorageContext userContext = new AppUserStorageContext(mockLogger.Object))
            {
                AppUser result = await userContext.Entities.FindByIDAsync(appUser.Id);
                result.Should().NotBeNull();

                result.Person.FirstName.Should().Be(appUser.Person.FirstName);
                result.Person.LastName.Should().Be(appUser.Person.LastName);
            }
        }
    }
}
