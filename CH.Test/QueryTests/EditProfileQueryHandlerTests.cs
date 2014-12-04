﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CH.Domain.Contracts;
using CH.Domain.Contracts.Identity;
using CH.Domain.Identity;
using CH.Domain.Queries;
using CH.Website.Models;
using CH.Website.Services.QueryHandlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.QueryTests
{
    [TestClass]
    public class EditProfileQueryHandlerTests
    {
        [TestMethod]
        public async Task EditProfileQueryHandlerReturnsViewModel()
        {
            UserQuery query = new UserQuery()
            {
                Username = "test.user"
            };

            Uri uriReferrer = new Uri("http://google.com");

            IdentityUser user = new IdentityUser(query.Username)
            {
                FirstName = "First",
                LastName = "Last",
                Email = "email@email.com"
            };

            Mock<IHttpContext> mockHttpContext = new Mock<IHttpContext>();
            mockHttpContext.Setup(x => x.Request.UrlReferrer).Returns(uriReferrer);

            Mock<IApplicationUserStore> mockUserStore = new Mock<IApplicationUserStore>();
            mockUserStore.Setup(x => x.FindByNameAsync(query.Username)).Returns(Task.FromResult(user));

            EditProfileViewModelQueryHandler handler = new EditProfileViewModelQueryHandler(mockHttpContext.Object, mockUserStore.Object);
            EditProfileModel model = await handler.Retrieve(query);

            model.Should().NotBeNull();
            model.Username.Should().Be(user.UserName);
            model.FirstName.Should().Be(user.FirstName);
            model.LastName.Should().Be(user.LastName);
            model.Email.Should().Be(user.Email);
            model.ReturnUrl.Should().Be(uriReferrer.AbsoluteUri);

            mockHttpContext.Verify(x => x.Request.UrlReferrer, Times.Once);
            mockUserStore.Verify(x => x.FindByNameAsync(query.Username), Times.Once);
        }
    }
}
