﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ZipNachWebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            
           // WebApiConfig.Register(GlobalConfiguration.Configuration);           
            GlobalConfiguration.Configure(WebApiConfig.Register);

        }
    }
}
