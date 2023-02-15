using System.IO;
using System.Xml;

public abstract class MetadataWriter
{
    protected readonly string _filePath;

    protected MetadataWriter(string filePath)
    {
        _filePath = filePath;
    }

    protected abstract void WriteData(XmlWriter writer);

    public void Write()
    {
        var stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        var settings = new XmlWriterSettings { Async = true, Indent = true, ConformanceLevel = ConformanceLevel.Auto};
        using var writer = XmlWriter.Create(stream, settings);
        
        WriteData(writer);

        writer.Flush();
        writer.Close();
    }

    protected void WriteSimpleElement(string tagName, string content, XmlWriter writer)
    {
        writer.WriteStartElement(tagName);  
        writer.WriteString(content);  
        writer.WriteEndElement(); 
    }
}