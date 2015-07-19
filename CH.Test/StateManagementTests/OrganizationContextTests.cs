﻿using System;
using System.Collections.Generic;
using System.Linq;
using CritterHeroes.Web.Common.StateManagement;
using CritterHeroes.Web.Data.Models;
using CritterHeroes.Web.Models;
using FluentAssertions;
using Microsoft.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.StateManagementTests
{
    [TestClass]
    public class OrganizationContextTests : BaseContextTest
    {
        [TestMethod]
        public void CanGetOrganizationContext()
        {
            OrganizationContext context = new OrganizationContext()
            {
                OrganizationID = Guid.NewGuid(),
                FullName = "Full",
                ShortName = "Short",
                AzureName = "Azure",
                LogoFilename = "Logo",
                EmailAddress = "email@email.com",
                SupportedCritters = GetTestSupportedSpecies()
            };

            StateSerializer serializer = new StateSerializer();

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies["CritterHeroes_Organization"] = serializer.Serialize(context);

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Request.Cookies).Returns(new RequestCookieCollection(cookies));

            OrganizationStateManager stateManager = new OrganizationStateManager(mockOwinContext.Object, serializer);
            OrganizationContext result = stateManager.GetContext();
            result.OrganizationID.Should().Be(context.OrganizationID);
            result.FullName.Should().Be(context.FullName);
            result.ShortName.Should().Be(context.ShortName);
            result.AzureName.Should().Be(context.AzureName);
            result.LogoFilename.Should().Be(context.LogoFilename);
            result.EmailAddress.Should().Be(context.EmailAddress);
            result.SupportedCritters.Should().Equal(context.SupportedCritters);

            mockOwinContext.Verify(x => x.Request.Cookies, Times.Once);
        }

        [TestMethod]
        public void CanSaveOrganizationContext()
        {
            OrganizationContext context = new OrganizationContext()
            {
                OrganizationID = Guid.NewGuid(),
                FullName = "Full",
                ShortName = "Short",
                AzureName = "Azure",
                LogoFilename = "Logo",
                EmailAddress = "email@email.com",
                SupportedCritters = GetTestSupportedSpecies()
            };

            Dictionary<string, string[]> cookies = new Dictionary<string, string[]>();

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Response.Cookies).Returns(new ResponseCookieCollection(new HeaderDictionary(cookies)));

            OrganizationStateManager stateManager = new OrganizationStateManager(mockOwinContext.Object, new StateSerializer());
            stateManager.SaveContext(context);

            cookies.Should().HaveCount(1);
            cookies.First().Value[0].Should().Contain("CritterHeroes_Organization");

            mockOwinContext.Verify(x => x.Response.Cookies, Times.Once);
        }

        [TestMethod]
        public void CanCreateItselfFromOrganization()
        {
            Organization organization = new Organization()
            {
                FullName = "Full",
                ShortName = "Short",
                AzureName = "Azure",
                LogoFilename = "Logo",
                EmailAddress = "email@email.com",
                SupportedCritters = GetTestSupportedSpecies()
            };

            OrganizationContext context = OrganizationContext.FromOrganization(organization);

            context.OrganizationID.Should().Be(organization.ID);
            context.FullName.Should().Be(organization.FullName);
            context.ShortName.Should().Be(organization.ShortName);
            context.AzureName.Should().Be(organization.AzureName);
            context.LogoFilename.Should().Be(organization.LogoFilename);
            context.EmailAddress.Should().Be(organization.EmailAddress);
            context.SupportedCritters.Should().Equal(organization.SupportedCritters);
        }

        [TestMethod]
        public void StateManagerReturnsNullIfFullNameIsMissing()
        {
            OrganizationContext context = new OrganizationContext()
            {
                OrganizationID = Guid.NewGuid(),
                ShortName = "Short",
                AzureName = "Azure",
                LogoFilename = "Logo",
                EmailAddress = "email@email.com",
                SupportedCritters = GetTestSupportedSpecies(),

                FullName = null
            };

            StateSerializer serializer = new StateSerializer();

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies["CritterHeroes.Organization"] = serializer.Serialize(context);

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Request.Cookies).Returns(new RequestCookieCollection(cookies));

            OrganizationStateManager stateManager = new OrganizationStateManager(mockOwinContext.Object, serializer);
            stateManager.GetContext().Should().BeNull();
        }

        [TestMethod]
        public void StateManagerReturnsNullIfShortNameIsMissing()
        {
            OrganizationContext context = new OrganizationContext()
            {
                OrganizationID = Guid.NewGuid(),
                FullName = "Full",
                AzureName = "Azure",
                LogoFilename = "Logo",
                EmailAddress = "email@email.com",
                SupportedCritters = GetTestSupportedSpecies(),

                ShortName = null
            };

            StateSerializer serializer = new StateSerializer();

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies["CritterHeroes.Organization"] = serializer.Serialize(context);

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Request.Cookies).Returns(new RequestCookieCollection(cookies));

            OrganizationStateManager stateManager = new OrganizationStateManager(mockOwinContext.Object, serializer);
            stateManager.GetContext().Should().BeNull();
        }

        [TestMethod]
        public void StateManagerReturnsNullIfAzureNameIsMissing()
        {
            OrganizationContext context = new OrganizationContext()
            {
                OrganizationID = Guid.NewGuid(),
                FullName = "Full",
                ShortName = "Short",
                LogoFilename = "Logo",
                EmailAddress = "email@email.com",
                SupportedCritters = GetTestSupportedSpecies(),

                AzureName = null
            };

            StateSerializer serializer = new StateSerializer();

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies["CritterHeroes.Organization"] = serializer.Serialize(context);

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Request.Cookies).Returns(new RequestCookieCollection(cookies));

            OrganizationStateManager stateManager = new OrganizationStateManager(mockOwinContext.Object, serializer);
            stateManager.GetContext().Should().BeNull();
        }

        [TestMethod]
        public void StateManagerReturnsNullIfSupportedCrittersNameIsMissing()
        {
            OrganizationContext context = new OrganizationContext()
            {
                OrganizationID = Guid.NewGuid(),
                FullName = "Full",
                ShortName = "Short",
                AzureName = "Azure",
                LogoFilename = "Logo",
                EmailAddress = "email@email.com",
                SupportedCritters = new Species[] { }
            };

            StateSerializer serializer = new StateSerializer();

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies["CritterHeroes.Organization"] = serializer.Serialize(context);

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Request.Cookies).Returns(new RequestCookieCollection(cookies));

            OrganizationStateManager stateManager = new OrganizationStateManager(mockOwinContext.Object, serializer);
            stateManager.GetContext().Should().BeNull();
        }

        [TestMethod]
        public void StateManagerReturnsNullIEmailAddressIsMissing()
        {
            OrganizationContext context = new OrganizationContext()
            {
                OrganizationID = Guid.NewGuid(),
                FullName = "Full",
                ShortName = "Short",
                LogoFilename = "Logo",
                AzureName = "Azure",
                SupportedCritters = GetTestSupportedSpecies(),

                EmailAddress = null
            };

            StateSerializer serializer = new StateSerializer();

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies["CritterHeroes.Organization"] = serializer.Serialize(context);

            Mock<IOwinContext> mockOwinContext = new Mock<IOwinContext>();
            mockOwinContext.Setup(x => x.Request.Cookies).Returns(new RequestCookieCollection(cookies));

            OrganizationStateManager stateManager = new OrganizationStateManager(mockOwinContext.Object, serializer);
            stateManager.GetContext().Should().BeNull();
        }
    }
}
