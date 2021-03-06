﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsFormsApplication1.Util;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        //IDbConnection conn = new SqlConnection(connString);
        private void button1_Click(object sender, EventArgs e)
        {/*
            string query = "INSERT INTO Book(Name)VALUES(@name)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "wo");
           // book.Name = "wo";
            //对字典进行操作
            conn.Execute(query, dic);
            MessageBox.Show("完成");
            */
        }
        #region //暂时不用

        /*
        
        //public static string connString= "server=127.0.0.1;database=MyDataBase;User=sa;password=123456;Connect Timeout=1000000";
        book book = new book();
        //增
        public void insert() {  
            book.Name = "C#本质论";
            string query = "INSERT INTO Book(Name)VALUES(@name)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name","wo");
            //对对象进行操作
            conn.Execute(query, dic);
            conn.Execute(query, book);
            //直接赋值操作
            conn.Execute(query, new { name = "C#本质论" });
        }
        //删
        public void delete() {
            string query = "DELETE FROM Book WHERE id = @id";
            conn.Execute(query, book);
            conn.Execute(query, new { id = 1 });
        }
        //改
        public void update() {
            string query = "UPDATE Book SET  Name=@name WHERE id =@id";
            conn.Execute(query, book);
        }
        //查
        public void select() {
            string query = "SELECT * FROM Book";
            //无参数查询，返回列表，带参数查询和之前的参数赋值法相同。
            conn.Query<book>(query).ToList();

            //返回单条信息
            string query2= "SELECT * FROM Book WHERE id = @id";
            book = conn.Query<book>(query2, new { id = 1 }).SingleOrDefault();
        }
        */
        book_doa doa = new book_doa();
        #endregion
        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", 1);
            dic.Add("name", "hello");
            doa.book_inp(dic);
            MessageBox.Show("完成");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //第一种记录用法
            //（1）FormMain是类名称
            //（2）第二个参数是字符串信息
            LogHelper.WriteLog(typeof(Form1), "测试Log4Net日志是否写入");
            //第二种记录用法
            //（1）FormMain是类名称
            //（2）第二个参数是需要捕捉的异常块
            //try { 
            //}catch(Exception ex){
            //    LogHelper.WriteLog(typeof(FormMain), ex);
            //}
        }
        //删
        private void button4_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name", "hello");
            doa.book_delete(dic);
        }
        //改
        private void button5_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", 1);
            dic.Add("name", "hello2");
            doa.book_update(dic);
        }
        //查
        private void button6_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            doa.book_select(dic);
        }
        //查单条
        private void button7_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("id", 1);
            //dic.Add("name", "hello2");
            doa.book_select_single(dic);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("name","ss");
            doa.book_select_more(dic);
        }
    }
}
