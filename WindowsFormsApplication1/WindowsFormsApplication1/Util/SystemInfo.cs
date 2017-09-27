using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Configuration;
using System.Runtime.InteropServices;

namespace XYS.Utils.Sys
{
    /// <summary>
    /// 系统方法类
    /// </summary>
    public sealed class SystemInfo
    {
        #region 私有常量
        private readonly static string DEFAULT_NULL_TEXT = "(null)";
        private readonly static string DEFAULT_NOT_AVAILABLE_TEXT = "NOT AVAILABLE";
        #endregion

        #region 私有实例构造函数
        private SystemInfo()
        {
        }
        #endregion Private Instance Constructors

        #region 静态构造函数
        static SystemInfo()
        {
            string nullText = DEFAULT_NULL_TEXT;
            string notAvailableText = DEFAULT_NOT_AVAILABLE_TEXT;
#if !NETCF
            // Look for XYS.NullText in AppSettings
            string nullTextAppSettingsKey = SystemInfo.GetAppSetting("XYS.NullText");
            if (nullTextAppSettingsKey != null && nullTextAppSettingsKey.Length > 0)
            {
                ConsoleInfo.Debug(declaringType, "Initializing NullText value to [" + nullTextAppSettingsKey + "].");
                nullText = nullTextAppSettingsKey;
            }
            // Look for XYS.NotAvailableText in AppSettings
            string notAvailableTextAppSettingsKey = SystemInfo.GetAppSetting("XYS.NotAvailableText");
            if (notAvailableTextAppSettingsKey != null && notAvailableTextAppSettingsKey.Length > 0)
            {
                ConsoleInfo.Debug(declaringType, "Initializing NotAvailableText value to [" + notAvailableTextAppSettingsKey + "].");
                notAvailableText = notAvailableTextAppSettingsKey;
            }
#endif
            s_notAvailableText = notAvailableText;
            s_nullText = nullText;
        }
        #endregion

