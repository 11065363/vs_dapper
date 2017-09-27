using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using XYS.Utils.Sys;

namespace WindowsFormsApplication1.DB
{
    class DBfactory:IDisposable
    {
        private string _connStr;
        private DbConnection _conn;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">连接名</param>
        public DBfactory(string name)
        {
            ConnStruct cs = SystemInfo.GetConnStruct(name);
            _connStr = cs.ConnStr;
            Factory = DbProviderFactories.GetFactory(cs.ProviderName);
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection Conn
        {
            get
            {
                if (_conn == null)
                {
                    _conn = Factory.CreateConnection();
                    if (string.IsNullOrEmpty(_connStr))
                    {
                        throw new Exception("连接字符串非法: }" + _connStr + "}");
                    }
                    _conn.ConnectionString = _connStr;
                }
                return _conn;
            }
        }
        /// <summary>
        /// 数据库驱动工厂
        /// </summary>
        public DbProviderFactory Factory { get; set; }
        /// <summary>
        /// 数据工厂名
        /// </summary>
        /// <returns></returns>
        public string FactoryName
        {
            get { return Factory.GetType().FullName; }
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
            Conn.Dispose();
        }
    }
}
