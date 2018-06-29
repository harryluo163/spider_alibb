using System;
using System.Collections.Generic;
using System.Drawing;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace spider
{
    public class HttpClient
    {
        private readonly System.Net.Http.HttpClient _client;//只需要实例化一次，就可以保持多次请求
        private HttpClientHandler handler;

        private string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36";
        private string _acceptLanguage = "zh-CN,zh;q=0.8";
        private string _accept = "text/html, application/xhtml+xml, */*";
        private string _acceptEncoding = "gzip, deflate";
        private string _charset = "utf-8";

        public string UserAgent { set { ChangeHeaders("User-Agent", value); } }
        public string AcceptLanguage { set { ChangeHeaders("Accept-Language", value); } }
        public string Accept { set { ChangeHeaders("Accept", value); } }
        public string Cookie { get; set; }
        public string AcceptEncoding { set { ChangeHeaders("Accept-Encoding", value); } }
        public string ContentType { get; set; }
        public string Charset { set { ChangeHeaders("Accept-Charset", value); } }
        public string TagCode { get; set; }

        public string exception;

        private void ChangeHeaders(string name, string newHeaders)
        {
            _client.DefaultRequestHeaders.Remove(name);
            _client.DefaultRequestHeaders.Add(name, newHeaders);
        }

        public HttpClient()
        {
            ContentType= "application/x-www-form-urlencoded";
            handler = new HttpClientHandler()
            {
                UseCookies = true,
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip //设置自动解压
                //UseProxy = true,
                //Proxy = new WebProxy("218.64.147.211", 9000)
            };
            _client = new System.Net.Http.HttpClient(handler);
            _client.DefaultRequestHeaders.Add("Accept", _accept);
            _client.DefaultRequestHeaders.Add("Accept-Language", _acceptLanguage);
            _client.DefaultRequestHeaders.Add("Accept-Encoding", _acceptEncoding);
            _client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("Accept-Charset", _charset);
        }

        public HttpClient(string cookie)
        {
            ContentType = "application/x-www-form-urlencoded";
            if (!string.IsNullOrEmpty(cookie))
            {
                CookieContainer cookieContainer = new CookieContainer();
                string[] coos = cookie.Split('|');
                foreach (var c in coos)
                {
                    string key = c.Split('=')[0];
                    string value = c.Split('=')[1];
                    string domain = c.Split('=')[2];
                    var coo = new Cookie(key, value, "/", domain);
                    cookieContainer.Add(coo);   // 加入Cookie
                }

                handler = new HttpClientHandler()
                {
                    CookieContainer = cookieContainer,
                    AllowAutoRedirect = true,
                    UseCookies = true,
                    //UseProxy=true,
                    //Proxy = new WebProxy("218.64.147.211",9000)
                };
                _client = new System.Net.Http.HttpClient(handler);
            }
            else
            {
                handler = new HttpClientHandler()
                {
                    UseCookies = true,
                    AllowAutoRedirect = true,
                    //UseProxy = true,
                    //Proxy = new WebProxy("218.64.147.211", 9000)
                };
                _client = new System.Net.Http.HttpClient(handler);
            }
     
        }

        public bool useTagCode = false;

        public string GetResponse(string referer = "", string url = "", string method = "get", string data = "", string EnCode = "")
        {
            if (!string.IsNullOrEmpty(referer)) _client.DefaultRequestHeaders.Referrer = new Uri(referer);

            HttpContent content = new StringContent(data);

            content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);

            Uri uri = new Uri(url);

            //_client.DefaultRequestHeaders.Remove("Accept-Charset");
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", EnCode);
            HttpResponseMessage res = null;
            try
            {
                if (method.ToLower().Equals("get"))
                {
                    res =  _client.GetAsync(uri).Result;
                }
                else res =  _client.PostAsync(uri, content).Result;

                if (res != null && res.IsSuccessStatusCode)
                {
                    try
                    {
                        CookieCollection cookieCollection = handler.CookieContainer.GetCookies(uri);
                        var list = new List<string>();
                        if (cookieCollection.Count > 0)
                        {
                            for (int i = 0; i < cookieCollection.Count; i++)
                            {
                                list.Add(cookieCollection[i].Name + "=" + cookieCollection[i].Value + "=" + cookieCollection[i].Domain);
                            }
                            Cookie = string.Join("|", list);
                        }
                    }
                    catch (Exception ex)
                    {
                        exception = ex.Message;
                        return null;
                    }

                    referer = res.RequestMessage.RequestUri.ToString();

                    if (referer.Contains("/(") && referer.Contains(")/"))
                    {
                        useTagCode = true;
                        TagCode = referer.Remove(0, referer.IndexOf("(") + 1);//获取识别码
                        TagCode = TagCode.Remove(TagCode.IndexOf(")"), TagCode.Length - TagCode.IndexOf(")"));
                    }

                    //res.Content.Headers.ContentType.CharSet = EnCode;
                    return  res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    exception = res == null ? "服务器不响应" : Enum.GetName(typeof(HttpStatusCode), res.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                exception = ex.Message;
                return null;
            }
        }



        ~HttpClient()
        {
            _client.Dispose();
        }
    }
}
