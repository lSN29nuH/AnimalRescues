﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CH.Domain.Contracts.Dashboard;
using CH.Domain.Contracts.Storage;
using CH.Domain.Models.Data;
using CH.Domain.Models.Json;
using CH.Domain.Models.Status;
using CH.Domain.Services.Queries;
using CH.Domain.Services.QueryHandlers.Dashboard;
using CH.Domain.StateManagement;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CH.Test.Models
{
    [TestClass]
    public class DataStatusQueryHandlerTests
    {
        [TestMethod]
        public async Task AnimalStatusQueryHandlerReturnsModel()
        {
            AnimalStatus[] sourceEntities = new AnimalStatus[] { new AnimalStatus("1", "Name1", "Status1"), new AnimalStatus("2", "Name2", "Status2") };
            AnimalStatus[] targetEntities = new AnimalStatus[] { new AnimalStatus("2", "Name2", "Status2"), new AnimalStatus("3", "Name3", "Status3") };

            OrganizationContext orgContext = new OrganizationContext()
            {
            };

            Mock<IStorageSource> mockSourceSource = new Mock<IStorageSource>();
            mockSourceSource.Setup(x => x.ID).Returns(1);

            Mock<IStorageSource> mockTargetSource = new Mock<IStorageSource>();
            mockTargetSource.Setup(x => x.ID).Returns(2);

            DashboardStatusQuery<AnimalStatus> query = new DashboardStatusQuery<AnimalStatus>(mockTargetSource.Object, mockSourceSource.Object, orgContext);

            Mock<IMasterStorageContext<AnimalStatus>> mockMasterContext = new Mock<IMasterStorageContext<AnimalStatus>>();
            mockMasterContext.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(targetEntities.AsEnumerable<AnimalStatus>()));

            Mock<ISecondaryStorageContext<AnimalStatus>> mockSecondaryContext = new Mock<ISecondaryStorageContext<AnimalStatus>>();
            mockSecondaryContext.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(sourceEntities.AsEnumerable<AnimalStatus>()));

            AnimalStatusDashboardItemStatusQueryHandler handler = new AnimalStatusDashboardItemStatusQueryHandler(mockMasterContext.Object, mockSecondaryContext.Object);
            DashboardItemStatus model = await handler.RetrieveAsync(query);

            model.DataItemCount.Should().Be(3);

            StorageItem storageItem1 = model.SourceItem;
            storageItem1.Should().NotBeNull();
            storageItem1.InvalidCount.Should().Be(1);
            storageItem1.ValidCount.Should().Be(2);

            ValidateDataItem(storageItem1.Items.ElementAt(0), sourceEntities[0].Name, true);
            ValidateDataItem(storageItem1.Items.ElementAt(1), sourceEntities[1].Name, true);
            ValidateDataItem(storageItem1.Items.ElementAt(2), null, false);

            StorageItem storageItem2 = model.TargetItem;
            storageItem2.Should().NotBeNull();
            storageItem2.InvalidCount.Should().Be(1);
            storageItem2.ValidCount.Should().Be(2);

            ValidateDataItem(storageItem2.Items.ElementAt(1), targetEntities[0].Name, true);
            ValidateDataItem(storageItem2.Items.ElementAt(2), targetEntities[1].Name, true);
            ValidateDataItem(storageItem2.Items.ElementAt(0), null, false);

            mockMasterContext.Verify(x => x.GetAllAsync(), Times.Once);
            mockSecondaryContext.Verify(x => x.GetAllAsync(), Times.Once);
            mockSourceSource.Verify(x => x.ID, Times.AtLeastOnce);
            mockTargetSource.Verify(x => x.ID, Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task BreedQueryHandlerReturnsModel()
        {
            Breed[] sourceEntities = new Breed[] { new Breed("1", "Species1", "Breed1"), new Breed("2", "Species2", "Breed2") };
            Breed[] targetEntities = new Breed[] { new Breed("2", "Species2", "Breed2"), new Breed("3", "Species3", "Breed3") };

            OrganizationContext orgContext = new OrganizationContext()
            {
                SupportedCritters = new Species[]
                {
                        new Species("Species1","1","2",null,null),
                        new Species("Species2","1","2",null,null),
                        new Species("Species3","1","2",null,null)
                }
            };

            Mock<IStorageSource> mockSourceSource = new Mock<IStorageSource>();
            mockSourceSource.Setup(x => x.ID).Returns(1);

            Mock<IStorageSource> mockTargetSource = new Mock<IStorageSource>();
            mockTargetSource.Setup(x => x.ID).Returns(2);

            DashboardStatusQuery<Breed> query = new DashboardStatusQuery<Breed>(mockTargetSource.Object, mockSourceSource.Object, orgContext);

            Mock<IMasterStorageContext<Breed>> mockMasterContext = new Mock<IMasterStorageContext<Breed>>();
            mockMasterContext.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(targetEntities.AsEnumerable<Breed>()));

            Mock<ISecondaryStorageContext<Breed>> mockSecondaryContext = new Mock<ISecondaryStorageContext<Breed>>();
            mockSecondaryContext.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(sourceEntities.AsEnumerable<Breed>()));

            BreedDashboardItemStatusQueryHandler handler = new BreedDashboardItemStatusQueryHandler(mockMasterContext.Object, mockSecondaryContext.Object);
            DashboardItemStatus model = await handler.RetrieveAsync(query);

            model.DataItemCount.Should().Be(3);

            StorageItem storageItem1 = model.SourceItem;
            storageItem1.Should().NotBeNull();
            storageItem1.InvalidCount.Should().Be(1);
            storageItem1.ValidCount.Should().Be(2);

            ValidateDataItem(storageItem1.Items.ElementAt(0), sourceEntities[0].Species + " - " + sourceEntities[0].BreedName, true);
            ValidateDataItem(storageItem1.Items.ElementAt(1), sourceEntities[1].Species + " - " + sourceEntities[1].BreedName, true);
            ValidateDataItem(storageItem1.Items.ElementAt(2), null, false);

            StorageItem storageItem2 = model.TargetItem;
            storageItem2.Should().NotBeNull();
            storageItem2.InvalidCount.Should().Be(1);
            storageItem2.ValidCount.Should().Be(2);

            ValidateDataItem(storageItem2.Items.ElementAt(1), targetEntities[0].Species + " - " + targetEntities[0].BreedName, true);
            ValidateDataItem(storageItem2.Items.ElementAt(2), targetEntities[1].Species + " - " + targetEntities[1].BreedName, true);
            ValidateDataItem(storageItem2.Items.ElementAt(0), null, false);

            mockMasterContext.Verify(x => x.GetAllAsync(), Times.Once);
            mockSecondaryContext.Verify(x => x.GetAllAsync(), Times.Once);
            mockSourceSource.Verify(x => x.ID, Times.AtLeastOnce);
            mockTargetSource.Verify(x => x.ID, Times.AtLeastOnce);
        }

        private void ValidateDataItem(DataItem dataItem, string expectedValue, bool isValid)
        {
            dataItem.Value.Should().Be(expectedValue);
            dataItem.IsValid.Should().Be(isValid);
        }
    }
}