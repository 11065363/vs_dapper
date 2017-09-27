using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XYS.Utils.Sys;

namespace WindowsFormsApplication1.DB
{
    class SqlManager
    {
        public static string GetSql(Dictionary<string,object> paramDic, String sqlName) {
            try
            {
                List<string> filter = new List<string>();
                if (paramDic != null)
                {
                    filter = paramDic.Keys.ToList();
                }
                XDocument document = XDocument.Load(SystemInfo.GetFullName(SystemInfo.ApplicationBaseDirectory, "Config", "sql.xml"));
                XElement element = document.Element("root");
                Clause clause = (from sql in document.Descendants("sql").Where(sql => sql.Attribute("name").Value == sqlName && sql.Descendants("where").Any())
                                 select new Clause
                                 {
                                     SqlStr = sql.Element("value").Value,
                                     Where = (from f in sql.Descendants("where")
                                              select new
                                              {
                                                  key = f.Attribute("field").Value,
                                                  value = f.Value
                                              }).ToDictionary(item => item.key, item => item.value)
                                 }).FirstOrDefault();
                //查看sql语句是否存在
                if (string.IsNullOrEmpty(clause.SqlStr))
                {
                    return string.Empty;
                }
                //xml提供的sql语句
                string str = clause.SqlStr + SystemInfo.NewLine; //+ " where  1=1" + SystemInfo.NewLine;
                /*
                foreach (string sk in filter)
                {
                    string field;
                    if (clause.Where.TryGetValue(sk, out field))
                    {
                        str = str + "and " + field + SystemInfo.NewLine;
                    }
                }
                */
                return str;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
    public class Clause
    {
        /// <summary>
        /// sql语句
        /// </summary>
        public string SqlStr { get; set; }
        /// <summary>
        /// where集合
        /// </summary>
        public Dictionary<string, string> Where { get; set; }
    }
}
