using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZipNachWebAPI.Models
{
    public class Login
    {
        public string emailId { get; set; }
        public string password { get; set; }
        public string AppId { get; set; }
    }
    public class LoginResponsee
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }
}