using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CICLatest.Contracts;
using CICLatest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Helper
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly AzureStorageConfiguration _azureConfig;
        public readonly IAppSettingsReader _appSettingsReader;

        public BlobStorageService(AzureStorageConfiguration azureConfig, IAppSettingsReader appSettingsReader)
        {
            _azureConfig = azureConfig;
            _appSettingsReader = appSettingsReader;
        }
        public string UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType, string path)
        {
            try
            {

                var _task = Task.Run(() => this.UploadFileToBlobAsync(strFileName, fileData, fileMimeType, path));
                _task.Wait();
                string fileUrl = _task.Result;                
                return Path.GetDirectoryName(fileUrl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void DeleteBlobData(string fileUrl)
        {
            try
            {
                Uri uriObj = new Uri(fileUrl);
                string BlobName = Path.GetFileName(uriObj.LocalPath);
                
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_azureConfig.StorageConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = "uploads";
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

                string pathPrefix = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/";
                CloudBlobDirectory blobDirectory = cloudBlobContainer.GetDirectoryReference(pathPrefix);
                // get block blob refarence  
                CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(BlobName);

                // delete blob from container      
                await blockBlob.DeleteAsync();
            }
            catch
            {
                //Do nothing
            }
        }

        private string GenerateFileName(string fileName, string path)
        {
            string datetfoldername = "", RegNo ="";
            if(path!=null)
            {
                if (path.Contains(@"\"))
                {
                    String[] pathlist = path.Split(@"\");
                    if (pathlist[3] != null || pathlist[3] != "")
                    {
                        datetfoldername = pathlist[3];
                        RegNo = pathlist[4];
                    }

                }
                else
                {
                    datetfoldername = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd");
                    RegNo = path;
                }
            }
            

            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = datetfoldername + "/" + RegNo + "/" + fileName;
            return strFileName;
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType, string path)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_azureConfig.StorageConnectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = "uploads";
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
                // string fileName = strFileName;
                string fileName = this.GenerateFileName(strFileName, path);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (fileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DownloadBlob(string filepath)
        {
            string dateFolderName = "", RegNo = "";
            if (filepath != null)
            {
                string path = filepath;
                String[] pathlist = path.Split(@"\");
                dateFolderName = pathlist[3];
                RegNo = pathlist[4];

                //string folderName = @"C:\CICFiles\" + RegNo;
                string folderName = @"C:";
                CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(_azureConfig.StorageConnectionString);
                CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("uploads");

                string containerName = "uploads";

                //if (!Directory.Exists(folderName))
                //{
                //    Directory.CreateDirectory(folderName);
                //}
                folderName = folderName + @"\";

                BlobContainerClient containerClient = new BlobContainerClient(_azureConfig.StorageConnectionString, containerName);
                string fName = "";
                var blobs1 = containerClient.GetBlobs();
                foreach (BlobItem blobItem in blobs1)
                {
                    String[] strlist = blobItem.Name.Split("/");
                    if (strlist[0] == dateFolderName && strlist[1] == RegNo)
                    {
                        fName = strlist[2];
                        CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fName);
                        FileStream uploadFileStream = System.IO.File.OpenWrite(folderName + fName);
                        cloudBlockBlob.DownloadToStreamAsync(uploadFileStream);
                    }
                }
            }
        }

        public void DownloadBlob1(string filepath, string folderPath)
        {
            string dateFolderName = "", RegNo = "";
            if (filepath != null)
            {

                string path = filepath;
                String[] pathlist = path.Split(@"\");
                dateFolderName = pathlist[3];
                RegNo = pathlist[4];

                string folderName = folderPath + RegNo;
                CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(_azureConfig.StorageConnectionString);
                CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("uploads");

                string containerName = "uploads";

                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                folderName = folderName + @"\";

                BlobContainerClient containerClient = new BlobContainerClient(_azureConfig.StorageConnectionString, containerName);
                string fName = "";
                var blobs1 = containerClient.GetBlobs();
                foreach (BlobItem blobItem in blobs1)
                {
                    String[] strlist = blobItem.Name.Split("/");
                    if (strlist[0] == dateFolderName && strlist[1] == RegNo)
                    {
                        fName = strlist[2];
                        CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fName);
                        FileStream uploadFileStream = System.IO.File.OpenWrite(folderName + fName);
                        cloudBlockBlob.DownloadToStreamAsync(uploadFileStream);
                    }
                }
            }
        }

        public List<FileList> GetBlobList(string filepath)
        {
            List<FileList> filelist = new List<FileList>();

            if (filepath != null)
            {
                if (filepath.Contains("https"))
                {

                    string dateFolderName = "", RegNo = "";

                    if (filepath != null)
                    {
                        string path = filepath;
                        String[] pathlist = path.Split(@"\");
                        dateFolderName = pathlist[3];
                        RegNo = pathlist[4];

                        string folderName = @"C:\CICFiles\" + RegNo;
                        CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(_azureConfig.StorageConnectionString);
                        CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();
                        CloudBlobContainer container = blobClient.GetContainerReference("uploads");

                        string containerName = "uploads";

                        //if (!Directory.Exists(folderName))
                        //{
                        //    Directory.CreateDirectory(folderName);
                        //}
                        folderName = folderName + @"\";

                        BlobContainerClient containerClient = new BlobContainerClient(_azureConfig.StorageConnectionString, containerName);
                        string fName = "";
                        var blobs1 = containerClient.GetBlobs();

                        var sortedBlobs = GetSortedBlobs(blobs1);
                        foreach (BlobItem blobItem in sortedBlobs)
                        {
                            String[] strlist = blobItem.Name.Split("/");
                            if (strlist[0] == dateFolderName && strlist[1] == RegNo)
                            {
                                if(strlist[2]!= "Files")
                                {
                                    fName = strlist[2];
                                    String[] filename = fName.Split(new char[] { '_' }, 2);
                                    // String[] filename = fName.Split("_");
                                    filelist.Add(new FileList { FileKey = filename[0], FileValue = filename[1] });
                                }
                                                                

                            }
                        }

                        return filelist;
                    }
                }
            }
               
            return filelist;
        }

        private IOrderedEnumerable<BlobItem> GetSortedBlobs(Pageable<BlobItem> blobs1)
        {
            return blobs1.Select(item => item).OrderBy(item1 => item1.Properties.LastModified);
        }

        public List<FileModel> GetInvoices(string custno)
        {
            List<FileModel> filelist = new List<FileModel>();
            List<string> subs3 = new List<string>();
            BlobServiceClient blobServiceClient = new BlobServiceClient(_azureConfig.StorageConnectionString);
            string containerName = "invoices";
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = containerClient.GetBlobs(prefix: custno);
            string filepath = _appSettingsReader.Read("InvoicePath");

            foreach (var blob in blobs)
            {
                string[] sub_names = blob.Name.Split('/');
                
                if (sub_names.Length > 1 && !subs3.Contains(sub_names[sub_names.Length - 1]))
                {
                    //filepath = filepath + sub_names[sub_names.Length - 1];
                    //subs3.Add(sub_names[sub_names.Length - 1]);
                    filelist.Add(new FileModel { CustNo = custno, FileName = sub_names[sub_names.Length - 1],path= filepath + sub_names[0] + "/" + sub_names[sub_names.Length - 1] });
                }
                
            }
           
            return filelist;
        }

        public List<FileList> DownloadFile(string filepath, string filename)
        {
            string dateFolderName = "", RegNo = "";
            List<FileList> filelist = new List<FileList>();
            if (filepath != null)
            {
                string path = filepath;
                String[] pathlist = path.Split(@"\");
                dateFolderName = pathlist[3];
                RegNo = pathlist[4];

                string folderName = @"C:\CICFiles\" + RegNo;
                CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(_azureConfig.StorageConnectionString);
                CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("uploads");

                string containerName = "uploads";


                folderName = folderName + @"\";

                BlobContainerClient containerClient = new BlobContainerClient(_azureConfig.StorageConnectionString, containerName);

                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filename);
                FileStream uploadFileStream = System.IO.File.OpenWrite(folderName + filename);
                cloudBlockBlob.DownloadToStreamAsync(uploadFileStream);

                return filelist;
            }

            return filelist;
        }

        public List<FileList> DownloadPdf(string custno, string filename)
        {
            List<FileList> filelist = new List<FileList>();
            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(_azureConfig.StorageConnectionString);
            CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("invoices");

            string containerName = "invoices";

            string filepath = _appSettingsReader.Read("ImagePath")+custno + "/"+filename;

            BlobContainerClient containerClient = new BlobContainerClient(_azureConfig.StorageConnectionString, containerName);

            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filename);
            FileStream uploadFileStream = System.IO.File.OpenWrite(filepath);
            cloudBlockBlob.DownloadToStreamAsync(uploadFileStream);

            return filelist;
        }
    }
}
