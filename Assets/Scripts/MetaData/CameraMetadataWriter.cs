using System.IO;
using System.Xml;
using UnityEngine;

public class CameraMetadataWriter : MetadataWriter
{
    private GameObject _recordingObject;
    private CharacterChoice.CharacterItem _character;

    public CameraMetadataWriter(string filePath, GameObject recordingObject, CharacterChoice.CharacterItem character) : base(filePath)
    {
        _recordingObject = recordingObject;
        _character = character;
    }

    protected override void WriteData(XmlWriter writer)
    {
        var camPosition = _recordingObject.transform.position;
        var camRotation = _recordingObject.transform.eulerAngles;
        
        writer.WriteStartElement("character");
        WriteSimpleElement("id", _character.name, writer);
        if (_character.details.Length > 0) WriteSimpleElement("details", _character.details, writer);
        writer.WriteEndElement();
        
        writer.WriteStartElement("camera");
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
        writer.WriteEndElement();
    }
}