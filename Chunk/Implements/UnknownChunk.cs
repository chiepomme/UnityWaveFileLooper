using System.IO;
using System.Text;

namespace WaveFile
{
    public class UnknownChunk : IWaveFileChunk
    {
        public string Id { get; private set; }
        public int PayloadSize { get { return Payload.Length; } }
        public byte[] Payload { get; private set; }

        public UnknownChunk(string id, byte[] payload)
        {
            Id = id;
            Payload = payload;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Encoding.ASCII.GetBytes(Id));
            writer.Write(PayloadSize);
            writer.Write(Payload);
        }
    }
}