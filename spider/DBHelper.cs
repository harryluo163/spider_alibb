using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Data;//引入命名空间

namespace spider
{
    public sealed class DBHelper
    {
        //获取数据库连接字符串
        public static string connString = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

        /// <summary>
        /// 专门用来执行增、删、改的方法（非存储过程）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="para">参数数组</param>
        /// <returns>执行结果</returns>
        public static bool ExecuteNonQuery(string sql, params SqlParameter[] para)
        {
            return ExecuteNonQuery(sql, false, para);
        }

        /// <summary>
        /// 专门用来执行增、删、改的方法
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="isStoredProcedure">是否存储过程</param>
        /// <param name="para">参数</param>
        /// <returns>执行结果</returns>
        public static bool ExecuteNonQuery(string sql, bool isStoredProcedure, params SqlParameter[] para)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (isStoredProcedure)
                    {
                        //如果是存储过程
                        cmd.CommandType = CommandType.StoredProcedure;
                    }
                    if (para != null)
                    {
                        cmd.Parameters.AddRange(para);
                    }
                    //打开连接
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    int i = cmd.ExecuteNonQuery();
                    return i > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 此方法专门用来执行sql语句，并且返回一个DataTable对象（非存储过程）
        /// </summary>
        /// <param name="sql">参数化的sql语句(一般为含有select关键字的sql语句)</param>
        /// <param name="para">SqlParameter数组型的参数:如果此sql语句没有参数则para为null;否则在调用方传一个SqlParameter[]数组</param>
        /// <returns>DataTable格式的结果数据</returns>
        public static DataTable ExecuteSelect(string sql, params SqlParameter[] para)
        {
            return ExecuteSelect(sql, false, para);
        }

        /// <summary>
        /// 此方法专门用来执行sql语句，并且返回一个DataTable对象
        /// </summary>
        /// <param name="sql">参数化的sql语句(一般为含有select关键字的sql语句)</param>
        /// <param name="isStoredProcedure">标志要调用的是否是存储过程</param>
        /// <param name="para">SqlParameter数组型的参数:如果此sql语句没有参数则para为null;否则在调用方传一个SqlParameter[]数组</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteSelect(string sql, bool isStoredProcedure, params SqlParameter[] para)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, connString);
                if (isStoredProcedure)
                {
                    //如果是存储过程
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                }
                if (para != null)
                {
                    da.SelectCommand.Parameters.AddRange(para);
                }
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 用于查询的ExecuteReader方法（不带存储过程的）
        /// </summary>
        /// <param name="strSql">查询的SQL语句</param>
        /// <param name="para">字符串格式化</param>
        /// <returns>返回SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] para)
        {
            return ExecuteReader(sql, false, para);
        }

        /// <summary>
        /// 用于查询的ExecuteReader方法（带存储过程的）
        /// </summary>
        /// <param name="strSql">查询的SQL语句</param>
        /// <param name="para">字符串格式化</param>
        /// <returns>返回SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string sql, bool isStoredProcedure, params SqlParameter[] para)
        {
            SqlDataReader reader = null;
            SqlConnection sqlConn = new SqlConnection(connString);
            try
            {
                SqlCommand sqlComm = new SqlCommand(sql, sqlConn);
                if (isStoredProcedure)
                {
                    //如果是存储过程
                    sqlComm.CommandType = CommandType.StoredProcedure;
                }
                if (para != null)
                {
                    sqlComm.Parameters.AddRange(para);
                }
                //打开连接
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                reader = sqlComm.ExecuteReader();
                return reader;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 用于统计数据
        /// </summary>
        /// <param name="strSql">查询语句</param>
        /// <param name="para">参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, params SqlParameter[] para)
        {

            try
            {
                SqlConnection sqlconn = new SqlConnection(connString);
                SqlCommand sqlcomm = new SqlCommand(sql, sqlconn);
                if (para != null)
                {
                    sqlcomm.Parameters.AddRange(para);
                }
                //打开连接
                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }
                return sqlcomm.ExecuteScalar();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 返回DataTable对象（非存储过程）
        /// </summary>
        /// <param name="strSql">以Select语句开头的查询语句</param>
        /// <param name="para">参数</param>
        /// <returns>返回一个DataTable对象</returns>
        public static DataTable GetTable(string sql, params SqlParameter[] para)
        {
            return GetTable(sql, false, para);
        }

        /// <summary>
        /// 返回DataTable对象
        /// </summary>
        /// <param name="strSql">以Select语句开头的查询语句</param>
        /// <param name="para">参数</param>
        /// <returns>返回一个DataTable对象</returns>
        public static DataTable GetTable(string sql, bool isStoredProcedure, params SqlParameter[] para)
        {
            try
            {
                SqlDataAdapter sqlDA = new SqlDataAdapter(sql, connString);
                DataTable dt = new DataTable();
                //如果是存储过程
                if (isStoredProcedure)
                {
                    sqlDA.SelectCommand.CommandType = CommandType.StoredProcedure;
                }
                //如果参数化不为空
                if (para != null)
                {
                    sqlDA.SelectCommand.Parameters.AddRange(para);
                }
                sqlDA.Fill(dt);//如果这里出错一般就是SQL语句的错误
                return dt;
            }
            catch
            {
                throw;
            }
        }
    }
}