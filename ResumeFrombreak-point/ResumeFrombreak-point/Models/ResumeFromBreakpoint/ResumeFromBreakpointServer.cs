using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ResumeFrombreak_point.Models.ResumeFromBreakpoint
{
    public class ResumeFromBreakpointServer
    {

        public HttpResponseMessage DownloadFile(HttpRequestMessage request,string filePath )
        {
            FileInfo file = new FileInfo(filePath);
            HttpResponseMessage response = new HttpResponseMessage();

            if (!file.Exists)
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.Content = new StringContent("找不到此檔案");
                return response;
            }

            string fileName = file.Name;
            long fileLength = file.Length;

            int bufferSize = 80*1024;
            byte[] bufferArray = new byte[bufferSize];

            FileStream fs = null;

            long startBytes = 0;

            try
            {
                fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);

                //有值為續傳，否則為普通下載
                if (request.Headers.Contains("Range") )
                {
                    response.StatusCode = System.Net.HttpStatusCode.PartialContent;
                    
                    //取得續傳檔案目前下載到的位置
                    string raqnge = request.Headers.GetValues("Range").First().Replace("bytes=","").Replace("-","");
                    startBytes = long.Parse(raqnge);
                }
         
                if (startBytes !=0)
                {
                    //格式
                    response.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}",startBytes,fileLength -1,fileLength));
                }

               

                //增加說明標籤
                response.Headers.Add("E-Tag",string.Format("filename:{0},filesize:{1}", fileName, fileLength));

                //告訴用戶端此Serve支援續傳功能
                response.Headers.Add("Accept-Ranges", "bytes");

          

                //設置串流起始位置
                fs.Seek(startBytes, SeekOrigin.Begin);
                StreamContent sc = new StreamContent(fs);


                //檔案長度
                sc.Headers.ContentLength = fileLength;

                //傳送的檔案內容類型，octet-stream表無類型 為單純的字節流，而瀏覽器處理字節流的方式就是下載
                sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                //向客戶端發送檔案時，提示用戶需要保存"",string.Format(";filename={0}", )
                sc.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") {FileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) };

                sc.Headers.ContentEncoding.Add(System.Text.Encoding.UTF8.ToString());

                response.Content = sc;

                return response;
                /*
                 To

                 Do
                 */
            }
            catch(Exception e)
            {
                return null;
            }
            finally
            {
                if (fs !=null)
                {
                    //fs.Dispose();
                }
            }


        }

    }
}