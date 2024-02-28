using Google.Protobuf;
using Protobuf;
using System.IO.Compression;

namespace Playback
{
    using PBConst = PlayBackConstant;
    public class MessageWriter : IDisposable
    {
        private readonly FileStream fs;
        private readonly CodedOutputStream cos;
        private readonly MemoryStream ms;
        private readonly GZipStream gzs;

        private static void ClearMemoryStream(MemoryStream msToClear)
        {
            msToClear.Position = 0;
            msToClear.SetLength(0);
        }

        public MessageWriter(string fileName, uint teamCount, uint playerCount)
        {
            if (!fileName.EndsWith(PBConst.ExtendedName))
            {
                fileName += PBConst.ExtendedName;
            }

            fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            fs.Write(PBConst.FileHeader);                   // 写入文件头
            fs.Write(BitConverter.GetBytes(teamCount));     // 写入队伍数
            fs.Write(BitConverter.GetBytes(playerCount));   // 写入每队的玩家人数
            ms = new MemoryStream(PBConst.BufferMaxSize);
            cos = new CodedOutputStream(ms);
            gzs = new GZipStream(fs, CompressionMode.Compress);
        }

        public void WriteOne(MessageToClient msg)
        {
            cos.WriteMessage(msg);
            if (ms.Length > PBConst.BufferMaxSize)
                Flush();
        }

        public void Flush()
        {
            if (fs.CanWrite)
            {
                cos.Flush();
                gzs.Write(ms.GetBuffer(), 0, (int)ms.Length);
                gzs.Flush();
                ClearMemoryStream(ms);
                fs.Flush();
            }
        }

        public void Dispose()
        {
            if (fs.CanWrite)
            {
                Flush();
                cos.Dispose();
                gzs.Dispose();
                fs.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~MessageWriter()
        {
            Dispose();
        }
    }
}
