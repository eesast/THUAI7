using System.Text.Json;

Dictionary<string, string> MD5Data = new Dictionary<string, string>();

public string ConvertAbsToRel(string basePath, string fullPath)
{
    if (fullPath.StartsWith(basePath))
    {
        fullPath = fullPath.Replace(basePath, ".");
    }
    return fullPath;
}

public string GetFileMd5Hash(string strFileFullPath)
{
    var fst = new FileStream(strFileFullPath, FileMode.Open, FileAccess.Read);
    byte[] data = System.Security.Cryptography.MD5.Create().ComputeHash(fst);

    StringBuilder sBuilder = new StringBuilder();

    for (int i = 0; i < data.Length; i++)
    {
        sBuilder.Append(data[i].ToString("x2"));
    }

    fst.Close();
    return sBuilder.ToString().ToLower();
}

public static bool IsUserFile(string filename)
{
    if (filename.Contains("git") || filename.Contains("bin") || filename.Contains("obj"))
        return true;
    if (filename.EndsWith("sh") || filename.EndsWith("cmd"))
        return true;
    if (filename.EndsWith("gz"))
        return true;
    if (filename.Contains("AI.cpp") || filename.Contains("AI.py"))
        return true;
    if (filename.Contains("hash.json"))
        return true;
    if (filename.EndsWith("log"))
        return true;
    return false;
}

public void SaveMD5Data()
{
    FileStream fs = new FileStream(@"/home/runner/work/THUAI7/hash.json", FileMode.OpenOrCreate, FileAccess.ReadWrite);
    StreamWriter sw = new StreamWriter(fs);
    fs.SetLength(0);
    var exp1 = from i in MD5Data
               select new KeyValuePair<string, string>(i.Key.Replace(Path.DirectorySeparatorChar, '/'), i.Value);
    sw.Write(JsonSerializer.Serialize(exp1.ToDictionary<string, string>()));
    sw.Flush();
    sw.Dispose();
    fs.Dispose();
}

public void ScanDir(string dir)
{
    var d = new DirectoryInfo(dir);
    foreach (var file in d.GetFiles())
    {
        var relFile = ConvertAbsToRel(@"/home/runner/work/THUAI7/", file.FullName);
        // 用户自己的文件不会被计入更新hash数据中
        if (IsUserFile(relFile))
            continue;
        var hash = GetFileMd5Hash(file.FullName);
        if (MD5Data.Keys.Contains(relFile))
        {
            if (MD5Data[relFile] != hash)
            {
                MD5Data[relFile] = hash;
            }
        }
        else
        {
            MD5Data.Add(relFile, hash);
        }
    }
    foreach (var d1 in d.GetDirectories()) { ScanDir(d1.FullName); }
}

ScanDir(@"/home/runner/work/THUAI7/");
SaveMD5Data();