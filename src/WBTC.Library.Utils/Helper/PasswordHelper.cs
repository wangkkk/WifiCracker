using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBTC.Library.Utils.Model;

namespace WBTC.Library.Utils.Helper
{
    public static class PasswordHelper
    {
        public static ConcurrentQueue<string> PasswordQuqe { get; set; } = null;
        /// <summary>
        /// 第一个bool类型是：是否完成读取
        /// </summary>
        public static Tuple<bool, ReadStatus> ReadFileStatus { get; set; }
        #region 根据规则生成密码

        #endregion

        #region Read File Password
        /// <summary>
        /// 返回ReadFileStatus，能实现一边读取文件内容，一边取密码
        /// 小文件可以直接等待函数执行完，再从队列中拿数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<bool> GetPassword(string filePath)
        {
            ReadFileStatus = new Tuple<bool, ReadStatus>(false, ReadStatus.ReadyToBegin);
            return await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        ReadFileStatus = new Tuple<bool, ReadStatus>(true, ReadStatus.NotHasPath);
                        return false;
                    }
                    using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(filePath))
                    {
                        using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                        {
                            string leftoverData = string.Empty;
                            byte[] buffer = new byte[16384];
                            int bytesRead = 0;
                            PasswordQuqe = new ConcurrentQueue<string>();
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                leftoverData = AnalysiData(leftoverData, buffer, bytesRead);
                            }
                            ReadFileStatus = new Tuple<bool, ReadStatus>(true, ReadStatus.Success);
                            return true;
                        }
                    }
                }
                catch
                {
                    ReadFileStatus = new Tuple<bool, ReadStatus>(true, ReadStatus.Error);
                    return false;
                }

            });

        }
        /// <summary>
        /// 分析并将数据入队
        /// </summary>
        /// <param name="leftoverData"></param>
        /// <param name="buffer"></param>
        /// <param name="bytesRead"></param>
        /// <returns></returns>
        private static string AnalysiData(string leftoverData, byte[] buffer, int bytesRead)
        {
            string data = leftoverData + Encoding.Default.GetString(buffer, 0, bytesRead);
            StringBuilder sb = new StringBuilder(data);

            // 如果最后一个字符不是换行符，则将其作为未处理数据存储
            if (!data.EndsWith("\n"))
            {
                leftoverData = sb.ToString(sb.Length - 1, 1);
                sb.Length--;
            }

            string[] lines = sb.ToString().Split(new[] { '\n' }, StringSplitOptions.None);

            // 使用 Queue 存储行数据
            foreach (string line in lines)
            {
                PasswordQuqe.Enqueue(line);
            }
            return leftoverData;
        }
        #endregion
        /// <summary>
        /// 读取小文件
        /// </summary>                                                                                            11          
        /// <param name="passwordFilePath"></param>
        /// <returns></returns>
        public static void ReadLittleFileStr(string passwordFilePath)
        {
            if(PasswordQuqe==null)
                PasswordQuqe=new ConcurrentQueue<string>();
            using (StreamReader sr=new StreamReader(passwordFilePath))
            {
                string line;
                while((line=sr.ReadLine())!=null)
                {
                    PasswordQuqe.Enqueue(line);
                }
            }
        }
    }
    
}
