using System.Text;
using Unity.Profiling;
using UnityEngine;

public class MemoryStats : MonoBehaviour
{
    public static long weaveDuration=0;
    string statsText;
    ProfilerRecorder totalMemoryRecorder;
    ProfilerRecorder systemUsedMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder totalReservedMemoryRecorder;

    void OnEnable()
    {
        totalMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
        totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 30);
    }

    void OnDisable()
    {
        totalMemoryRecorder.Dispose();
        totalReservedMemoryRecorder.Dispose();
        systemUsedMemoryRecorder.Dispose();
    }

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;
        }

        r *= (1e-6f);
        return r;
    }
    void Update()
    {
        var sb = new StringBuilder(500);
        sb.AppendLine($"Frames per Second: {1000.0f / (GetRecorderFrameAverage(mainThreadTimeRecorder) ):F0} fps");
        sb.AppendLine($"Total Used Memory: {totalMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"Total Reserved Memory: {totalReservedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        statsText = sb.ToString();
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(Screen.width-260, 25, 250, 50), statsText);
    }


}