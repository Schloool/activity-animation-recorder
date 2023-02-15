using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandLineApplicationAdapter : MonoBehaviour
{
    private const float TimeBuffer = 0.5f;

    [SerializeField] private List<GameObject> destroyOnUse;
    [SerializeField] private RecordingSettings settings;

    private IEnumerator Start()
    {
        if (!Environment.GetCommandLineArgs().ToList().Contains("--record")) yield break;

        destroyOnUse.ForEach(Destroy);
        
        yield return null;
            
        var videoRecorder = FindObjectOfType<VideoRecorder>();

        StartCoroutine(videoRecorder.RecordingRoutine(settings));

        var characterAmount = FindObjectOfType<CharacterChoice>().characterList.items.Count;
        yield return new WaitForSeconds(settings.clipLength * characterAmount + TimeBuffer);
        Application.Quit();
    }

    public void ParseCameraAmount(string amountString)
    {
        if (!int.TryParse(amountString, out var amount)) return;
        settings.cameraAmount = amount;
    }
    
    public void ParseFPS(string fpsString)
    {
        if (!int.TryParse(fpsString, out var fps)) return;
        settings.frameRate = fps;
    }
    
    public void ParseClipLength(string lengthString)
    {
        if (!float.TryParse(lengthString, out var length)) return;
        settings.clipLength = length;
    }

    public void ParseRadius(string radiusString)
    {
        if (!float.TryParse(radiusString, out var radius)) return;
        settings.radius = radius;
    }

    public void SetRecordingOutputPath(string outputFolder)
    {
        settings.recordingOutputFolder = outputFolder;
    }
    
    public void SetMetadataOutputPath(string outputFolder)
    {
        settings.metaOutputFolder = outputFolder;
    }
}