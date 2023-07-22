using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class BCConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

