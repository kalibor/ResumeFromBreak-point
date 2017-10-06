using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ResumeFromBreakpointClientApp.ResumeFromBreakpoint
{
    public class DownloadContext : IDisposable
    {
        public DownloadContext(string FilePath)
        {
            FileInfo = new FileInfo(FilePath);

            if (FileInfo.Exists)
            {
                CurrentStream = FileInfo.OpenWrite();
                StartBytes = CurrentStream.Length;
                CurrentStream.Seek(StartBytes, SeekOrigin.Current);
            }
            else
            {
                CurrentStream = FileInfo.Create();
            }

        }

        public void SetFileSize(HttpContentHeaders headers)
        {
            if (headers.ContentRange != null)
            {
                FileSize = (long)headers.ContentRange.To;
            }
            else if (headers.ContentLength != null)
            {
                FileSize = (long)headers.ContentLength;
            }
            else
            {
                FileSize = 0;
            }
        }

        public string GetFileName(HttpContentHeaders headers)
        {
            var filename = "";

            if (headers.ContentDisposition != null)
            {
                filename = System.Net.WebUtility.UrlDecode(headers.ContentDisposition.FileName.Replace("\"", ""));
            }
            else
            {
                filename = Guid.NewGuid().ToString() + ".tmp";
            }

            return filename;
        }

        public void Dispose()
        {
            if (CurrentStream != null)
            {
                ((IDisposable)CurrentStream).Dispose();
            }

        }

        #region Property

        public FileInfo FileInfo { get; set; }
        public FileStream CurrentStream { get; set; }
        public long StartBytes { get; set; }
        public long FileSize { get; set; }
        
        #endregion


    }
}
