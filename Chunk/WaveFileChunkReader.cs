using System.IO;
using System.Text;

namespace WaveFile
{
    public static class WaveFileChunkReader
    {
        public static IWaveFileChunk Read(BinaryReader reader)
        {
            var chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
            var payloadSize = reader.ReadInt32();
            var payload = reader.ReadBytes(payloadSize);

            switch (chunkId)
            {
                case RiffChunk.Id: return new RiffChunk(payload);
                case SamplerChunk.Id: return new SamplerChunk(payload);
                default: return new UnknownChunk(chunkId, payload);
            }
        }
    }
}