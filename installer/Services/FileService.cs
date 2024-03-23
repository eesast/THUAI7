using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace installer.Services
{
    public static class FileService
    {
        // TO DO: 将MergeUserFile整合到Update()中
        public static string GetFileMd5Hash(string strFileFullPath)
        {
            FileStream? fst = null;
            try
            {
                fst = new FileStream(strFileFullPath, FileMode.Open, FileAccess.Read);
                byte[] data = MD5.Create().ComputeHash(fst);

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                fst.Close();
                return sBuilder.ToString().ToLower();
            }
            catch (Exception)
            {
                if (fst != null)
                    fst.Close();
                if (File.Exists(strFileFullPath))
                    return "conflict";
                return "";
            }
            finally
            {
            }
        }

        public static string ConvertAbsToRel(string basePath, string fullPath)
        {
            if (fullPath.StartsWith(basePath))
            {
                fullPath = fullPath.Replace(basePath, ".");
            }
            return fullPath;
        }

        public static string ReadToEnd(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        public static string MergeUserCode(string strUserCode, string strOldTemplate, string strNewTemplate)
        {
            StringBuilder builder = new StringBuilder();
            string[] userLines = { }, oldLines = { }, newLines = { };
            BigInteger[] userIDs = { }, oldIDs = { }, newIDs = { };
            // 假设用户不会修改模板行数据
            var tasks = new Task[]
            {
                Task.Run(() =>
                {
                    var md5 = MD5.Create();
                    var encoder = Encoding.UTF8;
                    userLines = strUserCode.Split(Environment.NewLine);
                    userIDs = (from l in userLines
                           let m = md5.ComputeHash(encoder.GetBytes(l))
                           let n = new BigInteger(m)
                           select n).ToArray();
                }),
                Task.Run(() =>
                {
                    var md5 = MD5.Create();
                    var encoder = Encoding.UTF8;
                    oldLines = strOldTemplate.Split(Environment.NewLine);
                    oldIDs = (from l in oldLines
                           let m = md5.ComputeHash(encoder.GetBytes(l))
                           let n = new BigInteger(m)
                           select n).ToArray();
                }),
                Task.Run(() =>
                {
                    var md5 = MD5.Create();
                    var encoder = Encoding.UTF8;
                    newLines = strNewTemplate.Split(Environment.NewLine);
                    newIDs = (from l in newLines
                           let m = md5.ComputeHash(encoder.GetBytes(l))
                           let n = new BigInteger(m)
                           select n).ToArray();
                })
            };
            Task.WaitAll(tasks);

            // (是否为模板行，该行内容）
            List<(bool, string)> result = new List<(bool, string)>();

            // 将旧模板和用户文件进行对比，得出用户代码部分
            int i = 0, j = 0, k = 0;
            for (i = 0, j = 0; i < oldLines.Length && j < userLines.Length; i++)
            {
                while (oldIDs[i] != userIDs[j] || oldLines[i] != userLines[j])
                {
                    result.Add((false, userLines[j]));
                    j++;
                    if (j == userLines.Length)
                        break;
                }
                result.Add((true, userLines[j]));
                j++;
            }
            for (; j < userLines.Length; j++)
                result.Add((false, userLines[j]));


            for (i = 0, j = 0, k = 0; i < newLines.Length && j < oldLines.Length;)
            {
                while (!result[k].Item1)
                {
                    k++;
                    if (k == newLines.Length)
                        break;
                }
                if (newIDs[i] == oldIDs[j])
                {
                    i++; j++; k++;
                    continue;
                }
                // 新增行：旧模板本行与新模板下方某行匹配
                // 删除行：新模板本行与旧模板下方某行匹配
                // 修改行：均无匹配，替换成新行后前往下一行
                bool success = false;
                int i1 = i + 1;
                while (i1 < newLines.Length && !success)
                {
                    if (newIDs[i1] == oldIDs[j] && newLines[i1] == oldLines[j])
                    {
                        // 匹配成功
                        for (int x = i; x < i1; x++)
                        {
                            result.Insert(k, (true, newLines[x]));
                            k++;
                        }
                        i = i1;
                        success = true;
                    }
                    i1++;
                }
                int j1 = j + 1;
                while (j1 < oldLines.Length && !success)
                {
                    if (oldIDs[j1] == newIDs[i] && oldLines[j1] == newLines[i])
                    {
                        // 匹配成功
                        for (int x = j; x < j1; x++)
                        {
                            result.RemoveAt(k);
                        }
                        j = j1;
                        success = true;
                    }
                    j1++;
                }
                if (!success)
                {
                    result[k] = (true, newLines[i]);
                    i++; j++; k++;
                }
            }
            for (; i < newLines.Length; i++)
                result.Add((true, newLines[i]));

            foreach (var r in result)
            {
                builder.AppendLine(r.Item2);
            }

            return builder.ToString();
        }
    }
}
