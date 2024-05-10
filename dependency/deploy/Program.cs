using installer.Data;
using installer.Model;
using installer.Services;
using System.Collections.Concurrent;
using System.Diagnostics;

Logger Log = LoggerProvider.FromConsole();

// 全权读写
Tencent_Cos Cloud = new Tencent_Cos("1319625962", "ap-beijing", "thuai7", Log);
Cloud.UpdateSecret(args[0], args[1]);

Downloader d = new Downloader();
d.Data.Config.InstallPath = @"D:\a\mirror\";
d.Log.Partner.Add(Log);
// 每次更新需要更新默认值
d.CurrentVersion = new TVersion();
File.Create(Path.Combine("D:\\a\\publish", d.CurrentVersion.InstallerVersion.ToString()));

if (d.CheckUpdate())
{
    foreach (var r in d.Data.MD5Update)
    {
        Log.LogInfo($"{r.state}, {r.name}");
    }

    d.Data.SaveMD5Data();
    List<Task> l = new List<Task>();
    foreach (var r in d.Data.MD5Update)
    {
        var n = r.name.Replace('\\', '/');
        n = n.TrimStart('.').TrimStart('/');
        if (r.state == System.Data.DataRowState.Added || r.state == System.Data.DataRowState.Modified)
        {
            l.Add(Cloud.UploadFileAsync(Path.Combine(d.Data.Config.InstallPath, r.name), n));
        }
        else if (r.state == System.Data.DataRowState.Deleted)
        {
            l.Add(Cloud.DeleteFileAsync(n));
        }
    }
    Task.WaitAll(l.ToArray());
}
else
{
    Log.LogInfo("Nothing to update");    
}

var found = (from i in d.Data.MD5Data
         where i.Key.Contains("interface")
         select i).ToList();

d.Data.SaveMD5Data();
Cloud.UploadFile(Path.Combine(d.Data.Config.InstallPath, "hash.json"), "hash.json");

Cloud.UploadFile(Path.Combine(d.Data.Config.InstallPath, "CAPI", "cpp", "API", "src", "AI.cpp"),
    $"Templates/t.{d.CurrentVersion.TemplateVersion}.cpp");
Cloud.UploadFile(Path.Combine(d.Data.Config.InstallPath, "CAPI", "python", "PyAPI", "AI.py"),
    $"Templates/t.{d.CurrentVersion.TemplateVersion}.py");
Log.LogInfo("User code uploaded.");

var list = (from i in d.Data.MD5Data
           select i.Key.Replace(Path.DirectorySeparatorChar, '/').TrimStart('.').TrimStart('/')).ToArray();
Log.LogInfo(list[0]);
using (FileStream s = new FileStream(Path.Combine(d.Data.Config.InstallPath, "compress.csv"), FileMode.Create, FileAccess.Write))
using (StreamWriter w = new StreamWriter(s))
{
    foreach (var item in list)
    {
        w.WriteLine("https://thuai7-1319625962.cos.ap-beijing.myqcloud.com/" + item);
    }
}
Cloud.UploadFile(Path.Combine(d.Data.Config.InstallPath, "compress.csv"), "compress.csv");
Log.LogInfo("Compress csv generated.");