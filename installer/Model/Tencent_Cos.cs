using COSXML;
using COSXML.Auth;
using COSXML.CosException;
using COSXML.Model.Object;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using COSXML.Common;
using COSXML.Transfer;
using CoreGraphics;

namespace installer.Model
{
    public class Tencent_Cos
    {
        public string Appid { get; init; }      // 设置腾讯云账户的账户标识（APPID）
        public string Region { get; init; }     // 设置一个默认的存储桶地域
        public string BucketName { get; set; }
        public ConcurrentStack<Exception> Exceptions { get; set; }

        private string secretId = "***"; //"云 API 密钥 SecretId";
        private string secretKey = "***"; //"云 API 密钥 SecretKey";
        protected CosXmlServer cosXml;

        public Tencent_Cos(string appid, string region, string bucketName)
        {
            Appid = appid; Region = region; BucketName = bucketName;
            Exceptions = new ConcurrentStack<Exception>();
            // 初始化CosXmlConfig（提供配置SDK接口）
            var config = new CosXmlConfig.Builder()
                        .IsHttps(true)      // 设置默认 HTTPS 请求
                        .SetAppid(Appid)    // 设置腾讯云账户的账户标识 APPID
                        .SetRegion(Region)  // 设置一个默认的存储桶地域
                        .SetDebugLog(true)  // 显示日志
                        .Build();           // 创建 CosXmlConfig 对象
            long durationSecond = 1000;  // 每次请求签名有效时长，单位为秒
            QCloudCredentialProvider cosCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);
            // 初始化 CosXmlServer
            cosXml = new CosXmlServer(config, cosCredentialProvider);
        }

        public void UpdateSecret(QCloudCredentialProvider credential)
        {
            var config = new CosXmlConfig.Builder()
            .IsHttps(true)      // 设置默认 HTTPS 请求
            .SetAppid(Appid)    // 设置腾讯云账户的账户标识 APPID
            .SetRegion(Region)  // 设置一个默认的存储桶地域
            .SetDebugLog(true)  // 显示日志
            .Build();           // 创建 CosXmlConfig 对象
            cosXml = new CosXmlServer(config, credential);
        }

        public async Task DownloadFileAsync(string savePath, string remotePath = null)
        {
            // download_dir标记根文件夹路径，key为相对根文件夹的路径（不带./）
            // 创建存储桶
            try
            {
                // 覆盖对应文件，如果无法覆盖则报错
                if (File.Exists(savePath))
                    File.Delete(savePath);
                string bucket = $"{BucketName}-{Appid}";                                // 格式：BucketName-APPID
                string localDir = Path.GetDirectoryName(savePath)     // 本地文件夹
                    ?? throw new Exception("本地文件夹路径获取失败");
                string localFileName = Path.GetFileName(savePath);    // 指定本地保存的文件名
                GetObjectRequest request = new GetObjectRequest(bucket, remotePath ?? localFileName, localDir, localFileName);

                Dictionary<string, string> test = request.GetRequestHeaders();
                request.SetCosProgressCallback(delegate (long completed, long total)
                {
                    //Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
                });
                // 执行请求
                GetObjectResult result = cosXml.GetObject(request);
                // 请求成功
            }
            catch (Exception ex)
            {
                Exceptions.Push(ex);
                throw;
                //MessageBox.Show($"下载{download_dir}时出现未知问题，请反馈");
            }
        }

        public async Task DownloadQueueAsync(ConcurrentQueue<string> queue, ConcurrentQueue<string> downloadFailed)
        {
            ThreadPool.SetMaxThreads(20, 20);
            for (int i = 0; i < queue.Count; i++)
            {
                string item;
                queue.TryDequeue(out item);
                ThreadPool.QueueUserWorkItem(async _ =>
                {
                    try
                    {
                        await DownloadFileAsync(item);
                    }
                    catch (Exception ex)
                    {
                        downloadFailed.Enqueue(item);
                    }
                });
            }
        }

        public void ArchieveUnzip(string zipPath, string targetDir)
        {
            Stream? inStream = null;
            Stream? gzipStream = null;
            TarArchive? tarArchive = null;
            try
            {
                using (inStream = File.OpenRead(zipPath))
                {
                    using (gzipStream = new GZipInputStream(inStream))
                    {
                        tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
                        tarArchive.ExtractContents(targetDir);
                        tarArchive.Close();
                    }
                }
            }
            catch
            {
                //出错
            }
            finally
            {
                if (tarArchive != null) tarArchive.Close();
                if (gzipStream != null) gzipStream.Close();
                if (inStream != null) inStream.Close();
            }
        }

        public async Task UploadFileAsync(string localPath, string targetPath)
        {
            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);

            string bucket = $"{BucketName}-{Appid}";

            COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucket, targetPath);

            uploadTask.SetSrcPath(localPath);

            uploadTask.progressCallback = delegate (long completed, long total)
            {
                //Console.WriteLine(string.Format("progress = {0:##.##}%", completed * 100.0 / total));
            };

            COSXMLUploadTask.UploadTaskResult result = await transferManager.UploadAsync(uploadTask);

            try
            {
                COSXMLUploadTask.UploadTaskResult r = await transferManager.UploadAsync(uploadTask);
                //Console.WriteLine(result.GetResultInfo());
                string eTag = r.eTag;
                //到这里应该是成功了，但是因为我没有试过，也不知道具体情况，可能还要根据result的内容判断
            }
            catch (Exception ex)
            {
                Exceptions.Push(ex);
                throw;
            }

        }
    }
}
