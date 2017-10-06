using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ResumeFromBreakpointClientApp.ResumeFromBreakpoint
{
    public class ResumeFromBreakpointClient
    {
        public ResumeFromBreakpointClient(string url, string directory)
        {
            this._url = url;
            this._directory = directory;
        }

        public void Download()
        {
            DownloadContext downloadcontext = new DownloadContext(Path.Combine(_directory, tempFileName));

            try
            {
                using (var clien = new HttpClient())
                {
                    if (downloadcontext.StartBytes > 0)
                    {
                        clien.DefaultRequestHeaders.Range = new RangeHeaderValue(downloadcontext.StartBytes, null);
                    }
                    using (var response = clien.GetAsync(this._url, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).Result)
                    {
                        StartDownload(response, downloadcontext);

                        _fileName = downloadcontext.GetFileName(response.Content.Headers);
                    }
                }
                FinishedDownload(downloadcontext);
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
            finally
            {
                if (downloadcontext.CurrentStream!=null)
                {
                    downloadcontext.CurrentStream.Dispose();
                }
            }

        }

        #region 私有方法

        private void StartDownload(HttpResponseMessage response, DownloadContext downloadcontext)
        {
            using (var stream = response.Content.ReadAsStreamAsync().Result)
            {
                byte[] bufferArrray = new byte[_bufferSize];

                downloadcontext.SetFileSize(response.Content.Headers);
          
                int readSize = stream.Read(bufferArrray, 0, _bufferSize);

                while (readSize > 0)
                {
                    downloadcontext.StartBytes += readSize;
                    downloadcontext.CurrentStream.Write(bufferArrray, 0, readSize);

                    readSize = stream.Read(bufferArrray, 0, _bufferSize);
                    fileloading.Invoke(downloadcontext.StartBytes, (long)downloadcontext.FileSize);
                }
            }
        }


        private void FinishedDownload(DownloadContext context)
        {
            context.Dispose();
            FileReName();
            if (fileloaded != null)
            {
                fileloaded.Invoke();
            }
        }

        #region 檔名處理

        private void FileReName()
        {
            string dest = GetNoRepeatFileFullName();
            FileInfo file = new FileInfo(Path.Combine(_directory, tempFileName));
            file.MoveTo(dest);
        }


        private string GetNoRepeatFileFullName()
        {
  
            var FullName = Path.Combine(_directory, _fileName);
            FileInfo fileinfo = new FileInfo(FullName);

            string NoRepeatFullName = "";
            string templateName = string.Format("{0}(temp){1}", fileinfo.Name.Replace(fileinfo.Extension, ""), fileinfo.Extension);
            int index = 1;

            while (string.IsNullOrEmpty(NoRepeatFullName))
            {
                if (!fileinfo.Exists)
                {
                    NoRepeatFullName = fileinfo.FullName;
                    break;
                }
                else
                {
                    string fileName = templateName.Replace("(temp)", string.Format("({0})", index));
                    fileinfo = new FileInfo(Path.Combine(fileinfo.DirectoryName, fileName));
                    index++;
                }
            }

            return NoRepeatFullName;

        }
        #endregion

        #endregion

        #region property

        private string _url { get; set; }
        private string _directory { get; set; }
        private string _fileName { get; set; }

        public onFileLoading fileloading { get; set; }
        public onFileLoaded fileloaded { get; set; }

        #endregion

        #region column

        readonly string tempFileName = "暫存檔案.tmp";
        readonly int _bufferSize = 1024 * 80;

        #endregion
        
    }


   

    public delegate void onFileLoading(long nowLength, long maxLength);
    public delegate void onFileLoaded();


}
