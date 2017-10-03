using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ResumeFromBreakpointClientApp.ResumeFromBreakpoint
{
    class ResumeFromBreakpointClient
    {
        public ResumeFromBreakpointClient(string url, string filePath)
        {
            this._url = url;
            this._filePath = filePath;

        }

        public void Download()
        {
            FileStream fs = null;
            string fileName = "";
            string tempFileName = "暫存檔案";

            long startBytes = 0;

            FileInfo file = new FileInfo(Path.Combine(this._filePath, tempFileName));
            if (file.Exists)
            {
                fs = file.OpenWrite();
                startBytes = fs.Length;
                fs.Seek(startBytes, SeekOrigin.Current);


            }
            else
            {
                fs = file.Create();
            }

            try
            {
                using (var clien = new HttpClient())
                {
                    if (startBytes > 0)
                    {
                        clien.DefaultRequestHeaders.Range = new System.Net.Http.Headers.RangeHeaderValue(0, startBytes);

                    }


                    using (var response = clien.GetAsync(this._url).Result)
                    {

                     
                        using (var stream = response.Content.ReadAsStreamAsync().Result)
                        {
                            int bufferSize = 80 * 1040;
                            byte[] bufferArrray = new byte[bufferSize];
                            int readSize = stream.Read(bufferArrray, 0, bufferSize);

                            while (readSize > 0)
                            {

                                fs.Write(bufferArrray, 0, readSize);
                                readSize = stream.Read(bufferArrray, 0, bufferSize);
                            }
                            fileName = response.Content.Headers.ContentDisposition.FileName.Replace("\"", "");
                          
                        }
                
              


                    }
                }


            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
            finally
            {
                if (fs != null)
                {
                 
                    fs.Dispose();
                   
                }
                file.CopyTo(Path.Combine(file.DirectoryName, fileName));
                file.Delete();
            }



            //using (var response = client.GetAsync(this._url))
            //{
            //    var data = response.Result.Headers;
            //}

        }


        private string _url { get; set; }
        private string _filePath { get; set; }
    
    }
}
