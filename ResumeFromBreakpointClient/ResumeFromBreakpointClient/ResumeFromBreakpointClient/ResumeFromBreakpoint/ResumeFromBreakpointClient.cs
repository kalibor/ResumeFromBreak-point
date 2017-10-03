using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
                        clien.DefaultRequestHeaders.Range = new System.Net.Http.Headers.RangeHeaderValue(startBytes, null);
                    }

                    using (var response = clien.GetAsync(this._url, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).Result)
                    {
                        using (var stream = response.Content.ReadAsStreamAsync().Result)
                        {
                            int bufferSize = 80 * 1040;
                            byte[] bufferArrray = new byte[bufferSize];

                            var contentRange = response.Content.Headers.ContentRange;
                            long? maxLength = 0;
                            long? nowLength =0;
                            if (contentRange!=null)
                            {
                                maxLength = contentRange.To;
                                nowLength = contentRange.From;
                            }
                            else
                            {
                                maxLength = response.Content.Headers.ContentLength;
                            }
                         
                           
                            int readSize = stream.Read(bufferArrray, 0, bufferSize);

                            while (readSize > 0)
                            {
                                nowLength += readSize;
                                fs.Write(bufferArrray, 0, readSize);
                                readSize = stream.Read(bufferArrray, 0, bufferSize);

                                if (s != null)
                                {
                                    s.Invoke((long)nowLength, (long)maxLength);
                                }
                            }


                            if (response.Content.Headers.ContentDisposition != null)
                            {
                                fileName = response.Content.Headers.ContentDisposition.FileName.Replace("\"", "");

                            }
                            else
                            {
                                fileName = Guid.NewGuid().ToString();
                            }

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

        }


        private string _url { get; set; }
        private string _filePath { get; set; }
        public onFileLoad s { get; set; }

        public delegate void onFileLoad(long nowLength, long maxLength);
    }

}
