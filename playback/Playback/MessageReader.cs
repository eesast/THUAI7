using Google.Protobuf;
using Protobuf;
using System.IO.Compression;

namespace Playback
{
    using PBConst = PlayBackConstant;
    public class FileFormatNotLegalException(string fileName) : Exception
    {
        private readonly string fileName = fileName;
        public override string Message => $"The file: {fileName} is not a legal playback file for THUAI{PBConst.Version}.";
    }

    public class MessageReader : IDisposable
    {
        private readonly FileStream fs;
        private CodedInputStream cis;
        private readonly GZipStream gzs;
        private byte[] buffer;
        public bool Finished { get; private set; } = false;

        public readonly uint teamCount;
        public readonly uint playerCount;

        public MessageReader(string fileName)
        {
            if (!fileName.EndsWith(PBConst.ExtendedName))
            {
                fileName += PBConst.ExtendedName;
            }

            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            try
            {
                var headerLen = PBConst.FileHeader.Length;
                byte[] bt = new byte[headerLen + sizeof(uint) * 2];
                fs.Read(bt, 0, bt.Length);
                for (int i = 0; i < headerLen; ++i)
                {
                    if (bt[i] != PBConst.FileHeader[i]) throw new FileFormatNotLegalException(fileName);
                }

                teamCount = BitConverter.ToUInt32(bt, headerLen);
                playerCount = BitConverter.ToUInt32(bt, headerLen + sizeof(uint));
            }
            catch
            {
                throw new FileFormatNotLegalException(fileName);
            }

            gzs = new GZipStream(fs, CompressionMode.Decompress);
            var tmpBuffer = new byte[PBConst.BufferMaxSize];
            var bufferSize = gzs.Read(tmpBuffer);
            if (bufferSize == 0)
            {
                buffer = tmpBuffer;
                Finished = true;
            }
            else if (bufferSize != PBConst.BufferMaxSize)       // 不留空位，防止 CodedInputStream 获取信息错误
            {
                if (bufferSize == 0)
                {
                    Finished = true;
                }
                buffer = new byte[bufferSize];
                Array.Copy(tmpBuffer, buffer, bufferSize);
            }
            else
            {
                buffer = tmpBuffer;
            }
            cis = new CodedInputStream(buffer);
        }

        public MessageToClient? ReadOne()
        {
        beginRead:
            if (Finished)
                return null;
            var pos = cis.Position;
            try
            {
                MessageToClient? msg = new();
                cis.ReadMessage(msg);
                return msg;
            }
            catch (InvalidProtocolBufferException)
            {
                var leftByte = buffer.Length - pos;     // 上次读取剩余的字节
                if (buffer.Length < PBConst.BufferMaxSize / 2)
                {
                    var newBuffer = new byte[PBConst.BufferMaxSize];
                    for (int i = 0; i < leftByte; i++)
                    {
                        newBuffer[i] = buffer[pos + i];
                    }
                    buffer = newBuffer;
                }
                else
                {
                    for (int i = 0; i < leftByte; ++i)
                    {
                        buffer[i] = buffer[pos + i];
                    }
                }
                var bufferSize = gzs.Read(buffer, (int)leftByte, (int)(buffer.Length - leftByte)) + leftByte;
                if (bufferSize == leftByte)
                {
                    Finished = true;
                    return null;
                }
                if (bufferSize != buffer.Length)        // 不留空位，防止 CodedInputStream 获取信息错误
                {
                    var tmpBuffer = new byte[bufferSize];
                    Array.Copy(buffer, tmpBuffer, bufferSize);
                    buffer = tmpBuffer;
                }
                cis = new CodedInputStream(buffer);
                goto beginRead;
            }
        }

        public void Dispose()
        {
            Finished = true;
            if (fs.CanRead)
            {
                fs.Close();
            }
            GC.SuppressFinalize(this);
        }

        ~MessageReader()
        {
            Dispose();
        }
    }
}
