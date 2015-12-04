using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WaveFile
{
    public class SamplerChunk : IWaveFileChunk
    {
        public const string Id = "smpl";
        string IWaveFileChunk.Id { get { return Id; } }

        public class SampleLoop
        {
            public const int Size = sizeof(int) * 6;

            public int LoopIdentifier;
            public int LoopType;
            public int LoopStart;
            public int LoopEnd;
            public int LoopFraction;
            public int LoopPlayCount;
        }

        public int PayloadSize { get { return sizeof(int) * 9 + SampleLoop.Size * SampleLoopCount; } } // int フィールドが 9 + SampleLoop

        public int Manufacturer;
        public int Product;
        public int SamplePeriod;
        public int MIDIUnityNote;
        public int MIDIPitchFraction;
        public int SMPTEFormat;
        public int SMPTEOffset;
        public int SampleLoopCount { get { return SampleLoops.Count; } }
        public int SamplerData;
        public List<SampleLoop> SampleLoops = new List<SampleLoop>();

        public SamplerChunk() { }
        public SamplerChunk(byte[] payload)
        {
            UnityEngine.Debug.Log("sampler chunk");
            UnityEngine.Debug.Log(payload.Length);

            using (var stream = new MemoryStream(payload, false))
            using (var reader = new BinaryReader(stream))
            {
                Manufacturer = reader.ReadInt32();
                Product = reader.ReadInt32();
                SamplePeriod = reader.ReadInt32();
                MIDIUnityNote = reader.ReadInt32();
                MIDIPitchFraction = reader.ReadInt32();
                SMPTEFormat = reader.ReadInt32();
                SMPTEOffset = reader.ReadInt32();
                var sampleLoopCount = reader.ReadInt32();
                SamplerData = reader.ReadInt32();

                for (var i = 0; i < sampleLoopCount; i++)
                {
                    UnityEngine.Debug.Log(i);
                    var sampleLoop = new SampleLoop();
                    sampleLoop.LoopIdentifier = reader.ReadInt32();
                    sampleLoop.LoopType = reader.ReadInt32();
                    sampleLoop.LoopStart = reader.ReadInt32();
                    sampleLoop.LoopEnd = reader.ReadInt32();
                    sampleLoop.LoopFraction = reader.ReadInt32();
                    sampleLoop.LoopPlayCount = reader.ReadInt32();

                    SampleLoops.Add(sampleLoop);
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Encoding.ASCII.GetBytes(Id));
            writer.Write(PayloadSize);

            writer.Write(Manufacturer);
            writer.Write(Product);
            writer.Write(SamplePeriod);
            writer.Write(MIDIUnityNote);
            writer.Write(MIDIPitchFraction);
            writer.Write(SMPTEFormat);
            writer.Write(SMPTEOffset);
            writer.Write(SampleLoopCount);
            writer.Write(SamplerData);

            foreach (var sampleLoop in SampleLoops)
            {
                writer.Write(sampleLoop.LoopIdentifier);
                writer.Write(sampleLoop.LoopType);
                writer.Write(sampleLoop.LoopStart);
                writer.Write(sampleLoop.LoopEnd);
                writer.Write(sampleLoop.LoopFraction);
                writer.Write(sampleLoop.LoopPlayCount);
            }
        }
    }
}