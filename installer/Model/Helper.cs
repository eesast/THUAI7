using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace installer.Model
{
    internal static class Helper
    {
        public static T DeserializeJson1<T>(string json)
            where T : notnull
        {
            return JsonConvert.DeserializeObject<T>(json)
                ?? throw new Exception("Failed to deserialize json.");
        }

        public static T? TryDeserializeJson<T>(string json)
            where T : notnull
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

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
                fullPath.Replace(basePath, ".");
            }
            return fullPath;
        }
    }
}
