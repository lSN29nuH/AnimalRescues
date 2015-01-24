﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using CritterHeroes.Web.Areas.Account.CommandHandlers;
using CritterHeroes.Web.Areas.Account.Models;
using CritterHeroes.Web.Areas.Home;
using CritterHeroes.Web.Common.Commands;
using CritterHeroes.Web.Common.Identity;
using CritterHeroes.Web.Common.Notifications;
using CritterHeroes.Web.Common.StateManagement;
using CritterHeroes.Web.Contracts;
using CritterHeroes.Web.Contracts.Email;
using CritterHeroes.Web.Contracts.Identity;
using CritterHeroes.Web.Contracts.Notifications;
using CritterHeroes.Web.Models;
using CritterHeroes.Web.Models.Logging;
using FluentAssertions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.CommandTests
{
    [TestClass]
    public class ResetPasswordCommandHandlerTests
    {
        [TestMethod]
        public async Task ReturnsFailedIfUsernameIsInvalid()
        {
            ResetPasswordModel model = new ResetPasswordModel()
            {
                Username = "unit.test",
                Code = "code"
            };

            Mock<INotificationPublisher> mockNotificationPublisher = new Mock<INotificationPublisher>();
            mockNotificationPublisher.Setup(x => x.PublishAsync(It.IsAny<UserActionNotification>())).Returns<UserActionNotification>((notification) =>
            {
                notification.Action.Should().Be(UserActions.ResetPasswordFailure);
                notification.Username.Should().Be(model.Username);
                return Task.FromResult(0);
            });

            Mock<IApplicationUserManager> mockUserManager = new Mock<IApplicationUserManager>();
            mockUserManager.Setup(x => x.FindByNameAsync(model.Username)).Returns(Task.FromResult((IdentityUser)null));

            ResetPasswordCommandHandler handler = new ResetPasswordCommandHandler(mockNotificationPublisher.Object, null, mockUserManager.Object, null, null);
            CommandResult result = await handler.ExecuteAsync(model);
            result.Succeeded.Should().BeFalse();
            result.Errors[""].First().Should().Be("There was an error resetting your password. Please try again.");

            mockUserManager.Verify(x => x.FindByNameAsync(model.Username), Times.Once);
            mockNotificationPublisher.Verify(x => x.PublishAsync(It.IsAny<UserActionNotification>()), Times.Once);
        }

        [TestMethod]
        public async Task ReturnsFailedIfResetPasswordFails()
        {
            ResetPasswordModel model = new ResetPasswordModel()
            {
                Username = "unit.test",
                Code = "code",
                Password = "password"
            };

            IdentityUser user = new IdentityUser(model.Username);

            Mock<INotificationPublisher> mockNotificationPublisher = new Mock<INotificationPublisher>();
            mockNotificationPublisher.Setup(x => x.PublishAsync(It.IsAny<UserActionNotification>())).Returns<UserActionNotification>((notification) =>
            {
                notification.Action.Should().Be(UserActions.ResetPasswordFailure);
                notification.Username.Should().Be(model.Username);
                return Task.FromResult(0);
            });

            Mock<IApplicationUserManager> mockUserManager = new Mock<IApplicationUserManager>();
            mockUserManager.Setup(x => x.FindByNameAsync(model.Username)).Returns(Task.FromResult(user));
            mockUserManager.Setup(x => x.ResetPasswordAsync(user.Id, model.Code, model.Password)).Returns(Task.FromResult(IdentityResult.Failed("nope")));

            ResetPasswordCommandHandler handler = new ResetPasswordCommandHandler(mockNotificationPublisher.Object, null, mockUserManager.Object, null, null);
            CommandResult result = await handler.ExecuteAsync(model);
            result.Succeeded.Should().BeFalse();
            result.Errors[""].First().Should().Be("There was an error resetting your password. Please try again.");

            mockUserManager.Verify(x => x.FindByNameAsync(model.Username), Times.Once);
            mockUserManager.Verify(x => x.ResetPasswordAsync(user.Id, model.Code, model.Password), Times.Once);
            mockNotificationPublisher.Verify(x => x.PublishAsync(It.IsAny<UserActionNotification>()), Times.Once);
        }

        [TestMethod]
        public async Task ReturnsFailedIfLoginFails()
        {
            ResetPasswordModel model = new ResetPasswordModel()
            {
                Username = "unit.test",
                Code = "code",
                Password = "password"
            };

            IdentityUser user = new IdentityUser(model.Username);

            UserActions[] notificationActions = new UserActions[] { UserActions.PasswordLoginFailure, UserActions.ResetPasswordFailure };

            Mock<INotificationPublisher> mockNotificationPublisher = new Mock<INotificationPublisher>();
            mockNotificationPublisher.Setup(x => x.PublishAsync(It.IsAny<UserActionNotification>())).Returns<UserActionNotification>((notification) =>
            {
                notificationActions.Should().Contain(notification.Action);
                notification.Username.Should().Be(model.Username);
                return Task.FromResult(0);
            });

            Mock<IApplicationUserManager> mockUserManager = new Mock<IApplicationUserManager>();
            mockUserManager.Setup(x => x.FindByNameAsync(model.Username)).Returns(Task.FromResult(user));
            mockUserManager.Setup(x => x.ResetPasswordAsync(user.Id, model.Code, model.Password)).Returns(Task.FromResult(IdentityResult.Success));

            Mock<IApplicationSignInManager> mockSigninManager = new Mock<IApplicationSignInManager>();
            mockSigninManager.Setup(x => x.PasswordSignInAsync(model.Username, model.Password, false, false)).Returns(Task.FromResult(SignInStatus.Failure));

            ResetPasswordCommandHandler handler = new ResetPasswordCommandHandler(mockNotificationPublisher.Object, mockSigninManager.Object, mockUserManager.Object, null, null);
            CommandResult result = await handler.ExecuteAsync(model);
            result.Succeeded.Should().BeFalse();
            result.Errors[""].First().Should().Be("There was an error resetting your password. Please try again.");

            mockUserManager.Verify(x => x.FindByNameAsync(model.Username), Times.Once);
            mockUserManager.Verify(x => x.ResetPasswordAsync(user.Id, model.Code, model.Password), Times.Once);
            mockSigninManager.Verify(x => x.PasswordSignInAsync(model.Username, model.Password, false, false), Times.Once);
            mockNotificationPublisher.Verify(x => x.PublishAsync(It.IsAny<UserActionNotification>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ReturnsSuccessIfPasswordSuccessfullyResetAndLoginSucceeds()
        {
            ResetPasswordModel model = new ResetPasswordModel()
            {
                Username = "unit.test",
                Code = "code",
                Password = "password",

                OrganizationContext = new OrganizationContext()
                {
                    FullName = "FullName",
                    EmailAddress = "org@org.com"
                }
            };

            IdentityUser user = new IdentityUser(model.Username)
            {
                Email = "email@email.com"
            };

            UserActions[] notificationActions = new UserActions[] { UserActions.PasswordLoginSuccess, UserActions.ResetPasswordSuccess };

            EmailMessage emailMessage = null;

            Mock<INotificationPublisher> mockNotificationPublisher = new Mock<INotificationPublisher>();
            mockNotificationPublisher.Setup(x => x.PublishAsync(It.IsAny<UserActionNotification>())).Returns<UserActionNotification>((notification) =>
            {
                notificationActions.Should().Contain(notification.Action);
                notification.Username.Should().Be(model.Username);
                return Task.FromResult(0);
            });

            Mock<IApplicationUserManager> mockUserManager = new Mock<IApplicationUserManager>();
            mockUserManager.Setup(x => x.FindByNameAsync(model.Username)).Returns(Task.FromResult(user));
            mockUserManager.Setup(x => x.ResetPasswordAsync(user.Id, model.Code, model.Password)).Returns(Task.FromResult(IdentityResult.Success));

            Mock<IApplicationSignInManager> mockSigninManager = new Mock<IApplicationSignInManager>();
            mockSigninManager.Setup(x => x.PasswordSignInAsync(model.Username, model.Password, false, false)).Returns(Task.FromResult(SignInStatus.Success));

            Mock<IUrlGenerator> mockUrlGenerator = new Mock<IUrlGenerator>();
            mockUrlGenerator.Setup(x => x.GenerateSiteUrl<HomeController>(It.IsAny<Expression<Func<HomeController, ActionResult>>>())).Returns("account-url");

            Mock<IEmailClient> mockEmailClient = new Mock<IEmailClient>();
            mockEmailClient.Setup(x => x.SendAsync(It.IsAny<EmailMessage>())).Returns((EmailMessage msg) =>
            {
                emailMessage = msg;
                return Task.FromResult(0);
            });

            ResetPasswordCommandHandler handler = new ResetPasswordCommandHandler(mockNotificationPublisher.Object, mockSigninManager.Object, mockUserManager.Object, mockUrlGenerator.Object, mockEmailClient.Object);
            CommandResult result = await handler.ExecuteAsync(model);
            result.Succeeded.Should().BeTrue();

            model.ModalDialog.Should().NotBeNull();
            model.ModalDialog.Text.Should().NotBeNullOrEmpty();
            model.ModalDialog.Buttons.Should().NotBeEmpty();
            model.ModalDialog.Buttons.Any(x => x.Url != null && x.Url.Contains("account-url")).Should().BeTrue();

            emailMessage.From.Should().Be(model.OrganizationContext.EmailAddress);
            emailMessage.To.Should().Contain(user.Email);
            emailMessage.Subject.Should().Contain(model.OrganizationContext.FullName);

            mockUserManager.Verify(x => x.FindByNameAsync(model.Username), Times.Once);
            mockUserManager.Verify(x => x.ResetPasswordAsync(user.Id, model.Code, model.Password), Times.Once);
            mockSigninManager.Verify(x => x.PasswordSignInAsync(model.Username, model.Password, false, false), Times.Once);
            mockNotificationPublisher.Verify(x => x.PublishAsync(It.IsAny<UserActionNotification>()), Times.Exactly(2));
            mockEmailClient.Verify(x => x.SendAsync(It.IsAny<EmailMessage>()), Times.Once);
        }
    }
}
