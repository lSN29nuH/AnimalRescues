﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Areas.Account;
using CritterHeroes.Web.Areas.Account.CommandHandlers;
using CritterHeroes.Web.Areas.Account.Models;
using CritterHeroes.Web.Common.Commands;
using CritterHeroes.Web.Common.Identity;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Identity;
using CritterHeroes.Web.Contracts.Logging;
using CritterHeroes.Web.Models.Logging;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.AccountTests
{
    [TestClass]
    public class ConfirmEmailCommandHandlerTests
    {
        [TestMethod]
        public async Task ReturnsFailedIfUserNotFound()
        {
            ConfirmEmailModel command = new ConfirmEmailModel()
            {
                Email = "email@email.com"
            };

            Mock<IApplicationUserManager> mockUserManager = new Mock<IApplicationUserManager>();
            mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).Returns(Task.FromResult<IdentityUser>(null));

            Mock<IUserLogger> mockUserLogger = new Mock<IUserLogger>();

            ConfirmEmailCommandHandler handler = new ConfirmEmailCommandHandler(mockUserLogger.Object, mockUserManager.Object, null, null);
            CommandResult commandResult = await handler.ExecuteAsync(command);
            commandResult.Succeeded.Should().BeFalse();

            mockUserManager.Verify(x => x.FindByEmailAsync(command.Email), Times.Once);
            mockUserLogger.Verify(x => x.LogActionAsync(UserActions.ConfirmEmailFailure, command.Email));
        }

        [TestMethod]
        public async Task ReturnsFailedIfConfirmEmailFails()
        {
            IdentityUser user = new IdentityUser("email@email.com");

            ConfirmEmailModel command = new ConfirmEmailModel()
            {
                Email = user.Email,
                ConfirmationCode = "invalid"
            };

            IdentityResult identityResult = IdentityResult.Failed("failed");

            Mock<IApplicationUserManager> mockUserManager = new Mock<IApplicationUserManager>();
            mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).Returns(Task.FromResult(user));
            mockUserManager.Setup(x => x.ConfirmEmailAsync(user.Id, command.ConfirmationCode)).Returns(Task.FromResult(identityResult));

            Mock<IUserLogger> mockUserLogger = new Mock<IUserLogger>();

            ConfirmEmailCommandHandler handler = new ConfirmEmailCommandHandler(mockUserLogger.Object, mockUserManager.Object, null, null);
            CommandResult commandResult = await handler.ExecuteAsync(command);
            commandResult.Succeeded.Should().BeFalse();

            mockUserManager.Verify(x => x.FindByEmailAsync(command.Email), Times.Once);
            mockUserManager.Verify(x => x.ConfirmEmailAsync(user.Id, command.ConfirmationCode), Times.Once);
            mockUserLogger.Verify(x => x.LogActionAsync(UserActions.ConfirmEmailFailure, command.Email, identityResult.Errors));
        }

        [TestMethod]
        public async Task ReturnsSucceededIfConfirmEmailSucceeds()
        {
            IdentityUser user = new IdentityUser("email@email.com")
            {
                NewEmail = "new@new.com"
            };

            ConfirmEmailModel command = new ConfirmEmailModel()
            {
                Email = user.NewEmail,
                ConfirmationCode = "code"
            };

            Mock<IApplicationUserManager> mockUserManager = new Mock<IApplicationUserManager>();
            mockUserManager.Setup(x => x.FindByEmailAsync(command.Email)).Returns(Task.FromResult(user));
            mockUserManager.Setup(x => x.ConfirmEmailAsync(user.Id, command.ConfirmationCode)).Returns(Task.FromResult(IdentityResult.Success));

            Mock<IUserLogger> mockUserLogger = new Mock<IUserLogger>();

            Mock<IUrlGenerator> mockUrlGenerator = new Mock<IUrlGenerator>();
            mockUrlGenerator.Setup(x => x.GenerateSiteUrl<AccountController>(c => c.Login(null))).Returns("http://localhost");

            Mock<IAuthenticationManager> mockAuthenticationManager = new Mock<IAuthenticationManager>();

            ConfirmEmailCommandHandler handler = new ConfirmEmailCommandHandler(mockUserLogger.Object, mockUserManager.Object, mockUrlGenerator.Object, mockAuthenticationManager.Object);
            CommandResult commandResult = await handler.ExecuteAsync(command);
            commandResult.Succeeded.Should().BeTrue();
            command.ModalDialog.Should().NotBeNull();

            user.Email.Should().Be(command.Email);

            mockUserManager.Verify(x => x.FindByEmailAsync(command.Email), Times.Once);
            mockUserManager.Verify(x => x.ConfirmEmailAsync(user.Id, command.ConfirmationCode), Times.Once);
            mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once);
            mockUserLogger.Verify(x => x.LogActionAsync(UserActions.ConfirmEmailSuccess, command.Email));
            mockAuthenticationManager.Verify(x => x.SignOut(), Times.Once);
        }
    }
}