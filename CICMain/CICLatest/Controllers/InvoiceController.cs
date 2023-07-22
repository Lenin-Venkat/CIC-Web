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

        public InvoiceController(IHostingEnvironment _environment, UserManager<UserModel> userManager)
        {
            Environment = _environment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<FileModel> files = new List<FileModel>();
            BlobStorageService b = new BlobStorageService();
            string usr = _userManager.GetUserAsync(User).Result.CustNo;
            files = b.GetInvoices(usr);
                        
            return View(files);
        }

        public FileResult DownloadFile(string fileName, string cust)
        {
            string usr = _userManager.GetUserAsync(User).Result.CustNo;
            string tempPath = "Files/" + usr + "/";
            string path = Path.Combine("https://cicdatastorage.blob.core.windows.net/invoices/", tempPath) + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}
