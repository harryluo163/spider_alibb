using albbPassword.Models;
using spider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace albbPassword.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DataTable dt = DBHelper.ExecuteSelect("select * from LR_Demo_CPA2");
            HttpClient httpClient = new HttpClient();
            RegFunc rg = new RegFunc();
            //访问阿里巴巴登录页
            string content = httpClient.GetResponse("", "https://passport.alibaba.com/mini_login.htm?lang=zh_cn&appName=youmeng&appEntrance=default&styleType=auto&bizParams=&notLoadSsoView=true&notKeepLogin=false&isMobile=false&cssLink=https://passport.umeng.com/css/loginIframe.css&rnd=0.11412324061518286", "", "");

            string modulus = rg.GetStr(content, "name=\"modulus\"  value=\"", "\"");
            string exponent = rg.GetStr(content, "name=\"exponent\"  value=\"", "\" type");

            ViewBag.Data = dt;
            ViewBag.Datajson = rg.DataTableToJson(dt).ToString();
            ViewBag.modulus = modulus;
            ViewBag.exponent = exponent;

            return View();

        }


        public JsonResult update(string data)
        {
            RegFunc rg = new RegFunc();
            DataTable dt = rg.ToDataTable(data);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DBHelper.ExecuteNonQuery("update LR_Demo_CPA2 set F_QueryPwd2  = '" + dt.Rows[i]["F_QueryPwd2"] + "' where F_CPAId = '" + dt.Rows[i]["F_CPAId"] + "'");
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

    }



    class RegFunc
    {

        public DataTable ToDataTable(string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }

                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;
        }


        public string DataTableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                JsonString.Append("]");
            }
            return JsonString.ToString();
        }

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
}