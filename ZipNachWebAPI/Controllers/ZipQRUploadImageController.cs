using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ZipNachWebAPI.Models;

namespace ZipNachWebAPI.Controllers
{
    public class ZipQRUploadImageController : ApiController
    {
        [Route("api/UploadMandate/NotSubmitMandateImage")]
        [HttpPost]
        public ResponseGetMobileNO uploadA4scan(SubmitMandateImageInput context)
        {
            ResponseGetMobileNO pathInfo = new ResponseGetMobileNO();
            if (context.AppID == "" || context.AppID == null)
            {
                pathInfo.message = "Incomplete data";
                pathInfo.status = "Failure";
                return pathInfo;
            }
            else if (context.MandeteId == "" || context.MandeteId == null)
            {
                pathInfo.message = "Incomplete data";
                pathInfo.status = "Failure";
                return pathInfo;
            }

            else if (context.UserId == "" || context.UserId == null)
            {
                pathInfo.message = "Incomplete data";
                pathInfo.status = "Failure";
                return pathInfo;
            }
            //else if (context.ImageBytes == "" || context.ImageBytes == null)
            //{
            //    pathInfo.message = "Incomplete data";
            //    pathInfo.status = "Failure";
            //    return pathInfo;
            //}
            //else if (context.ImageBytes.Length < 0 || context.ImageBytes == null)
            //{
            //    pathInfo.message = "Incomplete data";
            //    pathInfo.status = "Failure";
            //    return pathInfo;
            //}
            if (CheckMandateInfo.CheckMandateId(context.AppID, context.MandeteId) != "1")
            {
                pathInfo.message = "Invalid MandateId";
                pathInfo.status = "Failure";
                return pathInfo;
            }
            else
            {
                pathInfo.message = "Image uploaded successfully";
                pathInfo.status = "Success";
                return pathInfo;
            }

        }
    }
}
