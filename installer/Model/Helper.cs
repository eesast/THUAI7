using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace installer.Model
{
    internal static class Helper
    {
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
            fullPath = fullPath.Replace(Path.DirectorySeparatorChar, '/');
            if (fullPath.StartsWith(basePath))
            {
                fullPath = fullPath.Replace(basePath, ".");
            }
            return fullPath;
        }
    }
}
