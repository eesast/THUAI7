﻿using COSXML;
using COSXML.Auth;
using COSXML.CosException;
using COSXML.Model.Object;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Formats.Tar;
using COSXML.Common;
using COSXML.Transfer;
using System;

// 禁用对没有调用异步API的异步函数的警告
#pragma warning disable CS1998

namespace installer.Model
{
    public class Tencent_Cos
    {
        public string Appid { get; init; }      // 设置腾讯云账户的账户标识（APPID）
        public string Region { get; init; }     // 设置一个默认的存储桶地域
        public string BucketName { get; set; }
        public ExceptionStack Exceptions { get; set; }
        public Logger Log;
        public Logger LogError;

        protected CosXmlConfig config;
        protected CosXmlServer cosXml;

        public Tencent_Cos(string appid, string region, string bucketName, Logger? _log = null, Logger? _logError = null)
        {
            Appid = appid; Region = region; BucketName = bucketName;
            Log = _log ?? LoggerProvider.FromConsole();
            LogError = _logError ?? Log;
            Exceptions = new ExceptionStack(LogError, this);
            // 初始化CosXmlConfig（提供配置SDK接口）
            config = new CosXmlConfig.Builder()
                        .IsHttps(true)      // 设置默认 HTTPS 请求
                        .SetAppid(Appid)    // 设置腾讯云账户的账户标识 APPID
                        .SetRegion(Region)  // 设置一个默认的存储桶地域
                        .SetDebugLog(true)  // 显示日志
                        .Build();           // 创建 CosXmlConfig 对象
            QCloudCredentialProvider cosCredentialProvider = new DefaultQCloudCredentialProvider("***", "***", 1000);
            cosXml = new CosXmlServer(config, cosCredentialProvider);
        }

        public void UpdateSecret(string secretId, string secretKey, long durationSecond = 1000)
        {
            QCloudCredentialProvider cosCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);
            cosXml = new CosXmlServer(config, cosCredentialProvider);
        }

        public void UpdateSecret(string secretId, string secretKey, long durationSecond, string token)
        {
            QCloudCredentialProvider cosCredentialProvider = new DefaultSessionQCloudCredentialProvider(
                secretId, secretKey, durationSecond, token
            );
            cosXml = new CosXmlServer(config, cosCredentialProvider);
        }

        public async Task<int> DownloadFileAsync(string savePath, string? remotePath = null)
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
                    Log.LogInfo(thID, $"{savePath} has existed. Original file has been deleted.");
                }
                string bucket = $"{BucketName}-{Appid}";                                // 格式：BucketName-APPID
                string localDir = Path.GetDirectoryName(savePath)     // 本地文件夹
                    ?? throw new Exception("本地文件夹路径获取失败");
                string localFileName = Path.GetFileName(savePath);    // 指定本地保存的文件名
                remotePath = remotePath?.Replace('\\', '/')?.TrimStart('.', '/');
                GetObjectRequest request = new GetObjectRequest(bucket, remotePath ?? localFileName, localDir, localFileName);

                Dictionary<string, string> test = request.GetRequestHeaders();
                // 执行请求
                GetObjectResult result = cosXml.GetObject(request);
                // 请求成功
                if (result.httpCode != 200)
                    throw new Exception($"Download task: {{\"{remotePath}\"->\"{savePath}\"}} failed, message: {result.httpCode} {result.httpMessage}");
            }
            catch (Exception ex)
            {
                Exceptions.Push(ex, thID);
            }
            Log.LogInfo(thID, $"Download task: {{\"{remotePath}\"->\"{savePath}\"}} finished.");
            return thID;
        }

        public async Task<int> DownloadQueueAsync(string basePath, IEnumerable<string> queue, ConcurrentQueue<string> downloadFailed)
        {
            int thID = Log.StartNew();
            Log.LogInfo(thID, "Batch download task started.");
            var array = queue.ToArray();
            int count = array.Count();
            int finished = 0;
            if (count == 0)
                return 0;
            var partitionar = Partitioner.Create(0, count);
            Parallel.ForEach(partitionar, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (loopState.IsStopped)
                        break;
                    string local = Path.Combine(basePath, array[i]);
                    int subID = -1;
                    try
                    {
                        subID = DownloadFileAsync(local, array[i]).Result;
                    }
                    catch (Exception ex)
                    {
                        downloadFailed.Enqueue(array[i]);
                        Exceptions.Push(ex);
                    }
                    finally
                    {
                        Interlocked.Increment(ref finished);
                        Log.LogInfo(thID, $"Child process: {subID} finished.");
                    }
                }
            });
            Log.LogInfo(thID, "Batch download task finished.");
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

        public async Task UploadFileAsync(string localPath, string targetPath)
        {
            int thID = Log.StartNew();
            Log.LogInfo(thID, $"Upload task: {{\"{localPath}\"->\"{targetPath}\"}} started.");
            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);

            string bucket = $"{BucketName}-{Appid}";

            COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucket, targetPath);

            uploadTask.SetSrcPath(localPath);

            uploadTask.progressCallback = delegate (long completed, long total)
            {
                if (completed == 1.0)
                    Log.LogInfo(thID, $"[Upload: {targetPath}] progress = {completed * 100.0 / total:##.##}%");
            };

            COSXMLUploadTask.UploadTaskResult result = await transferManager.UploadAsync(uploadTask);

            try
            {
                COSXMLUploadTask.UploadTaskResult r = await transferManager.UploadAsync(uploadTask);
                if (r.httpCode != 200)
                    Exceptions.Push(new Exception($"Upload task: {{\"{localPath}\"->\"{targetPath}\"}} failed, message: {r.httpMessage}"), thID);
                string eTag = r.eTag;
                //到这里应该是成功了，但是因为我没有试过，也不知道具体情况，可能还要根据result的内容判断
            }
            catch (Exception ex)
            {
                Exceptions.Push(ex);
            }
            Log.LogInfo(thID, $"Upload task: {{\"{localPath}\"->\"{targetPath}\"}} finished.");
        }
    }
}
