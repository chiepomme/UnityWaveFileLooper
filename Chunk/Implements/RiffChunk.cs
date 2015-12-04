using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WaveFile
{
    public class RiffChunk : IWaveFileChunk
    {
        public const string Id = "RIFF";
        string IWaveFileChunk.Id { get { return Id; } }

        // WAVE Header + Chunks(ChunkId + PayloadSize + Payload)
        public int PayloadSize { get { return 4 + Chunks.Select(c => 4 + 4 + c.PayloadSize).Sum(); } }
        public List<IWaveFileChunk> Chunks { get; private set; }

        public RiffChunk(byte[] payload)
        {
            Chunks = new List<IWaveFileChunk>();

            using (var stream = new MemoryStream(payload))
            using (var reader = new BinaryReader(stream))
            {
                var riffType = Encoding.ASCII.GetString(reader.ReadBytes(4));
                Debug.Assert(riffType == "WAVE");

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    Chunks.Add(WaveFileChunkReader.Read(reader));
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Encoding.ASCII.GetBytes(Id));
            writer.Write(PayloadSize);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));

            foreach (var chunk in Chunks)
            {
                chunk.Write(writer);
            }
        }
    }
}