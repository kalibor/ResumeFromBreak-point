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
        
              var response = new ResumeFromBreakpointServer().DownloadFile(Request, @"C:\Users\sf104137\Desktop\新增資料夾\3acdfc44-b9a8-4589-9345-ec242758e6d9.tmp");

            return response;
        }
    }
}
