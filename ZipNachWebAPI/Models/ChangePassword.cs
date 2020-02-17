using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZipNachWebAPI.Models
{
    public class SubmitMandateImageInput
    {
        public string MandeteId { get; set; }
        public string UserId { get; set; }
        public string ImageBytes { get; set; }
        public string AppID { get; set; }
        public string MandeteIdRef { get; set; }

    }
    public class ChangePassword
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string AppId { get; set; }
    }
    public class ChangePasswordResponse
    {
        public string userId { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }
    public class SendEmail
    {

        public string emailId { get; set; }
        public string AppId { get; set; }

    }
    public class SendEmailResponse
    {
        public string message { get; set; }
        public string status { get; set; }
    }

}