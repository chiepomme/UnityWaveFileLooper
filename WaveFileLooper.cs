using WaveFile;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// http://www.blitter.com/~russtopia/MIDI/~jglatt/tech/wave.htm
public class WaveFileLooper : EditorWindow
{
    [MenuItem("Window/WaveFileLooper")]
    static void Init()
    {
        var window = (WaveFileLooper)GetWindow(typeof(WaveFileLooper));
        window.Show();
    }

    string previousPath;
    RiffChunk riffChunk;

    void OnGUI()
    {
        var selectedClip = Selection.activeObject as AudioClip;
        if (selectedClip == null) return;

        var path = AssetDatabase.GetAssetPath(selectedClip);
        if (string.IsNullOrEmpty(path)) return;

        if (path != previousPath || riffChunk == null)
        {
            riffChunk = ReadWaveFile(path);
            previousPath = path;
        }

        var samplerChunk = riffChunk.Chunks.OfType<SamplerChunk>().FirstOrDefault();
        if (samplerChunk == null)
        {
            samplerChunk = new SamplerChunk();
            riffChunk.Chunks.Add(samplerChunk);
        }

        var sampleLoop = samplerChunk.SampleLoops.FirstOrDefault();
        if (sampleLoop == null)
        {
            sampleLoop = new SamplerChunk.SampleLoop();
            samplerChunk.SampleLoops.Add(sampleLoop);
        }

        sampleLoop.LoopStart = EditorGUILayout.IntField("loop start (samples)", sampleLoop.LoopStart);
        sampleLoop.LoopEnd = EditorGUILayout.IntField("loop end (samples)", sampleLoop.LoopEnd);

        if (GUILayout.Button("Save"))
        {
            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                riffChunk.Write(writer);
            }

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    void OnSelectionChange()
    {
        Repaint();
    }

    RiffChunk ReadWaveFile(string path)
    {
        RiffChunk riffChunk;

        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        using (var reader = new BinaryReader(fs))
        {
            riffChunk = (RiffChunk)WaveFileChunkReader.Read(reader);
        }

        return riffChunk;
    }
}
