using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class AssetFileImporter<T, TL, AT> 
    where T : ChoiceObject 
    where TL : SimulationItemList<T>
    where AT : Object
{
    protected readonly string ResourcesSubfolder;
    private readonly string _assetFileName;
    
    private readonly Action<TL> _callback;

    protected AssetFileImporter(string resourcesSubfolder, string assetFileName, Action<TL> callback)
    {
        ResourcesSubfolder = resourcesSubfolder;
        _assetFileName = assetFileName;
        _callback = callback;
    }

    protected abstract void HandleFile(ModelImporter importer);
    protected abstract T GenerateItemFromPrefab(AT resource);
    
    public void Import()
    {
        var items = new List<T>();
        GetAssetFiles().ForEach(file =>
        {
            var importer = AssetImporter.GetAtPath($"Assets/Resources/{ResourcesSubfolder}/{file}") as ModelImporter;
            Debug.Assert(importer != null, $"The animation file {file} could not be imported");
            
            items.Add(LoadItemForAssetAtPath($"Assets/Resources/{ResourcesSubfolder}/{file}"));
            HandleFile(importer);
        });
        
        CreateListAsset(items);
    }

    private List<string> GetAssetFiles()
    {
        return Directory.GetFiles($"{Application.dataPath}/Resources/{ResourcesSubfolder}")
            .Where(FilterFileName)
            .Select(path => path.Split("\\").Last())
            .ToList();
    }

    protected virtual bool FilterFileName(string fileName)
    {
        return true;
    }
    
    private T LoadItemForAssetAtPath(string path)
    {
        var asset = AssetDatabase.LoadAssetAtPath<AT>(path);
        return GenerateItemFromPrefab(asset);
    }
    
    private void CreateListAsset(List<T> items) 
    {
        var asset = ScriptableObject.CreateInstance<TL>();
        asset.items = items;

        AssetDatabase.CreateAsset(asset, $"Assets/Scriptable Objects/{_assetFileName}.asset");
        AssetDatabase.SaveAssets();
        
        _callback?.Invoke(asset);

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}