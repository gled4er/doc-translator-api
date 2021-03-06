﻿using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using MicrosoftGraph.Services;
using Microsoft.WindowsAzure.Storage;

namespace DocumentManagement.Services
{
    /// <summary>
    /// Service for interacting with Azure Storage 
    /// </summary>
    public class StorageManagementService : IStorageManagementService
    {
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Constructor for <see cref="StorageManagementService"/>
        /// </summary>
        /// <param name="loggingService"><Instance of <see cref="ILoggingService"/></param>
        public StorageManagementService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// Download blob file to local file system
        /// </summary>
        /// <param name="storageContainerName">Name of storage container</param>
        /// <param name="storageFileName">Name of blob file</param>
        /// <returns></returns>
        public async Task<string> DownloadBlob(string storageContainerName, string storageFileName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(storageContainerName);
                container.CreateIfNotExists();
                var blockBlob = container.GetBlockBlobReference(storageFileName);
                await blockBlob.DownloadToFileAsync(blockBlob.Name, FileMode.Create);
                return blockBlob.Name;
            }
            catch(Exception ex)
            {
                _loggingService.Error("Error in StorageManagementService.DownloadBlob", ex);
                throw;
            }
        }
    }
}
