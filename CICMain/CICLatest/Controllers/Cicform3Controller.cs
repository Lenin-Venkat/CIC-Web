using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CICLatest.Controllers
{

    public class Cicform3Controller : Controller
    {
       
        public IActionResult CicForms()
        {
            return View("~/Views/Cicforms/CicForm3.cshtml");
        }
       
    }




}


