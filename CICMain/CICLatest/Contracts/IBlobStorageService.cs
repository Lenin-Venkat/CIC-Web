using CICLatest.Models;
using System.Collections.Generic;

namespace CICLatest.Contracts
{
    public interface IBlobStorageService
    {
        string UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType, string path);
        void DeleteBlobData(string fileUrl);
        void DownloadBlob(string filepath);
        void DownloadBlob1(string filepath, string folderPath);
        List<FileList> GetBlobList(string filepath);
        List<FileModel> GetInvoices(string custno);
        List<FileList> DownloadFile(string filepath, string filename);
        List<FileList> DownloadPdf(string custno, string filename);

    }
}
