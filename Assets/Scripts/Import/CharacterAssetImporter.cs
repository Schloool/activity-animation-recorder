#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class CharacterAssetImporter : AssetFileImporter<CharacterChoice.CharacterItem, CharacterList, GameObject>
{
    public CharacterAssetImporter(Action<CharacterList> callback, string resourcesSubfolder = "Characters", 
        string assetFileName = "Character List") : base(resourcesSubfolder, assetFileName, callback)
    { }

    protected override bool FilterFileName(string fileName)
    {
        return fileName.EndsWith("fbx");
    }

    protected override void HandleFile(ModelImporter importer)
    { 
        importer.importNormals = ModelImporterNormals.Calculate;
        importer.animationType = ModelImporterAnimationType.Human;
        importer.SaveAndReimport(); 
    }

    protected override CharacterChoice.CharacterItem GenerateItemFromPrefab(GameObject prefab)
    {
        return new CharacterChoice.CharacterItem
        {
            name = prefab.name,
            prefab = prefab
        };
    }
}
#endif