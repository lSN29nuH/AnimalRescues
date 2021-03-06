﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CH.Test.Mocks;
using CritterHeroes.Web.Data.Models;
using CritterHeroes.Web.Domain.Contracts.Configuration;
using CritterHeroes.Web.Domain.Contracts.StateManagement;
using CritterHeroes.Web.Domain.Contracts.Storage;
using CritterHeroes.Web.Features.Admin.Organizations.Commands;
using CritterHeroes.Web.Features.Admin.Organizations.Models;
using CritterHeroes.Web.Shared.Commands;
using CritterHeroes.Web.Shared.StateManagement;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.AdminOrganizationTests
{
    [TestClass]
    public class EditProfileCommandHandlerTests : BaseTest
    {
        [TestMethod]
        public async Task UpdatesOrganizationFromModel()
        {
            Organization org = new Organization(Guid.NewGuid());

            EditProfileModel model = new EditProfileModel()
            {
                Name = "New Name",
                ShortName = "New Short Name",
                Email = "new@new.com"
            };

            Mock<IAppConfiguration> mockAppConfiguration = new Mock<IAppConfiguration>();
            mockAppConfiguration.SetupGet(x => x.OrganizationID).Returns(org.ID);

            MockSqlCommandStorageContext<Organization> mockStorageContext = new MockSqlCommandStorageContext<Organization>(org);

            Mock<IOrganizationLogoService> mockLogoService = new Mock<IOrganizationLogoService>();
            Mock<IStateManager<OrganizationContext>> mockStateManager = new Mock<IStateManager<OrganizationContext>>();

            EditProfileCommandHandler handler = new EditProfileCommandHandler(mockAppConfiguration.Object, mockStorageContext.Object, mockLogoService.Object, mockStateManager.Object);
            CommandResult commandResult = await handler.ExecuteAsync(model);
            commandResult.Succeeded.Should().BeTrue();

            org.FullName.Should().Be(model.Name);
            org.ShortName.Should().Be(model.ShortName);
            org.EmailAddress.Should().Be(model.Email);

            mockStorageContext.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
