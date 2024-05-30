using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using PimItemFullCatalog.Interfaces;
using Azure.Storage.Blobs.Models;
using System;

namespace PimItemFullCatalog.Services
{
    internal class BlobService:IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient ;
        
        public BlobService(BlobServiceClient blobServiceClient) 
        { 
            _blobServiceClient = blobServiceClient ;            
        }

        public async Task<Guid> UploadFileAsync(Stream stream, string containerName, string filename, string contentType)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var fileID = Guid.NewGuid();
            BlobClient blobClient = containerClient.GetBlobClient(fileID.ToString());

            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: default);

            return fileID;
        }

        public async Task<List<string>> ListAllFilesAsync(string containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var blobHierarchyItems = containerClient.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/");
            var blobNames = new List<string>();
            await foreach (var blobHierarchyItem in blobHierarchyItems)
            {
                //check if the blob is a virtual directory.
                if (blobHierarchyItem.IsPrefix)
                {
                    // You can also access files under nested folders in this way,
                    // of course you will need to create a function accordingly (you can do a recursive function)
                    // var prefix = blobHierarchyItem.Name;
                    // blobHierarchyItem.Name = "folderA\"
                    // var blobHierarchyItems= container.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", prefix);     
                }
                else
                {
                    blobNames.Add(blobHierarchyItem.Blob.Name);
                }
            }
            return blobNames;
        }

        public async Task<Stream> DownloadFileAsync(string fileName,string containerName)
        {
            //set up the service client conn in startup.
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            var retObj = await  blobClient.DownloadContentAsync();
            return retObj.Value.Content.ToStream();
        }
    }
}
