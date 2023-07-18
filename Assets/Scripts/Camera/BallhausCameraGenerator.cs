using System.Collections.Generic;
using UnityEngine;

public class BallhausCameraGenerator : CameraGenerator
{
    private const int LatitudeIterations = 5;
    
    public BallhausCameraGenerator(Camera cameraPrefab, Transform cameraParent, Transform focusObjectTransform, List<Camera> cameras) 
        : base(cameraPrefab, cameraParent, focusObjectTransform, cameras) { }

    public override List<GameObject> GenerateCameras(int amount, float radius, Vector3 center)
    {
        var cameras = new List<GameObject>();
        var camerasPerLatitude = amount / LatitudeIterations;
        
        for (var latitude = 0; latitude < LatitudeIterations; latitude ++)
        {
            var lat = Mathf.Lerp(0f, 0.9f * Mathf.PI / 2f, latitude / (float) LatitudeIterations);
            
            for (var i = 0; i < camerasPerLatitude; i++)
            {
                var longitude = Mathf.Lerp(-Mathf.PI * 2f, Mathf.PI * 2f, i / (float) amount);

                var equatorX = Mathf.Cos(longitude);
                var equatorZ = Mathf.Sin(longitude);
                
                var multiplier = Mathf.Cos(lat);
                var x = multiplier * equatorX * radius;
                var z = multiplier * equatorZ * radius;
                var y = Mathf.Sin(lat) * radius;
            
                cameras.Add(GenerateCamera(new Vector3(x, y, z)));
            }
        }

        return cameras;
    }
}