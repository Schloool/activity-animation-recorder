using UnityEditor;
using Object = UnityEngine.Object;

public class ImportUtil
{
    [MenuItem("Import/Import All")]
    public static void ImportAll()
    {
        ImportCharacterFiles();
        ImportAnimations();
    }
    
    [MenuItem("Import/Import Animations")]
    public static void ImportAnimations()
    {
        var importer = new AnimationFileImporter(asset =>
        {
            Object.FindObjectOfType<AnimationChoice>().AnimationList = asset;
            Object.FindObjectOfType<AnimationItemDropdown>().ItemList = asset;
        });
        importer.Import();
    }
    
    [MenuItem("Import/Import FBX Files")]
    public static void ImportCharacterFiles()
    {
        var importer = new CharacterAssetImporter(asset =>
        {
            Object.FindObjectOfType<CharacterChoice>().characterList = asset;
            Object.FindObjectOfType<CharacterItemDropdown>().ItemList = asset;
        });
        importer.Import();
    }
}