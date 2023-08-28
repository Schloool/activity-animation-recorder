using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class RigMetadataWriter : MetadataWriter
{
    private readonly Transform _rigParent;
    
    public RigMetadataWriter(string filePath, Transform rigParent) : base(filePath)
    {
        _rigParent = rigParent;
    }
    
    protected override void WriteData(XmlWriter writer)
    {
        WriteBoneRecursive(_rigParent, writer);
    }

    private void WriteBoneRecursive(Transform boneParent, XmlWriter writer)
    {
        var position = boneParent.position;
        var rotation = boneParent.eulerAngles;
        
        writer.WriteStartElement(Regex.Replace(boneParent.gameObject.name, @"[\s()]+", ""));

        writer.WriteStartElement("position");
        WriteSimpleElement("x", position.x.ToString(), writer);
        WriteSimpleElement("y", position.y.ToString(), writer);
        WriteSimpleElement("z", position.z.ToString(), writer);
        writer.WriteEndElement();

        writer.WriteStartElement("rotation");
        WriteSimpleElement("x", rotation.x.ToString(), writer);
        WriteSimpleElement("y", rotation.y.ToString(), writer);
        WriteSimpleElement("z", rotation.z.ToString(), writer);
        writer.WriteEndElement();

        if (boneParent.childCount <= 0)
        {
            writer.WriteEndElement();
            return;
        }

        foreach (Transform boneChild in boneParent)
        {
            WriteBoneRecursive(boneChild, writer);
        }
        
        writer.WriteEndElement();
    }
}