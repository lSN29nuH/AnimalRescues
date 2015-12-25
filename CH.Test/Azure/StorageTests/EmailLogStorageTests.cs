﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Contracts.Storage;
using CritterHeroes.Web.DataProviders.Azure;
using CritterHeroes.Web.DataProviders.Azure.Storage.Logging;
using CritterHeroes.Web.Models.Logging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;

namespace CH.Test.Azure.StorageTests
{
    [TestClass]
    public class EmailLogStorageTests : BaseAzureTest
    {
        [TestMethod]
        public async Task CanSaveAndRetrieveEmailLog()
        {
            TestEmailData testData = new TestEmailData()
            {
                Message = "message"
            };

            bool isPrivate = true;

            EmailLog emailLog = new EmailLog(testData);

            string emailData = null;

            Mock<IAzureService> mockAzureService = new Mock<IAzureService>();
            mockAzureService.Setup(x => x.UploadBlobAsync(It.IsAny<string>(), isPrivate, It.IsAny<string>())).Returns((string path, bool callbackIsPrivate, string content) =>
            {
                emailData = content;
                CloudBlockBlob blob = new CloudBlockBlob(new Uri("http://localhost/container"));
                return Task.FromResult(blob);
            });
            mockAzureService.Setup(x => x.DownloadBlobAsync(It.IsAny<string>(), isPrivate)).Returns(() => Task.FromResult(emailData));

            AzureEmailLogger logger = new AzureEmailLogger(new AzureConfiguration(), mockAzureService.Object);
            await logger.LogEmailAsync(emailLog);

            IEnumerable<EmailLog> results = await logger.GetEmailLogAsync(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1));

            EmailLog result = results.FirstOrDefault(x => x.ID == emailLog.ID);
            result.Should().NotBeNull();
            result.WhenCreatedUtc.Should().Be(emailLog.WhenCreatedUtc);

            TestEmailData resultData = result.ConvertEmailData<TestEmailData>();
            resultData.Message.Should().Be(testData.Message);

            mockAzureService.Verify(x => x.UploadBlobAsync(It.IsAny<string>(), isPrivate, It.IsAny<string>()), Times.Once);
            mockAzureService.Verify(x => x.DownloadBlobAsync(It.IsAny<string>(), isPrivate), Times.AtLeastOnce);
        }

        public class TestEmailData
        {
            public string Message
            {
                get;
                set;
            }
        }
    }
}