        #region 公共静态属性
        //获取回车换行符
        public static string NewLine
        {
            get
            {
                return System.Environment.NewLine;
            }
        }
        //获取应用基本路径
        public static string ApplicationBaseDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        //获取程序默认配置文件
        public static string ConfigurationFileLocation
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            }
        }
        //获取程序集位置
        public static string EntryAssemblyLocation
        {
            get
            {
                return Assembly.GetEntryAssembly().Location;
            }
        }
        //获取当前线程id
        public static int CurrentThreadId
        {
            get
            {
                //return AppDomain.GetCurrentThreadId();
                return System.Threading.Thread.CurrentThread.ManagedThreadId;
            }
        }
        //获取主机名
        public static string HostName
        {
            get
            {
                if (s_hostName == null)
                {
                    // Get the DNS host name of the current machine
                    try
                    {
                        // Lookup the host name
                        s_hostName = System.Net.Dns.GetHostName();
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        ConsoleInfo.Debug(declaringType, "Socket exception occurred while getting the dns hostname. Error Ignored.");
                    }
                    catch (System.Security.SecurityException)
                    {
                        // We may get a security exception looking up the hostname
                        // You must have Unrestricted DnsPermission to access resource
                        ConsoleInfo.Debug(declaringType, "Security exception occurred while getting the dns hostname. Error Ignored.");
                    }
                    catch (Exception ex)
                    {
                        ConsoleInfo.Debug(declaringType, "Some other exception occurred while getting the dns hostname. Error Ignored.", ex);
                    }
                    // Get the NETBIOS machine name of the current machine
                    //获取当前主机的 netbios名称
                    if (s_hostName == null || s_hostName.Length == 0)
                    {
                        try
                        {
                            s_hostName = Environment.MachineName;
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        catch (System.Security.SecurityException)
                        {
                            // We may get a security exception looking up the machine name
                            // You must have Unrestricted EnvironmentPermission to access resource
                        }
                    }

                    // Couldn't find a value
                    if (s_hostName == null || s_hostName.Length == 0)
                    {
                        s_hostName = s_notAvailableText;
                        ConsoleInfo.Debug(declaringType, "Could not determine the hostname. Error Ignored. Empty host name will be used");
                    }
                }
                return s_hostName;
            }
        }
        //获取应用程序域的友好名称
        public static string ApplicationFriendlyName
        {
            get
            {
                if (s_appFriendlyName == null)
                {
                    try
                    {
                        s_appFriendlyName = AppDomain.CurrentDomain.FriendlyName;
                    }
                    catch (System.Security.SecurityException)
                    {
                        // This security exception will occur if the caller does not have 
                        // some undefined set of SecurityPermission flags.
                        ConsoleInfo.Debug(declaringType, "Security exception while trying to get current domain friendly name. Error Ignored.");
                    }
                    if (string.IsNullOrEmpty(s_appFriendlyName))
                    {
                        try
                        {
                            string assemblyLocation = SystemInfo.EntryAssemblyLocation;
                            s_appFriendlyName = Path.GetFileName(assemblyLocation);
                        }
                        catch (System.Security.SecurityException)
                        {
                            // Caller needs path discovery permission
                        }
                    }
                    //找不到友好名称
                    if (string.IsNullOrEmpty(s_appFriendlyName))
                    {
                        s_appFriendlyName = s_notAvailableText;
                    }
                }
                return s_appFriendlyName;
            }
        }
        //获取当前时间
        public static DateTime ProcessStartTime
        {
            get { return s_processStartTime; }
        }

        public static string NullText
        {
            get { return s_nullText; }
            set { s_nullText = value; }
        }
        public static string NotAvailableText
        {
            get { return s_notAvailableText; }
            set { s_notAvailableText = value; }
        }
        #endregion

        #region 公共静态方法

        //获取特定程序集的位置信息
        public static string AssemblyLocationInfo(Assembly myAssembly)
        {
            if (myAssembly.GlobalAssemblyCache)
            {
                return "Global Assembly Cache";
            }
            else
            {
                try
                {
                    if (myAssembly is System.Reflection.Emit.AssemblyBuilder)
                    {
                        return "Dynamic Assembly";
                    }
                    else if (myAssembly.GetType().FullName == "System.Reflection.Emit.InternalAssemblyBuilder")
                    {
                        return "Dynamic Assembly";
                    }
                    else
                    {
                        // This call requires FileIOPermission for access to the path
                        // if we don't have permission then we just ignore it and
                        // carry on.
                        return myAssembly.Location;
                    }
                }
                catch (NotSupportedException)
                {
                    // The location information may be unavailable for dynamic assemblies and a NotSupportedException
                    // is thrown in those cases. See: http://msdn.microsoft.com/de-de/library/system.reflection.assembly.location.aspx
                    return "Dynamic Assembly";
                }
                catch (TargetInvocationException ex)
                {
                    return "Location Detect Failed (" + ex.Message + ")";
                }
                catch (ArgumentException ex)
                {
                    return "Location Detect Failed (" + ex.Message + ")";
                }
                catch (System.Security.SecurityException)
                {
                    return "Location Permission Denied";
                }
            }
        }
        //程序集的简称
        public static string AssemblyShortName(Assembly myAssembly)
        {
            string name = myAssembly.FullName;
            int offset = name.IndexOf(',');
            if (offset > 0)
            {
                name = name.Substring(0, offset);
            }
            return name.Trim();
            // TODO: Do we need to unescape the assembly name string? 
            // Doc says '\' is an escape char but has this already been 
            // done by the string loader?
        }
        //程序集文件名称
        public static string AssemblyFileName(Assembly myAssembly)
        {
            return Path.GetFileName(myAssembly.Location);
        }

        public static Type GetTypeFromString(Type relativeType, string typeName, bool throwOnError, bool ignoreCase)
        {
            return GetTypeFromString(relativeType.Assembly, typeName, throwOnError, ignoreCase);
        }
        public static Type GetTypeFromString(string typeName, bool throwOnError, bool ignoreCase)
        {
            return GetTypeFromString(Assembly.GetCallingAssembly(), typeName, throwOnError, ignoreCase);
        }
        public static Type GetTypeFromString(Assembly relativeAssembly, string typeName, bool throwOnError, bool ignoreCase)
        {
            // typeName 不包含程序集名称
            if (typeName.IndexOf(',') == -1)
            {
                ConsoleInfo.Debug(declaringType, "Loading type [" + typeName + "] from assembly [" + relativeAssembly.FullName + "]");
                // 尝试从调用者所在程序集加载类型
                Type type = relativeAssembly.GetType(typeName, false, ignoreCase);
                if (type != null)
                {
                    // Found type in relative assembly
                    ConsoleInfo.Debug(declaringType, "Loaded type [" + typeName + "] from assembly [" + relativeAssembly.FullName + "]");
                    return type;
                }
                Assembly[] loadedAssemblies = null;
                try
                {
                    //获取加载的程序集
                    loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                }
                catch (System.Security.SecurityException)
                {
                    // Insufficient permissions to get the list of loaded assemblies
                }
                if (loadedAssemblies != null)
                {
                    //从加载的程序集中加载类型
                    foreach (Assembly assembly in loadedAssemblies)
                    {
                        type = assembly.GetType(typeName, false, ignoreCase);
                        if (type != null)
                        {
                            // Found type in loaded assembly
                            ConsoleInfo.Debug(declaringType, "Loaded type [" + typeName + "] from assembly [" + assembly.FullName + "] by searching loaded assemblies.");
                            return type;
                        }
                    }
                }
                // Didn't find the type
                if (throwOnError)
                {
                    throw new TypeLoadException("SystemInfo:Could not load type [" + typeName + "]. Tried assembly [" + relativeAssembly.FullName + "] and all loaded assemblies");
                }
                return null;
            }
            else
            {
                //包含程序集名称
                ConsoleInfo.Debug(declaringType, "Loading type [" + typeName + "] from global Type");
                return Type.GetType(typeName, throwOnError, ignoreCase);
            }
        }
        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        public static bool TryParse(string s, out int val)
        {
            // 初始化 out 参数
            val = 0;
            try
            {
                double doubleVal;
                if (Double.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out doubleVal))
                {
                    val = Convert.ToInt32(doubleVal);
                    return true;
                }
            }
            catch
            {
                // Ignore exception, just return false
            }
            return false;
        }
        public static bool TryParse(string s, out long val)
        {
            // 初始化 out 参数
            val = 0;
            try
            {
                double doubleVal;
                if (Double.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out doubleVal))
                {
                    val = Convert.ToInt64(doubleVal);
                    return true;
                }
            }
            catch
            {
                // Ignore exception, just return false
            }
            return false;
        }
        public static bool TryParse(string s, out short val)
        {
            // 初始化 out 参数
            val = 0;
            try
            {
                double doubleVal;
                if (Double.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out doubleVal))
                {
                    val = Convert.ToInt16(doubleVal);
                    return true;
                }
            }
            catch
            {
                // Ignore exception, just return false
            }
            return false;
        }
        public static bool ToBoolean(string argValue, bool defaultValue)
        {
            if (argValue != null && argValue.Length > 0)
            {
                try
                {
                    return bool.Parse(argValue);
                }
                catch (Exception e)
                {
                    ConsoleInfo.Error(declaringType, "[" + argValue + "] is not in proper bool", e);
                }
            }
            return defaultValue;
        }
        public static string GetAppSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                ConsoleInfo.Error(declaringType, "Exception while reading ConfigurationSettings. Check your .config file is well formed XML.", ex);
            }
            return null;
        }
        public static string GetConnString(string name)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch (Exception ex)
            {
                ConsoleInfo.Error(declaringType, "Exception while reading ConfigurationSettings. Check your .config file", ex);
            }
            return null;
        }
        public static ConnStruct GetConnStruct(string name)
        {
            ConnStruct cs;
            try
            {
                cs = new ConnStruct
                {
                    ConnStr = ConfigurationManager.ConnectionStrings[name].ConnectionString,
                    ProviderName = ConfigurationManager.ConnectionStrings[name].ProviderName
                };
            }
            catch (Exception ex)
            {
                ConsoleInfo.Error(declaringType, "Exception while reading ConfigurationSettings. Check your .config file", ex);
                cs = null;
            }
            return cs;
        }
        public static string ConvertToFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            string baseDirectory = "";
            try
            {
                string applicationBaseDirectory = ApplicationBaseDirectory;
                if (applicationBaseDirectory != null)
                {
                    // applicationBaseDirectory may be a URI not a local file path
                    Uri applicationBaseDirectoryUri = new Uri(applicationBaseDirectory);
                    if (applicationBaseDirectoryUri.IsFile)
                    {
                        baseDirectory = applicationBaseDirectoryUri.LocalPath;
                    }
                }
            }
            catch
            {
                // Ignore URI exceptions & SecurityExceptions from SystemInfo.ApplicationBaseDirectory
            }
            if (baseDirectory != null && baseDirectory.Length > 0)
            {
                // Note that Path.Combine will return the second path if it is rooted
                return Path.GetFullPath(Path.Combine(baseDirectory, path));
            }
            return Path.GetFullPath(path);
        }
        public static bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }
        public static bool IsType(Type type1, Type type2)
        {
            return type1.Equals(type2);
        }
        public static bool DeleteFile(string filePath)
        {
            if (IsFileExist(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true;
                }
                catch (Exception ex)
                {
                    ConsoleInfo.Warn(declaringType, "deleting the " + filePath + " file ocurre exception:" + ex.Message);
                }
            }
            return false;
        }
        public static void MakeDir(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }
        public static string GetFullName(params string[] paths)
        {
            return Path.Combine(paths);
        }
        public static string FormatDateTime(DateTime dt)
        {
            return FormatDateTime(dt, "yyyy-MM-dd HH:mm", "");
        }
        public static string FormatDateTime(DateTime dt, string formatter, string emptyLabel)
        {
            string result = null;
            if (dt == DateTime.MinValue)
            {
                result = emptyLabel;
            }
            else
            {
                try
                {
                    result = dt.ToString(formatter);
                }
                catch (FormatException fe)
                {
                    ConsoleInfo.Warn(declaringType, "formatting datetime occuring exception", fe);
                }
            }
            return result;
        }
        public static Hashtable CreateCaseInsensitiveHashtable(int capacity)
        {
            if (capacity > 0)
            {
                try
                {
                    return new Hashtable(capacity, new MyCaseInsensitiveComparer());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new Hashtable(new MyCaseInsensitiveComparer());
        }
        #endregion

        #region 公共静态字段
        public static readonly Type[] EmptyTypes = new Type[0];
        #endregion

        #region 私有静态字段
        private readonly static Type declaringType = typeof(SystemInfo);
        private static string s_hostName;
        private static string s_appFriendlyName;
        private static string s_nullText;
        private static string s_notAvailableText;
        private static DateTime s_processStartTime = DateTime.Now;
        #endregion

        #region Hashtable 帮助类
        public class MyCaseInsensitiveComparer : IEqualityComparer
        {
            private CaseInsensitiveComparer caseInsensitiveComparer;
            public MyCaseInsensitiveComparer()
            {
                try
                {
                    this.caseInsensitiveComparer = CaseInsensitiveComparer.DefaultInvariant;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public MyCaseInsensitiveComparer(CultureInfo culture)
            {
                this.caseInsensitiveComparer = new CaseInsensitiveComparer(culture);
            }
            public new bool Equals(object x, object y)
            {
                if (this.caseInsensitiveComparer.Compare(x, y) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public int GetHashCode(object obj)
            {
                return obj.ToString().ToLower().GetHashCode();
            }
        }
        #endregion
    }
    /// <summary>
    /// 数据库连接体
    /// </summary>
    public class ConnStruct
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnStr { get; set; }
        /// <summary>
        /// 驱动器名称
        /// </summary>
        public string ProviderName { get; set; }
    }
}
