using System.Collections.Generic;
using UnityEngine;

public class RandomCameraGenerator : CameraGenerator
{
    public RandomCameraGenerator(Camera cameraPrefab, Transform cameraParent, Transform focusObjectTransform, List<Camera> cameras) 
        : base(cameraPrefab, cameraParent, focusObjectTransform, cameras) { }

    public override List<GameObject> GenerateCameras(int amount, float radius, Vector3 center)
    {
        var cameras = new List<GameObject>();
        
        for (var i = 0; i < amount; i++)
        {
            var x = Random.Range(-radius, radius);
            var y = Random.Range(0.5f, radius);
            var z = Random.Range(-radius, radius);
            var position = new Vector3(x, y, z);
            var relativePos = center - position;

            cameras.Add(GenerateCamera(relativePos));
        }

        return cameras;
    }
}