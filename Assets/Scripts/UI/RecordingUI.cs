using System;
using TMPro;
using UnityEngine;

public class RecordingUI : MonoBehaviour
{
    private const int MinFPS = 30, MaxFps = 240;
    private const float MinClipLength = 0.5f, MaxClipLength = 5f;
    private const int MinCameras = 1, MaxCameras = 30;

    [SerializeField] [Tooltip("The amount of cameras used to record footage.")]
    private int cameraAmount = 5;
    
    [SerializeField] [Tooltip("The length in seconds one video clip will have.")]
    private float clipLengthInSeconds = 2f;
    
    [SerializeField] [Tooltip("The framerate in which the videos will be recorded.")]
    private int frameRate = 30;

    [SerializeField] [Tooltip("The text used to display the camera amount value.")]
    private TMP_Text cameraAmountText;
    
    [SerializeField] [Tooltip("The text used to display the recording length value.")]
    private TMP_Text recordingLengthText;
    
    [SerializeField] [Tooltip("The text used to display the FPS recording value.")]
    private TMP_Text fpsText;
    
    [SerializeField] [Tooltip("The UI which will be deactivated when starting the recording")]
    private GameObject uiParent;

    private VideoRecorder _recorder;

    private void Start()
    {
        _recorder = FindObjectOfType<VideoRecorder>();
        _recorder.OnFinishRecording += HandleRecordingEnd;
    }

    private void OnDestroy()
    {
        if (_recorder == null) return;
        
        _recorder.OnFinishRecording -= HandleRecordingEnd;
    }

    /// <summary>
    /// Starts a recording for all cameras.
    /// </summary>
    public void RecordAllCameras()
    {
        var settings = new RecordingSettings()
        {
            frameRate = frameRate,
            clipLength = clipLengthInSeconds,
            cameraAmount = cameraAmount,
            recordingOutputFolder = "Recordings",
            metaOutputFolder = "Recordings/Meta",
            radius = 6f,
        };
        
        StartCoroutine(_recorder.RecordingRoutine(settings));
        uiParent.SetActive(false);
    }

    private void HandleRecordingEnd()
    {
        uiParent.SetActive(true);
    }

    public void AddCamera(int amount)
    {
        cameraAmount = amount > 0 ? Math.Min(cameraAmount + amount, MaxCameras) : Math.Max(MinCameras, cameraAmount + amount);
        cameraAmountText.text = $"{cameraAmount} Cams";
    }
    
    public void AddClipLength(float amount)
    {
        clipLengthInSeconds = amount > 0 ? Math.Min(clipLengthInSeconds + amount, MaxClipLength) 
            : Math.Max(MinClipLength, clipLengthInSeconds + amount);
        recordingLengthText.text = $"{clipLengthInSeconds} Sec";
    }
    
    public void AddFPS(int amount)
    {
        frameRate = amount > 0 ? Math.Min(frameRate + amount, MaxFps) : Math.Max(MinFPS, frameRate + amount);
        fpsText.text = $"{frameRate} FPS";
    }
}