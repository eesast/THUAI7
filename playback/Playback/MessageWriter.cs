using Google.Protobuf;
using Protobuf;
using System.IO.Compression;

namespace Playback
{
    public class MessageWriter : IDisposable
    {
        /// <summary>
        /// 回放文件绝对路径
        /// </summary>
        public string FileName { get; }

        private readonly BinaryWriter bw;       // 基础类型二进制输出流
        private readonly CodedOutputStream cos; // Protobuf类型二进制输出流
        private readonly GZipStream gzs;        // 压缩输出流

        public bool Disposed { get; private set; } = false;

        public MessageWriter(string fileName, uint teamCount, uint playerCount)
        {
            Utils.FileNameRegular(ref fileName);
            FileStream fs = File.Create(fileName);
            FileName = fs.Name;
            bw = new(fs);
            WriteHeader(teamCount, playerCount);
            gzs = new(fs, CompressionMode.Compress);
            cos = new(gzs);
        }

        private void WriteHeader(uint teamCount, uint playerCount)
        {
            bw.Write(Constants.FileHeader); // 写入文件头
            bw.Write(teamCount);            // 写入队伍数
            bw.Write(playerCount);          // 写入每队玩家人数
        }

        public void WriteOne(MessageToClient msg)
        {
            if (Disposed) return;
            cos.WriteMessage(msg);
        }

        public void Flush()
        {
            bw.Flush();
            cos.Flush();
            gzs.Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                bw.Dispose();
                cos.Dispose();
                gzs.Dispose();
            }
            Disposed = true;
        }

        ~MessageWriter()
        {
            Dispose(false);
        }
    }
}
