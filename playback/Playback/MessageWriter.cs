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

        /// <summary>
        /// 写入消息数量
        /// </summary>
        public uint WrittenNum { get; private set; } = 0;
        private readonly uint FlushNum; // 刷新间隔

        private readonly CodedOutputStream cos; // Protobuf类型二进制输出流
        public bool Disposed { get; private set; } = false;

        public MessageWriter(string fileName, uint teamCount, uint playerCount, uint flushNum = 500)
        {
            Utils.FileNameRegular(ref fileName);
            FileStream fs = File.Create(fileName);
            FileName = fs.Name;
            fs.WriteHeader(teamCount, playerCount);
            GZipStream gzs = new(fs, CompressionMode.Compress);
            cos = new(gzs);
            FlushNum = flushNum;
        }

        public void WriteOne(MessageToClient msg)
        {
            if (Disposed) return;
            cos.WriteMessage(msg);
            WrittenNum++;
            if (WrittenNum % FlushNum == 0)
            {
                Flush();
                WrittenNum = 0;
            }
        }

        public void Flush()
        {
            cos.Flush();
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
                cos.Dispose();
            }
            Disposed = true;
        }

        ~MessageWriter()
        {
            Dispose(false);
        }
    }
}
