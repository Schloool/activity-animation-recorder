using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RenderHeads.Media.AVProMovieCapture;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Component used to record videos from multiple camera perspectives.
/// </summary>
public class VideoRecorder : MonoBehaviour
{
    /// <summary>
    /// Event called when a new set of recordings has been created.
    /// </summary>
    public event Action OnFinishRecording;
    
    [SerializeField] [Tooltip("The Transform which will be focused by cameras.")] 
    private Transform focusObjectTransform;
    
    [SerializeField] [Tooltip("The Transform which will be used as a parent object for cameras.")] 
    private Transform cameraParent;
    
    [SerializeField] [Tooltip("The camera prefab used for scene recording cameras.")]
    private Camera cameraPrefab;

    private List<Camera> _recordingCameras;
    private CameraGenerator _cameraGenerator;

    private void Start()
    {
        _recordingCameras = new List<Camera>();
        _cameraGenerator = new BallhausCameraGenerator(cameraPrefab, cameraParent, focusObjectTransform, _recordingCameras);
    }

    /// <summary>
    /// Coroutine used to record all cameras.
    /// Only cameras outside the main camera will be considered.
    /// </summary>
    public IEnumerator RecordingRoutine(RecordingSettings settings)
    {
        var mainCamera = Camera.main;
        Debug.Assert(mainCamera != null, "No main camera found.");

        var characterChoice = FindObjectOfType<CharacterChoice>();
        Debug.Assert(characterChoice != null, "No character choice found");
        
        _cameraGenerator.GenerateCameras(settings.cameraAmount, settings.radius, focusObjectTransform.position);

        if (!Directory.Exists(settings.recordingOutputFolder)) Directory.CreateDirectory(settings.recordingOutputFolder);
        if (!Directory.Exists(settings.metaOutputFolder)) Directory.CreateDirectory(settings.metaOutputFolder);

        if (settings.maxBatchSize <= 0)
        {
            settings.maxBatchSize = 1;
        }
        
        var characters = characterChoice.characterList.items;
        for (var characterIndex = 0; characterIndex < characters.Count; characterIndex++)
        {
            var captureUnits = _recordingCameras
                .Select(cam => cam.gameObject.AddComponent<CaptureFromCamera>())
                .ToList();
            
            characterChoice.ChooseCharacter(characterIndex);
            yield return null;
            
            var recordingIndex = 0;
            while (captureUnits.Any())
            {
                var currentBatch = captureUnits.Take(settings.maxBatchSize).ToList();
                
                foreach (var capture in currentBatch)
                {
                    recordingIndex++;
                    var fileName = $"rec_{characterIndex}-{recordingIndex}";
                    CaptureCamera(settings, capture, fileName);
                }

                yield return new WaitForSeconds(settings.clipLength); 
                
                captureUnits = captureUnits.Skip(settings.maxBatchSize).ToList();
            }
            yield return new WaitForSeconds(1f);
            
            captureUnits.ForEach(Destroy);
            yield return null;
        }
        
        StartCoroutine(RigMetadataCoroutine(settings));
        OnFinishRecording?.Invoke();
    }

    private void CaptureCamera(RecordingSettings settings, CaptureFromCamera capture, string fileName)
    {
        capture.IsRealTime = false;
        capture.FrameRate = settings.frameRate;
        capture.SelectRecordingResolution(1920, 1080);
        capture.QueueStartCapture();
        capture.AppendFilenameTimestamp = false;
        capture.FilenamePrefix = fileName;
        capture.OutputFolderPath = settings.recordingOutputFolder;

        capture.StartCapture();

        StartCoroutine(RecordingStopRoutine(capture, settings.clipLength));
        new CameraMetadataWriter($"{settings.metaOutputFolder}/{fileName}.xml", capture.gameObject).Write();
    }

    private IEnumerator RecordingStopRoutine(CaptureBase capture, float delay)
    {
        yield return new WaitForSeconds(delay);
        capture.StopCapture();
    }

    private IEnumerator RigMetadataCoroutine(RecordingSettings settings)
    {
        var rigParent = focusObjectTransform.GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.gameObject.name == "root");
        if (rigParent == null) yield break;

        Directory.CreateDirectory($"{settings.metaOutputFolder}/rig");
        
        int currentFrame = 0;
        while (currentFrame < settings.clipLength * settings.frameRate)
        {
            new RigMetadataWriter($"{settings.metaOutputFolder}/rig/frame_{currentFrame + 1}.xml", rigParent).Write();
            currentFrame++;
            yield return null;
        }
    }
}