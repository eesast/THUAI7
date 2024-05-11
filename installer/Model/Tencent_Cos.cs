using COSXML;
using COSXML.Auth;
using COSXML.CosException;
using COSXML.Model.Object;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Formats.Tar;
using COSXML.Common;
using COSXML.Transfer;
using System;
using installer.Data;
using System.Threading.Tasks;
using COSXML.Model.Bucket;
using COSXML.Model.Tag;

// 禁用对没有调用异步API的异步函数的警告
#pragma warning disable CS1998

namespace installer.Model
{
    public class Tencent_Cos
    {
        public string Appid { get; init; }      // 设置腾讯云账户的账户标识（APPID）
        public string Region { get; init; }     // 设置一个默认的存储桶地域
        public string BucketName { get; set; }
        public Logger Log;

        protected CosXmlConfig config;
        protected CosXmlServer cosXml;
        protected TransferConfig transfer;
        protected TransferManager manager;

        public DownloadReport Report;

        public Tencent_Cos(string appid, string region, string bucketName, Logger? _log = null)
        {
            Appid = appid; Region = region; BucketName = bucketName;
            Log = _log ?? LoggerProvider.FromConsole();
            Log.PartnerInfo = "[COS]";
            Report = new DownloadReport();
            // 初始化CosXmlConfig（提供配置SDK接口）
            config = new CosXmlConfig.Builder()
                        .IsHttps(true)      // 设置默认 HTTPS 请求
                        .SetAppid(Appid)    // 设置腾讯云账户的账户标识 APPID
                        .SetRegion(Region)  // 设置一个默认的存储桶地域
                        .SetDebugLog(true)  // 显示日志
                        .Build();           // 创建 CosXmlConfig 对象
            QCloudCredentialProvider cosCredentialProvider = new DefaultQCloudCredentialProvider("***", "***", 1000);
            cosXml = new CosXmlServer(config, cosCredentialProvider);
            transfer = new TransferConfig()
            {
                DivisionForDownload = 20 << 20,     // 下载分块阈值为20MB
                SliceSizeForDownload = 10 << 20,    // 下载分块大小为10MB
            };
            manager = new TransferManager(cosXml, transfer);
        }

        public void UpdateSecret(string secretId, string secretKey, long durationSecond = 1000)
        {
            QCloudCredentialProvider cosCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);
            cosXml = new CosXmlServer(config, cosCredentialProvider);
            manager = new TransferManager(cosXml, transfer);
        }

        public void UpdateSecret(string secretId, string secretKey, long durationSecond, string token)
        {
            QCloudCredentialProvider cosCredentialProvider = new DefaultSessionQCloudCredentialProvider(
                secretId, secretKey, durationSecond, token
            );
            cosXml = new CosXmlServer(config, cosCredentialProvider);
            manager = new TransferManager(cosXml, transfer);
        }

