using System;

[Serializable]
public struct RecordingSettings
{
    public int frameRate;
    public float clipLength;
    public int cameraAmount;
    public int maxBatchSize;
    public float radius;
    public string recordingOutputFolder;
    public string metaOutputFolder;
}