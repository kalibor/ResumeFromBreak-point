using ResumeFrombreak_point.Models.ResumeFromBreakpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ResumeFrombreak_point.Controllers
{
    public class HomeController : ApiController
    {

        public HttpResponseMessage Get()
        {
            var response = new ResumeFromBreakpointServer().DownloadFile(Request, @"C:\Users\sf104137\Desktop\新增資料夾 (2)\OfficialWebsite.7z");

            return response;
        }
    }
}
