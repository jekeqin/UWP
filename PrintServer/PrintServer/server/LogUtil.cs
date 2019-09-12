using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServer.server
{
    public class LogUtil
    {
        private static string logFilePath = null;

        private static void initFile()
        {
            if (logFilePath == null) {
                logFilePath = System.Environment.CurrentDirectory + "\\log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                if (!System.IO.File.Exists(logFilePath))
                {
                    FileStream stream = System.IO.File.Create(logFilePath);
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        public static void Info(string str)
        {
            initFile();
            writeToFile("INFO", str);
        }

        public static void Wran(string str)
        {
            initFile();
            writeToFile("WRAN", str);
        }

        public static void Error(string str)
        {
            initFile();
            writeToFile("ERROR", str);
        }

        private static void writeToFile(string type,string  log)
        {
            Console.WriteLine("{0}\t{1}\t{2}", DateTime.Now.ToString(), type, log);
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine("{0}\t{1}\t{2}", DateTime.Now.ToString(), type, log);
            }
        }
    }
}
