using CICLatest.Contracts;
using CICLatest.Helper;
using CICLatest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Controllers
{
    public class InvoiceController : Controller
    {
        private IHostingEnvironment Environment;
        private readonly UserManager<UserModel> _userManager;
        public readonly IAppSettingsReader _appSettingsReader;
        public readonly IBlobStorageService _blobStorageService;

        public InvoiceController(IHostingEnvironment _environment, UserManager<UserModel> userManager
            , IAppSettingsReader appSettingsReader, IBlobStorageService blobStorageService)
        {
            Environment = _environment;
            _userManager = userManager;
            _appSettingsReader = appSettingsReader;
            _blobStorageService = blobStorageService;
        }

        public IActionResult Index()
        {
            List<FileModel> files = new List<FileModel>();
            string usr = _userManager.GetUserAsync(User).Result.CustNo;
            files = _blobStorageService.GetInvoices(usr);
                        
            return View(files);
        }

        public FileResult DownloadFile(string fileName, string cust)
        {
            string usr = _userManager.GetUserAsync(User).Result.CustNo;
            string tempPath = "Files/" + usr + "/";
            string path = Path.Combine(_appSettingsReader.Read("InvoicePath"), tempPath) + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}
