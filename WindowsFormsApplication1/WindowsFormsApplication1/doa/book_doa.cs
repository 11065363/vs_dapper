using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WindowsFormsApplication1.DB;
using WindowsFormsApplication1.model;
using XYS.Utils.Sys;

namespace WindowsFormsApplication1
{
    class book_doa
    {
        private readonly string ConnName = "sqlserver";
        //插入语句
        public void book_inp(Dictionary<string, object> paramDic) {
            string tmp = SystemInfo.GetConnString(ConnName);
            IDbConnection conn = new SqlConnection(tmp);//连接字符串
            string sql = SqlManager.GetSql(paramDic, "insert");//得到sql语句,修改这里
            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("sql语句为空！");
            }
            conn.Execute(sql, paramDic);
        }
        //删
        public void book_delete(Dictionary<string, object> paramDic) {
            string tmp = SystemInfo.GetConnString(ConnName);
            IDbConnection conn = new SqlConnection(tmp);//连接字符串
            string sql = SqlManager.DeleteSql(paramDic, "delete");//得到sql语句,修改这里
            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("sql语句为空！");
            }
            conn.Execute(sql, paramDic);
        }

        internal void book_update(Dictionary<string, object> paramDic)
        {
            string tmp = SystemInfo.GetConnString(ConnName);
            IDbConnection conn = new SqlConnection(tmp);//连接字符串
            string sql = SqlManager.DeleteSql(paramDic, "update");//得到sql语句,修改这里
            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("sql语句为空！");
            }
            conn.Execute(sql, paramDic);
        }
        //全部查询
        internal void book_select(Dictionary<string, object> paramDic)
        {
            string tmp = SystemInfo.GetConnString(ConnName);
            IDbConnection conn = new SqlConnection(tmp);//连接字符串
            string sql = SqlManager.DeleteSql(paramDic, "select_all");//得到sql语句,修改这里
            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("sql语句为空！");
            }
            List<book> list_book = new List<book>();
            list_book = conn.Query<book>(sql).ToList();
        }

        internal void book_select_single(Dictionary<string, object> paramDic)
        {
            string tmp = SystemInfo.GetConnString(ConnName);
            IDbConnection conn = new SqlConnection(tmp);//连接字符串
            string sql = SqlManager.DeleteSql(paramDic, "select_single");//得到sql语句,修改这里
            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("sql语句为空！");
            }
            book book = new book();
            book = conn.Query<book>(sql,paramDic).SingleOrDefault();
        }

        internal void book_select_more(Dictionary<string, object> paramDic)
        {
            string tmp = SystemInfo.GetConnString(ConnName);
            IDbConnection conn = new SqlConnection(tmp);//连接字符串
            string sql = SqlManager.DeleteSql(paramDic, "select_more");//得到sql语句,修改这里
            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("sql语句为空！");
            }
            List<book> list_book = new List<book>();
            list_book = conn.Query<book>(sql,paramDic).ToList();
        }
    }
}
