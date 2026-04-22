using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace hms_tom_dev.Helper
{
    public static class LogHelper
    {
        public static string LogDirectory = AppDomain.CurrentDomain.BaseDirectory + "/logs/";

        public static bool WriteLog(string logName, string logMessage)
        {
            try
            {
                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);
                using (var txt = File.CreateText(LogDirectory + logName + ".log"))
                {
                    txt.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + logMessage);
                }
            }
            catch { }

            return false;
        }

        public static bool WriteLog(string logName, Exception exception)
        {
            string log = "Error: An exception has thrown" + ExceptionReader(exception) + "\r\n";
            return WriteLog(logName, log);
        }

        private static string ExceptionReader(Exception exception, int pad = 0)
        {
            string str = new string('\t', pad) + "Exception: " + exception.GetType().Name + " - " + exception.Message + "\r\n";
            str += new string('\t', pad) + "Stack Trace: \r\n";
            if (!string.IsNullOrWhiteSpace(exception.StackTrace))
                str += exception.StackTrace.Replace("\r\n", "\r\n" + new string('\t', pad));
            if (exception.InnerException != null)
            {
                str += "\r\n" + new string('\t', pad) + "Inner Exception: \r\n";
                str += ExceptionReader(exception.InnerException, pad + 1);
            }
            return str;
        }
    }
}