﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CritterHeroes.Web.Contracts.Storage;

namespace CritterHeroes.Web.DataProviders.Azure.Storage
{
    public class CritterPictureService : ICritterPictureService
    {
        private const string _blobName = "critters";
        private const bool isPrivate = false;

        private IAzureService _azureService;

        public CritterPictureService(IAzureService azureService)
        {
            this._azureService = azureService;
        }

        public string GetPictureUrl(int critterID, string filename)
        {
            return _azureService.CreateBlobUrl(GetBlobPath(critterID, filename));
        }

        public async Task GetPictureAsync(int critterID, string filename, Stream outputStream)
        {
            await _azureService.DownloadBlobAsync(GetBlobPath(critterID, filename), isPrivate, outputStream);
        }

        public async Task SavePictureAsync(Stream source, int critterID, string filename, string contentType)
        {
            await _azureService.UploadBlobAsync(GetBlobPath(critterID, filename), isPrivate, contentType, source);
        }

        private string GetBlobPath(int critterID, string filename)
        {
            return $"{_blobName}/{critterID}/{filename.ToLower()}";
        }
    }
}
