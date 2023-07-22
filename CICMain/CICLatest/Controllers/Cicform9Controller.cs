using CICLatest.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CICLatest.Controllers
{

    public class Cicform9Controller : Controller
    {
        //int cnt = 1;
        //int cntt = 1;
        Form9ViewModel form1Model = new Form9ViewModel();
        

        public IActionResult CicForms()
        {
            return View("~/Views/Cicforms/CicForm9.cshtml");
        }
       
    }




}


