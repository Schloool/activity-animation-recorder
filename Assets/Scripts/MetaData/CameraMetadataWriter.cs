using System.IO;
using System.Xml;
using UnityEngine;

public class CameraMetadataWriter : MetadataWriter
{
    private GameObject _recordingObject;

    public CameraMetadataWriter(string filePath, GameObject recordingObject) : base(filePath)
    {
        _recordingObject = recordingObject;
    }

    protected override void WriteData(XmlWriter writer)
    {
        var camPosition = _recordingObject.transform.position;
        var camRotation = _recordingObject.transform.eulerAngles;
        
        WriteSimpleElement("cam_distance", Vector3.Distance(Vector3.zero, camPosition).ToString(), writer);
        
        writer.WriteStartElement("position");
        WriteSimpleElement("x", camPosition.x.ToString(), writer);
        WriteSimpleElement("y", camPosition.y.ToString(), writer);
        WriteSimpleElement("z", camPosition.z.ToString(), writer);
        writer.WriteEndElement();
        
        writer.WriteStartElement("rotation");
        WriteSimpleElement("x", camRotation.x.ToString(), writer);
        WriteSimpleElement("y", camRotation.y.ToString(), writer);
        WriteSimpleElement("z", camRotation.z.ToString(), writer);
        writer.WriteEndElement();
    }
}