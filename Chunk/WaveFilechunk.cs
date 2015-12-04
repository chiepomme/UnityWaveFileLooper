using System.IO;

namespace WaveFile
{
    public interface IWaveFileChunk
    {
        string Id { get; }
        int PayloadSize { get; }
        void Write(BinaryWriter writer);
    }
}