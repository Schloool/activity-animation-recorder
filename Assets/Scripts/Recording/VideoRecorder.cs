using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RenderHeads.Media.AVProMovieCapture;
using UnityEngine;

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
        
        var animationChoice = FindObjectOfType<AnimationChoice>();
        Debug.Assert(animationChoice != null, "No animation choice found");
        
        foreach (var animation in settings.animationList.items)
        {
            var startTime = Time.realtimeSinceStartup;
            Debug.Log($"Recording animation {animation.name}");
            Directory.CreateDirectory($"{settings.recordingOutputFolder}/{animation.name}/{settings.metaOutputFolder}");
            
            for (var i = 0; i < animation.clips.Count; i++)
            {
                yield return null;
                yield return RecordAnimation(characterChoice, settings, 
                    () => animationChoice.ChooseAnimationByItem(animation.clips[i]),
                    animation.name, i, animation.clips[i]);
            }

            var neededTime = Time.realtimeSinceStartup - startTime;
            var clipAmount = animation.clips.Count * settings.cameraAmount * settings.radiusList.Count 
                             * settings.characterList.items.Count;
            Debug.Log($"Time: {neededTime:0.00}s for {clipAmount} clips");
        }

        yield return null;
        
        OnFinishRecording?.Invoke();
    }

    private IEnumerator RecordAnimation(CharacterChoice characterChoice, RecordingSettings settings, 
        Action SetCharacterAnimation, string animationName, int animationIndex, AnimationChoice.AnimationClipItem clip)
    {
        for (var i = 0; i < settings.characterList.items.Count; i++)
        {
            characterChoice.ChooseCharacter(settings.characterList.items[i]);
            yield return null;
            SetCharacterAnimation();
            yield return null;
            yield return RecordCharacter(i, settings, animationName, animationIndex, clip, 
                SetCharacterAnimation);

            if (i == 0 && settings.saveRigData)
            {
                yield return RigMetadataCoroutine(settings, animationName, animationIndex);
            }
        }
    }

    private IEnumerator RecordCharacter(int characterIndex, RecordingSettings settings, string animationName,
        int animationIndex, AnimationChoice.AnimationClipItem clip, Action SetCharacterAnimation)
    {
        foreach (var radius in settings.radiusList)
        {
            yield return RecordRadius(radius, settings, characterIndex, animationName, animationIndex, clip, 
                SetCharacterAnimation);
        }
    }

    private IEnumerator RecordRadius(float radius, RecordingSettings settings, int characterIndex, string animationName,
        int animationIndex, AnimationChoice.AnimationClipItem clip, Action SetCharacterAnimation)
    {
        _recordingCameras = new List<Camera>();
        _cameraGenerator = new BallhausCameraGenerator(cameraPrefab, cameraParent, focusObjectTransform, _recordingCameras);
        
        var cameras = _cameraGenerator.GenerateCameras(settings.cameraAmount, radius, focusObjectTransform.position);

        if (!Directory.Exists(settings.recordingOutputFolder)) Directory.CreateDirectory(settings.recordingOutputFolder);

        if (settings.maxBatchSize <= 0 || !clip.loopable)
        {
            settings.maxBatchSize = 1;
        }
        
        var captureUnits = _recordingCameras
            .Select(cam => cam.gameObject.AddComponent<CaptureFromCamera>())
            .ToList();
            
        yield return null;
            
        int cameraIndex = 0;
        while (captureUnits.Any())
        {
            if (!clip.loopable)
            {
                var characterTransform = focusObjectTransform.GetChild(0).transform;
                characterTransform.localPosition = Vector3.zero;
                characterTransform.rotation = Quaternion.identity;
                
                SetCharacterAnimation();
            }

            var currentBatch = captureUnits.Take(settings.maxBatchSize).ToList();
            var clipLength = clip.loopable ? settings.clipLength : clip.clip.length;
            var character = settings.characterList.items[characterIndex];
            
            foreach (var capture in currentBatch)
            {
                var folder = $"{settings.recordingOutputFolder}/{animationName}";
                var fileName = $"{animationName}_{animationIndex}-c{characterIndex}-r{radius}-{cameraIndex++}";
                
                CaptureCamera(settings, capture, folder, fileName, clipLength, character);
            }
            
            yield return new WaitForSeconds(clipLength);     
            captureUnits = captureUnits.Skip(settings.maxBatchSize).ToList();
        }

        yield return null;
        
        captureUnits.ForEach(Destroy);
        cameras.ForEach(Destroy);
        yield return null;
    }

    private void CaptureCamera(RecordingSettings settings, CaptureFromCamera capture, string folder, string fileName,
        float clipLength, CharacterChoice.CharacterItem character)
    {
        capture.IsRealTime = false;
        capture.FrameRate = settings.frameRate;
        capture.SelectRecordingResolution(640, 360);
        capture.TimelapseScale = 4000;
        capture.QueueStartCapture();
        capture.LogCaptureStartStop = false;
        capture.AppendFilenameTimestamp = false;
        capture.OutputFolderPath = folder;
        capture.FilenamePrefix = fileName;

        capture.StartCapture();

        StartCoroutine(RecordingStopRoutine(capture, clipLength));

        new CameraMetadataWriter($"{folder}/{settings.metaOutputFolder}/{fileName}.xml", capture.gameObject, character).Write();
    }

    private IEnumerator RecordingStopRoutine(CaptureBase capture, float delay)
    {
        yield return new WaitForSeconds(delay);
        capture.StopCapture();
    }

    private IEnumerator RigMetadataCoroutine(RecordingSettings settings, string animationName, int animationIndex)
    {
        var rigParent = focusObjectTransform.GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.gameObject.name == "root");
        if (rigParent == null) yield break;

        var rigFolder = $"{settings.recordingOutputFolder}/{animationName}/{settings.metaOutputFolder}/rig_{animationIndex}";
        Directory.CreateDirectory(rigFolder);
        
        int currentFrame = 0;
        while (currentFrame < settings.clipLength * settings.frameRate)
        {
            new RigMetadataWriter($"{rigFolder}/frame_{currentFrame + 1}.xml", rigParent).Write();
            currentFrame++;
            yield return null;
        }
    }
}