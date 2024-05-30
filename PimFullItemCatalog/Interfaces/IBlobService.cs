using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PimItemFullCatalog.Interfaces
{
    public  interface IBlobService
    {
        Task<List<string>> ListAllFilesAsync(string containerName);
        Task<Stream> DownloadFileAsync(string fileName, string containerName);
        Task<Guid> UploadFileAsync(Stream stream, string containerName, string filename, string contentType);
    }
}

