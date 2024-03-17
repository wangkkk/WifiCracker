using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBTC.Library.Utils.Helper
{
    public static class LogHelper
    {
        private static object _lock=new object();

        public static void WriteLog(string log,Exception ex=null)
        {
            var nowTime=DateTime.Now;
            string strOfNow = nowTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logFileName = nowTime.ToString("yyyyMMdd");
            string folderName = Path.Combine(Environment.CurrentDirectory, "log");
            if(!Directory.Exists(folderName)) 
            {
                Directory.CreateDirectory(folderName);
            }
            string path = Path.Combine(folderName, logFileName+".txt");
            lock(_lock)
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    string logStr = ex == null ? log:$"发生错误：{ex.Message}";
                    sw.WriteLine($"{strOfNow}:{logStr}");
                    sw.Close();
                    sw.Dispose();
                }
            }
            
        }
    }
}
