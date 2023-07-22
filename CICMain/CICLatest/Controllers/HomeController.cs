using CICLatest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CICLatest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache memoryCache;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            ListCache();
            memoryCache.Remove("Form1");
            memoryCache.Remove("Form3");
            memoryCache.Remove("Form4");
            memoryCache.Remove("Form6");
            memoryCache.Remove("Form7");
            memoryCache.Remove("Form5");
            memoryCache.Remove("Form8");

            memoryCache.Remove("Form9");
            return View();
        }

        public void ListCache()
        {
            PropertyInfo prop = memoryCache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            object innerCache = prop.GetValue(memoryCache);
            MethodInfo clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            clearMethod.Invoke(innerCache, null);
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
