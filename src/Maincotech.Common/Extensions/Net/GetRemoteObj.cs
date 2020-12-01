using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Net
{
    public class GetRemoteObj
    {
        #region 构造与析构函数

        ~GetRemoteObj()
        {
            Dispose();
        }

        #endregion 构造与析构函数

        #region IDisposable 成员

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable 成员

        #region 日期随机函数

        /**********************************
         * 函数名称:DateRndName
         * 功能说明:日期随机函数
         * 参    数:ra:随机数
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          Random ra = new Random();
         *          string s = o.DateRndName(ra);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 日期随机函数
        /// </summary>
        /// <param name="ra">随机数</param>
        /// <returns></returns>
        public string DateRndName(Random ra)
        {
            var d = DateTime.Now;
            string s = null, y, m, dd, h, mm, ss;
            y = d.Year.ToString();
            m = d.Month.ToString();
            if (m.Length < 2) m = "0" + m;
            dd = d.Day.ToString();
            if (dd.Length < 2) dd = "0" + dd;
            h = d.Hour.ToString();
            if (h.Length < 2) h = "0" + h;
            mm = d.Minute.ToString();
            if (mm.Length < 2) mm = "0" + mm;
            ss = d.Second.ToString();
            if (ss.Length < 2) ss = "0" + ss;
            s += y + m + dd + h + mm + ss;
            s += ra.Next(100, 999).ToString();
            return s;
        }

        #endregion 日期随机函数

        #region 取得文件后缀

        /**********************************
         * 函数名称:GetFileExtends
         * 功能说明:取得文件后缀
         * 参    数:filename:文件名称
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string url = @"http://www.zhaobus.net/images/zhaobus.jpg";
         *          string s = o.GetFileExtends(url);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 取得文件后缀
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public string GetFileExtends(string filename)
        {
            string ext = null;
            if (filename.IndexOf('.') > 0)
            {
                var fs = filename.Split('.');
                ext = fs[fs.Length - 1];
            }
            return ext;
        }

        #endregion 取得文件后缀

        #region 获取远程文件源代码

        /**********************************
         * 函数名称:GetRemoteHtmlCode
         * 功能说明:获取远程文件源代码
         * 参    数:Url:远程url
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string url = @"http://www.baidu.com";
         *          string s = o.GetRemoteHtmlCode(url);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 获取远程文件源代码
        /// </summary>
        /// <param name="url">远程url</param>
        /// <returns></returns>
        public string GetRemoteHtmlCode(string url, string charset)
        {
            /*
            string s = "";
            MSXML2.XMLHTTP _xmlhttp = new MSXML2.XMLHTTPClass();
            _xmlhttp.open("GET", Url, false, null, null);
            _xmlhttp.send("");
            if (_xmlhttp.readyState == 4)
            {
                s = System.Text.Encoding.Default.GetString((byte[])_xmlhttp.responseBody);
            }
            return s;
            */

            Check.Assert(url.IsUrl(), "Invalid Url!");

            if (charset == "" || charset == null) charset = "gb2312";
            var output = "";
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                var res = (HttpWebResponse)req.GetResponse();
                var stream1 = res.GetResponseStream();
                var reader1 = new StreamReader(stream1, Encoding.GetEncoding(charset));
                output = reader1.ReadToEnd();
                stream1.Close();
                res.Close();
            }
            catch //(Exception ex)
            {
                throw;
            }

            return output;
        }

        #endregion 获取远程文件源代码

        #region 保存远程文件

        /**********************************
         * 函数名称：RemoteSave
         * 功能说明：保存远程文件
         * 参    数：Url:远程url;Path:保存到的路径
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string s = "";
         *          string url = @"http://www.zhaobus.net/images/zhaobus.jpg";
         *          string path =Server.MapPath("Html/");
         *          s = o.RemoteSave(url,path);
         *          Response.Write(s);
         *          o.Dispose();
         * ******************************/

        /// <summary>
        /// 保存远程文件
        /// </summary>
        /// <param name="url">远程url</param>
        /// <param name="path">保存到的路径</param>
        /// <returns></returns>
        public string RemoteSave(string url, string charset, string path)
        {
            var src = GetRemoteHtmlCode(url, charset);
            var ra = new Random();
            var stringFileName = DateRndName(ra) + "." + GetFileExtends(url);
            var stringFilePath = path + stringFileName;
            /*

            MSXML2.XMLHTTP _xmlhttp = new MSXML2.XMLHTTPClass();
            _xmlhttp.open("GET", Url, false, null, null);
            _xmlhttp.send("");

            if (_xmlhttp.readyState == 4)
            */
            if (!src.IsNullOrEmpty())
            {
                if (File.Exists(stringFilePath))
                    File.Delete(stringFilePath);

                var fs = new FileStream(stringFilePath,

FileMode.CreateNew);
                var w = new BinaryWriter(fs);
                //w.Write((byte[])_xmlhttp.responseBody);
                w.Write(src);
                w.Close();
                fs.Close();
            }
            //else
            //    throw new Exception(_xmlhttp.statusText);

            return stringFileName;
        }

        #endregion 保存远程文件

        #region 替换网页中的换行和引号

        /**********************************
         * 函数名称:ReplaceEnter
         * 功能说明:替换网页中的换行和引号
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.zhaobus.net";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.ReplaceEnter(HtmlCode);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 替换网页中的换行和引号
        /// </summary>
        /// <param name="htmlCode">HTML源代码</param>
        /// <returns></returns>
        public string ReplaceEnter(string htmlCode)
        {
            var s = "";
            if (htmlCode == null || htmlCode == "")
                s = "";
            else
                s = htmlCode.Replace("\"", "");
            s = s.Replace("\r\n", "");
            return s;
        }

        #endregion 替换网页中的换行和引号

        #region 执行正则提取出值

        /**********************************
         * 函数名称:GetRegValue
         * 功能说明:执行正则提取出值
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.zhaobus.net";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.ReplaceEnter(HtmlCode);
         *          string Reg="<title>.+?</title>";
         *          string GetValue=o.GetRegValue(Reg,HtmlCode)
         *          Response.Write(GetValue);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 执行正则提取出值
        /// </summary>
        /// <param name="regexString">正则表达式</param>
        /// <param name="remoteStr">HtmlCode源代码</param>
        /// <returns></returns>
        public string GetRegValue(string regexString, string remoteStr)
        {
            var matchVale = "";
            var r = new Regex(regexString);
            var m = r.Match(remoteStr);
            if (m.Success)
            {
                matchVale = m.Value;
            }
            return matchVale;
        }

        #endregion 执行正则提取出值

        #region 替换HTML源代码

        /**********************************
         * 函数名称:RemoveHTML
         * 功能说明:替换HTML源代码
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.zhaobus.net";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.ReplaceEnter(HtmlCode);
         *          string Reg="<title>.+?</title>";
         *          string GetValue=o.GetRegValue(Reg,HtmlCode)
         *          Response.Write(GetValue);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 替换HTML源代码
        /// </summary>
        /// <param name="htmlCode">html源代码</param>
        /// <returns></returns>
        public string RemoveHtml(string htmlCode)
        {
            var matchVale = htmlCode;
            foreach (Match s in Regex.Matches(htmlCode, "<.+?>"))
            {
                matchVale = matchVale.Replace(s.Value, "");
            }
            return matchVale;
        }

        #endregion 替换HTML源代码

        #region 匹配页面的链接

        /**********************************
         * 函数名称:GetHref
         * 功能说明:匹配页面的链接
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.zhaobus.net";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.GetHref(HtmlCode);
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 获取页面的链接正则
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        public string GetHref(string htmlCode)
        {
            var matchVale = "";
            var reg = @"(h|H)(r|R)(e|E)(f|F) *= *('|"")?((\w|\\|\/|\.|:|-|_)+)('|""| *|>)?";
            foreach (Match m in Regex.Matches(htmlCode, reg))
            {
                matchVale += (m.Value).ToLower().Replace("href=", "").Trim() + "||";
            }
            return matchVale;
        }

        #endregion 匹配页面的链接

        #region 匹配页面的图片地址

        /**********************************
         * 函数名称:GetImgSrc
         * 功能说明:匹配页面的图片地址
         * 参    数:HtmlCode:html源代码;imgHttp:要补充的http.当比如:<img src="bb/x.gif">则要补充http://www.zhaobus.net/,当包含http信息时,则可以为空
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.zhaobus.net";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.GetImgSrc(HtmlCode,"http://www.zhaobus.net/");
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 匹配页面的图片地址
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <param name="imgHttp">要补充的http://路径信息</param>
        /// <returns></returns>
        public string GetImgSrc(string htmlCode, string imgHttp)
        {
            var matchVale = "";
            var reg = @"<img.+?>";
            foreach (Match m in Regex.Matches(htmlCode, reg))
            {
                matchVale += GetImg((m.Value).ToLower().Trim(), imgHttp) + "||";
            }
            return matchVale;
        }

        /// <summary>
        /// 匹配<img src="" />中的图片路径实际链接
        /// </summary>
        /// <param name="imgString"><img src="" />字符串</param>
        /// <returns></returns>
        public string GetImg(string imgString, string imgHttp)
        {
            var matchVale = "";
            var reg = @"src=.+\.(bmp|jpg|gif|png|)";
            foreach (Match m in Regex.Matches(imgString.ToLower(), reg))
            {
                matchVale += (m.Value).ToLower().Trim().Replace("src=", "");
            }
            return (imgHttp + matchVale);
        }

        #endregion 匹配页面的图片地址

        #region 替换通过正则获取字符串所带的正则首尾匹配字符串

        /**********************************
         * 函数名称:GetHref
         * 功能说明:匹配页面的链接
         * 参    数:HtmlCode:html源代码
         * 调用示例:
         *          GetRemoteObj o = new GetRemoteObj();
         *          string Url = @"http://www.zhaobus.net";
         *          strion HtmlCode = o.GetRemoteHtmlCode(Url);
         *          string s = o.RegReplace(HtmlCode,"<title>","</title>");
         *          Response.Write(s);
         *          o.Dispose();
         * ********************************/

        /// <summary>
        /// 替换通过正则获取字符串所带的正则首尾匹配字符串
        /// </summary>
        /// <param name="regValue">要替换的值</param>
        /// <param name="regStart">正则匹配的首字符串</param>
        /// <param name="regEnd">正则匹配的尾字符串</param>
        /// <returns></returns>
        public string RegReplace(string regValue, string regStart, string regEnd)
        {
            var s = regValue;
            if (regValue != "" && regValue != null)
            {
                if (regStart != "" && regStart != null)
                {
                    s = s.Replace(regStart, "");
                }
                if (regEnd != "" && regEnd != null)
                {
                    s = s.Replace(regEnd, "");
                }
            }
            return s;
        }

        #endregion 替换通过正则获取字符串所带的正则首尾匹配字符串
    }
}