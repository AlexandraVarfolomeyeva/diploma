using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace diploma
{
    public class Log
    {
            private static object sync = new object();
            public static void Write(Exception ex, string text="")
            {
                try
                {
                    // Путь .\\Log
                    string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                    if (!Directory.Exists(pathToLog))
                        Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                    string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log",
                    AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                    string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}.{2}()] {3} \nComment: {4}\r\n",
                    DateTime.Now, ex.TargetSite.DeclaringType, ex.TargetSite.Name, ex.Message, text);
                    lock (sync)
                    {
                        File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                    }
                }
                catch
                {
                    // Перехватываем все и ничего не делаем
                }
            }
        public static void WriteSuccess(string method, string action)
        {
            try
            {
                // Путь .\\Log
                string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log",
                AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = string.Format("[{0:dd.MM.yyy HH:mm:ss.fff}] [{1}()] {2}\r\n",
                DateTime.Now, method, action);
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, Encoding.GetEncoding("Windows-1251"));
                }
            }
            catch
            {
                // Перехватываем все и ничего не делаем
            }
        }
    }
}
