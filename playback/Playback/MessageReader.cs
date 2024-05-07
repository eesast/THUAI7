using Google.Protobuf;
using Protobuf;
using System.IO.Compression;

namespace Playback
{
    public class MessageReader : IDisposable
    {
        /// <summary>
        /// 回放文件绝对路径
        /// </summary>
        public string FileName { get; }

        public readonly uint teamCount;
        public readonly uint playerCount;

        private readonly CodedInputStream cis;  // Protobuf类型二进制输入流
        public bool Disposed { get; private set; } = false;

        public MessageReader(string fileName)
        {
            Utils.FileNameRegular(ref fileName);
            FileStream fs = File.OpenRead(fileName);
            FileName = fs.Name;
            (teamCount, playerCount) = fs.ReadHeader();
            GZipStream gzs = new(fs, CompressionMode.Decompress);
            cis = new(gzs);
        }

        public MessageToClient? ReadOne()
        {
            if (Disposed) return null;
            if (cis.IsAtEnd) return null;
            MessageToClient ret = new();
            try
            {
                cis.ReadMessage(ret);
                return ret;
            }
            catch
            {
                return null;
            }
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
                cis.Dispose();
            }
            Disposed = true;
        }

        ~MessageReader()
        {
            Dispose(false);
        }
    }
}
