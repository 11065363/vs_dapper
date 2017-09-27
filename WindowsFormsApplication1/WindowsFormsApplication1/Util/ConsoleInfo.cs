using System;
using System.Collections;

namespace XYS.Utils.Sys
{
    public delegate void InfoReceivedEventHandler(object sender, InfoReceivedEventArgs e);
    public class InfoReceivedEventArgs : EventArgs
    {
        private ConsoleInfo m_info;
        public InfoReceivedEventArgs(ConsoleInfo info)
        {
            this.m_info = info;
        }
        public ConsoleInfo Info
        {
            get { return this.m_info; }
        }
    }
    /// <summary>
    /// 控制台信息类
    /// </summary>
    public class ConsoleInfo
    {
        #region 静态字段
        private static bool s_debugEnabled = false;
        private static bool s_quietMode = false;
        private static bool s_emitInternalMessages = true;
        private static readonly string INFO_PREFIX = "xys-log:INFO ";
        private static readonly string ERR_PREFIX = "xys-log:ERROR ";
        private static readonly string WARN_PREFIX = "xys-log:WARN ";
        #endregion

        #region 实例字段
        private readonly Type source;
        private readonly DateTime timeStamp;
        private readonly string prefix;
        private readonly string message;
        private readonly Exception exception;
        #endregion

        #region
        public static event InfoReceivedEventHandler InfoReceived;
        #endregion

        #region 实例构造函数
        public ConsoleInfo(Type source, string prefix, string message, Exception exception)
        {
            this.prefix = prefix;
            this.source = source;
            this.message = message;
            this.exception = exception;
            this.timeStamp = DateTime.Now;
        }
        #endregion

        #region 静态构造函数
        static ConsoleInfo()
        {
            try
            {
                InternalDebugging = SystemInfo.ToBoolean(SystemInfo.GetAppSetting("Console-Log.Internal.Debug"), false);
                QuietMode = SystemInfo.ToBoolean(SystemInfo.GetAppSetting("Console-Log.Internal.Quiet"), false);
                EmitInternalMessages = SystemInfo.ToBoolean(SystemInfo.GetAppSetting("Console-Log.Internal.Emit"), true);
            }
            catch (Exception ex)
            {
                // If an exception is thrown here then it looks like the config file does not
                // parse correctly.
                // We will leave debug OFF and print an Error message
                Error(typeof(ConsoleInfo), "Exception while reading ConfigurationSettings. Check your .config file is well formed XML.", ex);
            }
        }
        #endregion

        #region 实例属性
        public Type Source
        {
            get { return source; }
        }
        public DateTime TimeStamp
        {
            get { return timeStamp; }
        }
        public string Prefix
        {
            get { return prefix; }
        }
        public string Message
        {
            get { return message; }
        }
        public Exception Exception
        {
            get { return exception; }
        }
        public override string ToString()
        {
            return Prefix + Source.Name + ": " + Message;
        }
        #endregion

        #region 静态属性
        public static bool QuietMode
        {
            get { return s_quietMode; }
            set { s_quietMode = value; }
        }
        public static bool InternalDebugging
        {
            get { return s_debugEnabled; }
            set { s_debugEnabled = value; }
        }
        public static bool EmitInternalMessages
        {
            get { return s_emitInternalMessages; }
            set { s_emitInternalMessages = value; }
        }
        //调试模式 启用调试并且不是安静模式 
        public static bool IsDebugEnabled
        {
            get { return s_debugEnabled && !s_quietMode; }
        }
        //警告模式
        public static bool IsWarnEnabled
        {
            get { return !s_quietMode; }
        }
        //
        public static bool IsErrorEnabled
        {
            get { return !s_quietMode; }
        }
        #region

        #endregion 静态方法
        public static void Debug(Type source, string message)
        {
            if (IsDebugEnabled)
            {
                if (EmitInternalMessages)
                {
                    EmitOutLine(INFO_PREFIX + source.Name + ":" + message);
                }
                OnInfoReceived(source, INFO_PREFIX, message, null);
            }
        }
        public static void Debug(Type source, string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                if (EmitInternalMessages)
                {
                    EmitOutLine(INFO_PREFIX + source.Name + ":" + message);
                    if (exception != null)
                    {
                        EmitOutLine(exception.ToString());
                    }
                }
                OnInfoReceived(source, INFO_PREFIX, message, exception);
            }
        }
        public static void Warn(Type source, string message)
        {
            if (IsWarnEnabled)
            {
                if (EmitInternalMessages)
                {
                    EmitErrorLine(WARN_PREFIX + source.Name + ":" + message);
                }
                OnInfoReceived(source, WARN_PREFIX, message, null);
            }
        }
        public static void Warn(Type source, string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                if (EmitInternalMessages)
                {
                    EmitErrorLine(WARN_PREFIX + source.Name + ":" + message);
                    if (exception != null)
                    {
                        EmitErrorLine(exception.ToString());
                    }
                }
                OnInfoReceived(source, WARN_PREFIX, message, exception);
            }
        }
        public static void Error(Type source, string message)
        {
            if (IsErrorEnabled)
            {
                if (EmitInternalMessages)
                {
                    EmitErrorLine(ERR_PREFIX + source.Name + ":" + message);
                }
                OnInfoReceived(source, ERR_PREFIX, message, null);
            }
        }
        public static void Error(Type source, string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                if (EmitInternalMessages)
                {
                    EmitErrorLine(ERR_PREFIX + source.Name + ":" + message);
                    if (exception != null)
                    {
                        EmitErrorLine(exception.ToString());
                    }
                }
                OnInfoReceived(source, ERR_PREFIX, message, exception);
            }
        }
        //通知事件
        public static void OnInfoReceived(Type source, string prefix, string message, Exception exception)
        {
            if (InfoReceived != null)
            {
                InfoReceived(null, new InfoReceivedEventArgs(new ConsoleInfo(source, prefix, message, exception)));
            }
        }
        #endregion

        #region
        private static void EmitOutLine(string message)
        {
            try
            {
#if NETCF
				Console.WriteLine(message);
				//System.Diagnostics.Debug.WriteLine(message);
#else
                Console.Out.WriteLine(message);
                //Trace.WriteLine(message);
#endif
            }
            catch
            {
                // Ignore exception, what else can we do? Not really a good idea to propagate back to the caller
            }
        }
        private static void EmitErrorLine(string message)
        {
            try
            {
#if NETCF
				Console.WriteLine(message);
				//System.Diagnostics.Debug.WriteLine(message);
#else
                Console.Error.WriteLine(message);
                //Trace.WriteLine(message);
#endif
            }
            catch
            {
                // Ignore exception, what else can we do? Not really a good idea to propagate back to the caller
            }
        }
        #endregion

        #region
        public class InfoReceivedAdapter : IDisposable
        {
            private readonly IList items;
            private readonly InfoReceivedEventHandler handler;
            public InfoReceivedAdapter(IList items)
            {
                this.items = items;
                handler = new InfoReceivedEventHandler(ConsoleInfo_InfoReceived);
                InfoReceived += handler;
            }
            void ConsoleInfo_InfoReceived(object source, InfoReceivedEventArgs e)
            {
                items.Add(e.Info);
            }
            public IList Items
            {
                get { return items; }
            }
            public void Dispose()
            {
                InfoReceived -= handler;
            }
        }
        #endregion
    }
}
