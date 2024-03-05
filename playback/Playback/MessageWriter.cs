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

        private readonly CodedOutputStream cos; // Protobuf类型二进制输出流

        public bool Disposed { get; private set; } = false;

        public MessageWriter(string fileName, uint teamCount, uint playerCount)
        {
            Utils.FileNameRegular(ref fileName);
            FileStream fs = File.Create(fileName);
            FileName = fs.Name;
            fs.WriteHeader(teamCount, playerCount);
            GZipStream gzs = new(fs, CompressionMode.Compress);
            cos = new(gzs);
        }

        public void WriteOne(MessageToClient msg)
        {
            if (Disposed) return;
            cos.WriteMessage(msg);
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