        public int DownloadFile(string savePath, string? remotePath = null)
        {
            int thID = Log.StartNew();
            // download_dir标记根文件夹路径，key为相对根文件夹的路径（不带./）
            // 创建存储桶
            try
            {
                Log.LogInfo(thID, $"Download task: {{\"{remotePath}\"->\"{savePath}\"}} started.");
                // 覆盖对应文件，如果无法覆盖则报错
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                    Log.LogWarning(thID, $"{savePath} has existed. Original file has been deleted.");
                }
                string bucket = $"{BucketName}-{Appid}";                                // 格式：BucketName-APPID
                string localDir = Path.GetDirectoryName(savePath)     // 本地文件夹
                    ?? throw new Exception("本地文件夹路径获取失败");
                string localFileName = Path.GetFileName(savePath);    // 指定本地保存的文件名
                remotePath = remotePath?.Replace('\\', '/')?.TrimStart('.').TrimStart('/');
                var head = cosXml.HeadObject(new HeadObjectRequest(bucket, remotePath ?? localFileName));
                long c = 0;
                if (head.size > (1 << 20))
                {
                    // 文件大小大于1MB则设置回调函数
                    Report.Total = head.size;
                    Report.Completed = 0;
                    Report.BigFileTraceEnabled = true;
                    var size = (head.size > 1 << 30) ?
                        string.Format("{0:##.#}GB", ((double)head.size) / (1 << 30)) :
                        string.Format("{0:##.#}MB", ((double)head.size) / (1 << 20));
                    Log.LogWarning($"Big file({size}) detected! Please keep network steady!");
                    COSXMLDownloadTask task = new COSXMLDownloadTask(bucket, remotePath ?? localFileName, localDir, localFileName);
                    task.progressCallback = (completed, total) =>
                    {
                        if (completed > 1 << 30 && completed - c > 100 << 20)
                        {
                            Log.LogDebug(string.Format("downloaded = {0:##.#}GB, progress = {1:##.##}%", ((double)completed) / (1 << 30), completed * 100.0 / total));
                            c = completed;
                        }
                        if (completed < 1 << 30 && completed - c > 10 << 20)
                        {
                            Log.LogDebug(string.Format("downloaded = {0:##.#}MB, progress = {1:##.##}%", ((double)completed) / (1 << 20), completed * 100.0 / total));
                            c = completed;
                        }
                        (Report.Completed, Report.Total) = (completed, total);
                    };
                    // 执行请求                
                    var result = manager.DownloadAsync(task).Result;
                    // 请求成功
                    if (result is not null && result.httpCode != 200 && result.httpCode != 206)
                        throw new Exception($"Download task: {{\"{remotePath}\"->\"{savePath}\"}} failed, message: {result.httpCode} {result.httpMessage}");
                    Log.LogDebug(thID, $"Download task: {{\"{remotePath}\"->\"{savePath}\"}} finished.");
                }
                else
                {
                    if (Report.Completed > 0 && Report.Total > 0 && Report.Completed == Report.Total)
                        Report.BigFileTraceEnabled = false;
                    var request = new GetObjectRequest(bucket, remotePath ?? localFileName, localDir, localFileName);
                    // 执行请求                
                    var result = cosXml.GetObject(request);
                    // 请求成功
                    if (result.httpCode != 200 && result.httpCode != 206)
                        throw new Exception($"Download task: {{\"{remotePath}\"->\"{savePath}\"}} failed, message: {result.httpCode} {result.httpMessage}");
                    Log.LogDebug(thID, $"Download task: {{\"{remotePath}\"->\"{savePath}\"}} finished.");

                }

                if (Report.BigFileTraceEnabled)
                    Report.Completed = Report.Total;
            }
            catch (Exception ex)
            {
                Log.LogError(thID, ex.Message);
                Log.LogDebug(thID, $"Download task: {{\"{remotePath}\"->\"{savePath}\"}} ended unexpectedly.");
                thID = -1;
            }
            return thID;
        }

        public int DownloadQueue(string basePath, IEnumerable<string> queue)
        {
            int thID = Log.StartNew();
            Log.LogDebug(thID, "Batch download task started.");
            var array = queue.ToArray();
            var count = array.Length;
            if (count == 0)
                return 0;
            var comCount = 0;
            var comCountOld = Report.ComCount;
            Report.Count += count;

            var partitionar = Partitioner.Create(0, count, count / 4 > 0 ? count / 4 : count);
            Parallel.ForEach(partitionar, (range, loopState) =>
            {
                for (long i = range.Item1; i < range.Item2; i++)
                {
                    if (loopState.IsStopped)
                        break;
                    string local = Path.Combine(basePath, array[i]);
                    int subID = -1;
                    try
                    {
                        subID = DownloadFile(local, array[i]);
                    }
                    catch (Exception ex)
                    {
                        Log.LogError(ex.Message + " on " + array[i]);
                    }
                    finally
                    {
                        Interlocked.Increment(ref comCount);
                        Report.ComCount = comCount + comCountOld;
                        Log.LogInfo(thID, $"Child process: {subID} finished.");
                    }
                }
            });
            Log.LogInfo(thID, "Batch download task finished.");
            Report.ComCount = comCount + comCountOld;
            return thID;
        }

        public void ArchieveUnzip(string zipPath, string targetDir)
        {
            Stream? inStream = null;
            Stream? gzipStream = null;
            int thID = Log.StartNew();
            Log.LogInfo(thID, $"Zip {zipPath} is being extracted...");
            try
            {
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                using (inStream = File.OpenRead(zipPath))
                {
                    using (gzipStream = new GZipStream(inStream, CompressionMode.Decompress))
                    {
                        TarFile.ExtractToDirectory(gzipStream, targetDir, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(thID, ex.Message);
            }
            finally
            {
                if (gzipStream != null) gzipStream.Close();
                if (inStream != null) inStream.Close();
                Log.LogInfo(thID, $"Zip has been extracted to {targetDir}");
            }
        }

        public void UploadFile(string localPath, string targetPath)
        {
            int thID = Log.StartNew();
            Log.LogInfo(thID, $"Upload task: {{\"{localPath}\"->\"{targetPath}\"}} started.");

            string bucket = $"{BucketName}-{Appid}";

            COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucket, targetPath);

            targetPath = targetPath.TrimStart('.').TrimStart('/');

            uploadTask.SetSrcPath(localPath);

            try
            {
                COSXMLUploadTask.UploadTaskResult r = manager.UploadAsync(uploadTask).Result;
                if (r.httpCode != 200)
                    Log.LogError(thID, $"Upload task: {{\"{localPath}\"->\"{targetPath}\"}} failed, message: {r.httpMessage}");
                string eTag = r.eTag;
                //到这里应该是成功了，但是因为我没有试过，也不知道具体情况，可能还要根据result的内容判断
                Log.LogInfo(thID, $"Upload task: {{\"{localPath}\"->\"{targetPath}\"}} finished.");
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }
        }

        public bool DetectFile(string remotePath)
        {
            string bucket = $"{BucketName}-{Appid}";
            remotePath = remotePath.TrimStart('.').TrimStart('/');
            //执行请求
            try
            {
                DoesObjectExistRequest requestd = new DoesObjectExistRequest(bucket, remotePath);
                return cosXml.DoesObjectExist(requestd);
            }
            catch (CosClientException clientEx)
            {
                Log.LogError("CosClientException: " + clientEx);
                return false;
            }
            catch (CosServerException serverEx)
            {
                Log.LogError("CosServerException: " + serverEx.GetInfo());
                return false;
            }
        }

        public void DeleteFile(string remotePath)
        {
            int thID = Log.StartNew();
            Log.LogInfo(thID, $"Delete task: \"{remotePath}\" started.");
            string bucket = $"{BucketName}-{Appid}";
            remotePath = remotePath.TrimStart('.').TrimStart('/');
            //执行请求
            try
            {
                if (!DetectFile(remotePath))
                {
                    Log.LogWarning($"{remotePath} doesn't exist!");
                    return;
                }
                DeleteObjectRequest request = new DeleteObjectRequest(bucket, remotePath);
                DeleteObjectResult result = cosXml.DeleteObject(request);
                Log.LogInfo(thID, $"Delete task: \"{remotePath}\" finished.");
            }
            catch (CosClientException clientEx)
            {
                //请求失败
                Log.LogError("CosClientException: " + clientEx);
            }
            catch (CosServerException serverEx)
            {
                //请求失败
                Log.LogError("CosServerException: " + serverEx.GetInfo());
            }
        }

        public List<string> EnumerateDir(string remotePath)
        {
            int thID = Log.StartNew();
            var result = new List<string>();
            string bucket = $"{BucketName}-{Appid}";
            remotePath = remotePath.TrimStart('.').TrimStart('/');
            Log.LogInfo(thID, $"Enumerate files in {remotePath}");

            bool truncated = false;
            string marker = string.Empty;
            do
            {
                GetBucketRequest request = new GetBucketRequest(bucket);
                request.SetPrefix(remotePath);
                request.SetDelimiter("/");
                if (!string.IsNullOrEmpty(marker))
                    request.SetMarker(marker);
                //执行请求
                GetBucketResult res = cosXml.GetBucket(request);
                ListBucket info = res.listBucket;
                result.AddRange(info.contentsList.Select(i => i.key).Where(i => i != remotePath));
                foreach (var dir in info.commonPrefixesList)
                {
                    result.AddRange(EnumerateDir(dir.prefix));
                }
                truncated = info.isTruncated;
                marker = info.nextMarker;
            } while (truncated);

            return result;
        }
        #region 异步方法包装
        public Task<int> DownloadFileAsync(string savePath, string? remotePath = null)
        {
            return Task.Run(() => DownloadFile(savePath, remotePath));
        }

        public Task<int> DownloadQueueAsync(string basePath, IEnumerable<string> queue)
        {
            return Task.Run(() => DownloadQueue(basePath, queue));
        }
        public Task ArchieveUnzipAsync(string zipPath, string targetDir)
        {
            return Task.Run(() => ArchieveUnzip(zipPath, targetDir));
        }

        public Task UploadFileAsync(string localPath, string targetPath)
        {
            return Task.Run(() => UploadFile(localPath, targetPath));
        }

        public Task DeleteFileAsync(string remotePath)
        {
            return Task.Run(() => DeleteFile(remotePath));
        }

        #endregion
    }
}
