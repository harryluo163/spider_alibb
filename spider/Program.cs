using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace spider
{
    class Program
    {
        static void Main(string[] args)
        {

            LogHelper.WriteLog("开始抓取");
            string SpiderTime = DateTime.Now.ToString("yyyy-MM-dd");
            //获取所有账号

            DataTable dt = DBHelper.ExecuteSelect("select * from LR_Demo_CPA2");
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                HttpClient httpClient = new HttpClient();

                LogHelper.WriteLog("访问阿里巴巴登录页");
                string content = httpClient.GetResponse("", "https://passport.alibaba.com/mini_login.htm?lang=zh_cn&appName=youmeng&appEntrance=default&styleType=auto&bizParams=&notLoadSsoView=true&notKeepLogin=false&isMobile=false&cssLink=https://passport.umeng.com/css/loginIframe.css&rnd=0.11412324061518286", "", "");
                Thread.Sleep(1000);
                LogHelper.WriteLog("获取登录秘钥str");
                RegFunc rg = new RegFunc();
                var _csrf_token = rg.GetStr(content, "_csrf_token\" value=\"", "\" type=\"hidden");
                LogHelper.WriteLog(dt.Rows[i]["F_ProductName"] + "开始登陆");
                var loginjson = PostUrl("https://passport.alibaba.com/newlogin/login.do?fromSite=-2&appName=youmeng", "loginId=" + dt.Rows[i]["F_QueryName"] + "" +
                    "&password2=" + dt.Rows[i]["F_QueryPwd2"] + "" +
                    "&checkCode=&appName=youmeng&appEntrance=default&bizParams=" +
                    "&ua=" +
                     "&umidGetStatusVal=255&lrfcf=&lang=zh_CN&scene=&isMobile=false&screenPixel=1920x1080&navlanguage=zh-CN&navUserAgent=Mozilla%2F5.0+(Windows+NT+10.0%3B+WOW64)+AppleWebKit%2F537.36+(KHTML%2C+like+Gecko)+Chrome%2F58.0.3029.96+Safari%2F537.36&navAppVersion=&navPlatform=Win32&token=&nocAppKey=&csessionid=&sig=&captchaToken=" +
                     "&_csrf_token=" + _csrf_token + "");
                var str = rg.GetStr(loginjson, "\"st\":\"", "\"");
                Thread.Sleep(1000);
                if (!string.IsNullOrEmpty(str))
                {
                    LogHelper.WriteLog("获取登录秘钥str成功");

                    LogHelper.WriteLog("调用盟友登录");
                    var url = "https://passport.umeng.com/login/register?st=" + str + "&appId=umeng&redirectUrl=undefined";
                    var dat = httpClient.GetResponse(url, url, "GET", "", "");
                    Thread.Sleep(1000);
                    //获取cookie
                    HttpClient httpClient3 = new HttpClient(httpClient.Cookie);
                    var dat2 = httpClient3.GetResponse(url, "http://mobile.umeng.com/apps", "GET", "", "");
                    Thread.Sleep(1000);
                    LogHelper.WriteLog("获取登录后数据"); 
                    var datajson = httpClient3.GetResponse(dt.Rows[i]["F_QueryUrl"].ToString().Replace("/reports/installation", "") + "/reports/load_table_data?page=1&per_page=30&start_date=" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "&end_date=" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "&versions%5B%5D=&channels%5B%5D=&segments%5B%5D=&time_unit=daily&stats=installations", dt.Rows[i]["F_QueryUrl"].ToString().Replace("/reports/installation", "") + "/reports/load_table_data?page=1&per_page=30&start_date=" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "&end_date=" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "&versions%5B%5D=&channels%5B%5D=&segments%5B%5D=&time_unit=daily&stats=installations", "GET", "", "");
                    ArrayList list = rg.GetStrArr(datajson, "{", "}");
                    //date\":\"06-26\",\"data\":14,\"rate\":73.7"

                    if (list.Count > 0)
                    {
                        for (int n = 0; n < list.Count; n++)
                        {
                            string date = DateTime.Now.Year.ToString() + "-" + rg.GetStr(list[n].ToString(), "date\":\"", "\",\"data\"");
                            string data = rg.GetStr(list[n].ToString(), "data\":", ",\"rate\":");
                            string rate = rg.GetStr(list[n].ToString() + "$", ",\"rate\":", "$").Replace("$", "");
                            DBHelper.ExecuteNonQuery("delete LR_Demo_CPAData where F_CPAId='"+ dt.Rows[i]["F_CPAId"] + "' and F_NumDate ='"+ date + "'");
                            DBHelper.ExecuteNonQuery("insert into LR_Demo_CPAData (F_CPADataId,F_CPAId,F_ProductName,F_NumDate,F_NewNum,F_CreateDate) values('" + Guid.NewGuid().ToString() + "','" + dt.Rows[i]["F_CPAId"] + "','" + dt.Rows[i]["F_ProductName"] + "','" + date + "','" + data + "','" + DateTime.Now + "')");

                        }
                    }
                }
                else
                {
                    LogHelper.WriteLog("获取登录秘钥str失败");
                }

            }


            LogHelper.WriteLog("抓取结束");





        }


        private static string PostUrl(string url, string postData)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request.ProtocolVersion = HttpVersion.Version11;
                // 这里设置了协议类型。
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                request.KeepAlive = false;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }

            request.Method = "POST";    //使用get方式发送数据
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = null;
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Accept = "*/*";

            byte[] data = Encoding.UTF8.GetBytes(postData);
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            //获取网页响应结果
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string result = string.Empty;
            using (StreamReader sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
    }

    class RegFunc
    {
        public ArrayList GetStrArr(string pContent, string regBegKey, string regEndKey)
        {
            ArrayList arr = new ArrayList();
            string regular = "(?<={0})(.|\n)*?(?={1})";
            regular = string.Format(regular, regBegKey, regEndKey);
            Regex regex = new Regex(regular, RegexOptions.IgnoreCase);
            MatchCollection mc = regex.Matches(pContent);
            foreach (Match m in mc)
            {
                arr.Add(m.Value.Trim());
            }
            return arr;
        }

        public string GetStr(string pContent, string regBegKey, string regEndKey)
        {
            string regstr = "";
            string regular = "(?<={0})(.|\n)*?(?={1})";
            regular = string.Format(regular, regBegKey, regEndKey);
            Regex regex = new Regex(regular, RegexOptions.IgnoreCase);
            Match m = regex.Match(pContent);
            if (m.Length > 0)
            {
                regstr = m.Value.Trim();
            }
            return regstr;
        }

        readonly System.Web.Script.Serialization.JavaScriptSerializer _serializer = new JavaScriptSerializer();

        /**/
        /// <summary>  
        /// json 序列化  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="obj"></param>  
        /// <param name="propertys"></param>  
        /// <returns></returns>  
        public string Serialize<T>(T obj, List<string> propertys)
        {
            _serializer.RegisterConverters(new[] { new PropertyVariableConveter(typeof(T), propertys) });

            return _serializer.Serialize(obj);

        }
    }


    public class PropertyVariableConveter : JavaScriptConverter
    {
        private readonly List<Type> _supportedTypes = new List<Type>();

        public PropertyVariableConveter(Type supportedType, List<string> propertys)
        {
            _supportedTypes.Add(supportedType);
            Propertys = propertys;

        }


        private List<string> Propertys { get; set; }


        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {

            throw new Exception("  这个暂时不支持 ， 谢谢 ");

        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var dic = new Dictionary<string, object>();

            var t = obj.GetType();
            var properties = t.GetProperties();

            foreach (var ite in properties)
            {
                string key = ite.Name;
                var v = t.GetProperty(key).GetValue(obj, null);

                if (Propertys == null || Propertys.Count <= 0)
                {
                    dic.Add(key, v);
                    continue;
                }

                if (Propertys.Contains(key))
                {
                    dic.Add(key, v);
                }
            }

            return dic;

        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return _supportedTypes; }
        }



    }
}
