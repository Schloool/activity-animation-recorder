using System;
using System.Collections.Generic;

[Serializable]
public struct RecordingSettings
{
    public int frameRate;
    public float clipLength;
    public int cameraAmount;
    public int maxBatchSize;
    public bool saveRigData;
    public string recordingOutputFolder;
    public string metaOutputFolder;
    
    public List<float> radiusList;
    public CharacterList characterList;
    public AnimationList animationList;
}