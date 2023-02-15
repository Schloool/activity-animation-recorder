using System.Collections.Generic;
using UnityEngine;

public abstract class CameraGenerator
{
    private readonly Camera _cameraPrefab;
    private readonly Transform _cameraParent;
    private readonly Transform _focusObjectTransform;
    private readonly List<Camera> _cameras;

    protected CameraGenerator(Camera cameraPrefab, Transform cameraParent, Transform focusObjectTransform, List<Camera> cameras)
    {
        _cameraPrefab = cameraPrefab;
        _cameraParent = cameraParent;
        _focusObjectTransform = focusObjectTransform;
        _cameras = cameras;
    }

    public abstract void GenerateCameras(int amount, float radius, Vector3 center);

    protected void GenerateCamera(Vector3 position)
    {
        var camera = Object.Instantiate(_cameraPrefab, position, Quaternion.identity, _cameraParent);
        camera.transform.LookAt(_focusObjectTransform);
        _cameras.Add(camera);
        camera.enabled = false;
    }
}