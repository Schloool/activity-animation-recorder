using System.Collections.Generic;
using UnityEngine;

public class BallhausCameraGenerator : CameraGenerator
{
    private const int LatitudeIterations = 10;
    
    public BallhausCameraGenerator(Camera cameraPrefab, Transform cameraParent, Transform focusObjectTransform, List<Camera> cameras) 
        : base(cameraPrefab, cameraParent, focusObjectTransform, cameras) { }

    public override void GenerateCameras(int amount, float radius, Vector3 center)
    {
        for (var latitude = 0; latitude < LatitudeIterations; latitude ++)
        {
            var lat = Mathf.Lerp(0f, 0.9f * Mathf.PI / 2f, latitude / (float) LatitudeIterations);
            
            for (var i = 0; i < amount; i++)
            {
                var longitude = Mathf.Lerp(-Mathf.PI * 2f, Mathf.PI * 2f, i / (float) amount);

                var equatorX = Mathf.Cos(longitude);
                var equatorZ = Mathf.Sin(longitude);
                
                var multiplier = Mathf.Cos(lat);
                var x = multiplier * equatorX * radius;
                var z = multiplier * equatorZ * radius;
                var y = Mathf.Sin(lat) * radius;
            
                GenerateCamera(new Vector3(x, y, z));
            }
        }
    }
}