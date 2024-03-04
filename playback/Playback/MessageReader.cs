using Google.Protobuf;
using Protobuf;
using System.IO.Compression;

namespace Playback
{
    public class FileFormatNotLegalException(string fileName) : Exception
    {
        public string FileName { get; } = fileName;
        public override string Message { get; }
            = $"The file: {fileName} is not a legal playback file for THUAI{Constants.Version}.";
    }

    public class MessageReader : IDisposable
    {
        /// <summary>
        /// 回放文件绝对路径
        /// </summary>
        public string FileName { get; }

        public readonly uint teamCount;
        public readonly uint playerCount;

        private readonly BinaryReader br;       // 基础类型二进制输入流
        private readonly CodedInputStream cis;  // Protobuf类型二进制输入流
        private readonly GZipStream gzs;        // 解压缩输入流

        public bool Disposed { get; private set; } = false;

        public MessageReader(string fileName)
        {
            Utils.FileNameRegular(ref fileName);
            FileStream fs = File.OpenRead(fileName);
            FileName = fs.Name;
            br = new(fs);
            (teamCount, playerCount) = ReadHeader();
            gzs = new(fs, CompressionMode.Decompress);
            cis = new(gzs);
        }

        private (uint teamCount, uint playerCount) ReadHeader()
        {
            if (!br.ReadBytes(Constants.FileHeader.Length)      // 判断文件头
                   .SequenceEqual(Constants.FileHeader))
                throw new FileFormatNotLegalException(FileName);
            return (br.ReadUInt32(), br.ReadUInt32());          // 读取队伍数和每队玩家人数
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
                gzs.Dispose();
            }
            Disposed = true;
        }

        ~MessageReader()
        {
            Dispose(false);
        }
    }
}
