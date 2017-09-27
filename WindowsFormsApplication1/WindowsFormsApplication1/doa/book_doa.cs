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

namespace WindowsFormsApplication1.doa
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
    }
}
