using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public static class HttpRequestFactory
    {

        /// <summary>
        /// 创建request
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static HttpRequest CreateHttpRequest(string msg, byte[] reqData)
        {
            try
            {
                var request = new HttpRequest();
                var splitIndex = msg.IndexOf("\r\n\r\n");
                var header = msg.Substring(0, splitIndex + 1);
                var body = msg.Substring(splitIndex + 4, msg.Length - splitIndex - 4);
                request.AnalyzeHeader(header);
                request.AnalyzeBody(body, splitIndex + 4, msg.Length - splitIndex - 4, reqData);

                request.AnalyzeHeader(GetEncodedStr(0, splitIndex + 1,reqData,Encoding.UTF8));

                request.FillInputStream(splitIndex + 4, msg.Length - splitIndex - 4, reqData);


                return request;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #region header
        /// <summary>
        /// 处理头部
        /// </summary>
        /// <param name="request"></param>
        /// <param name="header"></param>
        private static void AnalyzeHeader(this HttpRequest request, string header)
        {

            var lines = header.Split(new String[] { "\r\n" }, StringSplitOptions.None);
            request.AnalyzeRequstRow(lines[0]);

            //处理请求头部
            var headers = new NameValueCollection();
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var key = line.Substring(0, line.IndexOf(":"));
                var value = line.Substring(line.IndexOf(":") + 1, line.Length - line.IndexOf(":") - 1);
                headers.Add(key, value);
                if (key.ToUpper() == "HOST")
                {
                    request.Host = value;
                }
            }
            request.Headers = headers;
        }



        /// <summary>
        /// 解析请求行
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestRow"></param>
        private static void AnalyzeRequstRow(this HttpRequest request, string requestRow)
        {
            var rowInfo = requestRow.Split(' ');
            request.Method = rowInfo[0];
            request.Url = rowInfo[1];
            request.HttpVersion = rowInfo[2].Split('/')[1];
            //处理url上的参数
            try
            {
                var parameters = request.Url.Substring(request.Url.IndexOf("?") + 1, request.Url.Length - request.Url.IndexOf("?") - 1);
                var querystring = new NameValueCollection();
                foreach (var param in parameters.Split('&'))
                {
                    var paramInfo = param.Split('=');

                    if (!string.IsNullOrEmpty(querystring[paramInfo[0]]))
                        querystring[paramInfo[0]] = $"{querystring[paramInfo[0]]},{(paramInfo.Length == 2 ? paramInfo[1] : string.Empty)}";
                    else
                        querystring[paramInfo[0]] = $"{(paramInfo.Length == 2 ? paramInfo[1] : string.Empty)}";

                }
                request.Querystring = querystring;
            }
            catch (Exception ex)
            {
                //
            }

        }
        #endregion
        #region body
        /// <summary>
        /// 解析请求体
        /// </summary>
        /// <param name="request"></param>
        /// <param name="body"></param>
        private static void AnalyzeBody(this HttpRequest request, string body, int from, int to, byte[] reqData)
        {
            try
            {
                request.Form = new NameValueCollection();
                //文件上传
                if (request.Headers["Content-Type"].Contains("multipart/form-data"))
                {
                    request.AnalyzeFormDataBody(body, from, to, reqData);
                }
                else if (request.Headers["Content-Type"].Contains("x-www-form-urlencoded"))
                {
                    //stream.Seek(from, SeekOrigin.Begin);
                    //var buffer = new byte[to - from + 1];
                    //stream.Read(buffer, 0, buffer.Length);
                    //body = Encoding.UTF8.GetString(buffer);

                    body = GetEncodedStr(from,to, reqData, Encoding.UTF8);

                    foreach (var param in body.Split('&'))
                    {
                        var paramInfo = param.Split('=');

                        if (!string.IsNullOrEmpty(request.Form[paramInfo[0]]))
                            request.Form[paramInfo[0]] = $"{request.Form[paramInfo[0]]},{(paramInfo.Length == 2 ? paramInfo[1] : string.Empty)}";
                        else
                            request.Form[paramInfo[0]] = $"{(paramInfo.Length == 2 ? paramInfo[1] : string.Empty)}";
                    }

                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 解析文件表单数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="body"></param>
        private static void AnalyzeFormDataBody(this HttpRequest request, string body, int from, int to, byte[] reqData)
        {
            try
            {
                var boundary = request.Headers["Content-Type"].Split(';')[1].Trim().Split('=')[1];
                var files = new List<HttpRequestFile>();
                var splitWord = $"--{boundary}";
                foreach (var item in body.Split(new String[] { splitWord }, StringSplitOptions.None))
                {
                    if (string.IsNullOrEmpty(item)|| string.IsNullOrWhiteSpace(item))
                    {
                        from = from + item.Length;
                        continue;
                    }
                    from += splitWord.Length;

                    if (item.Trim() == "--") break;
                    var fileInfo = new HttpRequestFile();

                    var splitIndex = item.IndexOf("\r\n\r\n");
                    var formHeader = GetEncodedStr(from, from+ splitIndex, reqData, Encoding.UTF8);// item.Substring(0, splitIndex);
                    fileInfo.AnalyzeFormHeader(formHeader);
                    var formbody = item.Substring(splitIndex + 4, item.Length - splitIndex - 6);

                    
                    from += splitIndex + 4;
                    to = from + formbody.Length;
                    //有文件名的才当作文件看待
                    if (!string.IsNullOrEmpty(fileInfo.Filename))
                    {
                        fileInfo.AnalyzeFormBody(from, to, reqData);
                        files.Add(fileInfo);
                    }
                    else
                    {
                        body = GetEncodedStr(from, to,reqData, Encoding.UTF8);
                        request.Form.Add(fileInfo.Name, formbody);
                    }
                    from = to+2;
                }
                request.Files = files;
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 处理文件头部
        /// </summary>
        /// <param name="request"></param>
        /// <param name="formHeader"></param>
        private static void AnalyzeFormHeader(this HttpRequestFile fileInfo, string formHeader)
        {
            try
            {

                var lines = formHeader.Split(new String[] { "\r\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    var lineInfo = line.Split(':');
                    if (lineInfo[0] == "Content-Disposition")
                    {
                        foreach (var item in lineInfo[1].Split(';'))
                        {
                            var itemInfo = item.Split('=');
                            if (itemInfo[0].Trim().Equals("name"))
                                fileInfo.Name = item.Split('=')[itemInfo.Length - 1].Trim('"');
                            if (itemInfo[0].Trim().Equals("filename"))
                                fileInfo.Filename = item.Split('=')[itemInfo.Length - 1].Trim('"');
                        }
                    }
                    else if (lineInfo[0] == "Content-Type")
                    {
                        fileInfo.ContentType = lineInfo[1].Trim();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 解析表单内容
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="formBody"></param>
        private static void AnalyzeFormBody(this HttpRequestFile fileInfo, int from, int to, byte[] reqData)
        {
            try
            {

                var buffer = reqData.Skip(from).Take(to - from).ToArray();

                var fstream = new MemoryStream();
                fstream.Write(buffer, 0, buffer.Length);
                fstream.Flush();
                fstream.Position = 0;
                fileInfo.FileStream = fstream;
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 填充输入流
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="formBody"></param>
        private static void FillInputStream(this HttpRequest request, int from, int to, byte[] reqData)
        {
            try
            {
                var buffer = reqData.Skip(from).Take(to - from).ToArray();
                var inputStream = new MemoryStream();
                inputStream.Write(buffer, 0, buffer.Length);
                inputStream.Flush();
                inputStream.Position = 0;
                request.InputStream = inputStream;
            }
            catch (Exception)
            {
            }
        }


        #endregion

        private static string GetEncodedStr(int from,int to,byte[] data,Encoding toEncoding)
        {
            var buffer = data.Skip(from).Take(to - from).ToArray();
            return toEncoding.GetString( buffer);
        }
    }
}
