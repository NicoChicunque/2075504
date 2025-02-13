using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using Random = UnityEngine.Random;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Unity.EditorCoroutines.Editor;

public class TextureCreator : MonoBehaviour
{
    private const string k_DefaultDataPath = "Assets/Texture Creator/Data/Texture Creator Settings.asset";

    [MenuItem("Unity Support/Create 10 Groups, each 10 textures", false, 4)]
    public static void CreateSeveralGroups()
    {
        Debug.Log("Creating 10 groups with 10 textures each");
        EditorCoroutineUtility.StartCoroutineOwnerless(CreateGroups());
    }

    static IEnumerator CreateGroups()
    {
        for (int i = 0; i < 10; i++)
        {
            CreateTextureAssets();
            yield return new WaitForSeconds(0.4f);
        }
    }

    [MenuItem("Unity Support/Create Textures", false, 1)]
    public static void CreateTextureAssets()
    {
        TextureCreatorData data = AssetDatabase.LoadAssetAtPath<TextureCreatorData>(k_DefaultDataPath);

        string randomFolder = "" + Hash128.Compute(DateTime.Now.Millisecond);
        Directory.CreateDirectory(Path.Combine($"{data.DefaultFolder}", randomFolder));

        for (int i = 1; i <= data.Textures; i++)
        {
            Texture2D textureObject = CreateTextureObject(data);

            byte[] bytes = textureObject.EncodeToPNG();

            string path = Path.Combine($"{data.DefaultFolder}", randomFolder, $"ProceduralTexture_{randomFolder}_{i}.png");

            // Save the encoded PNG to the Assets folder
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();

            // Import the texture to the AssetDatabase
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.textureType = TextureImporterType.Default;
            //importer.assetBundleName = $"{data.AssetBundleDefaultName}_{i % (data.Bundles)}";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            // Optionally set texture format/compression addressableAssetSettings
            Texture2D importedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Debug.Log("Texture created at: " + path);
        }

        //Crear addressables group.
        CreateAddressableAssetGroup(data.DefaultFolder, randomFolder);
    }

    //[MenuItem("Unity Support/Create Addressable Asset Group", false, 3)]
    public static void CreateAddressableAssetGroup(string defaultFolder, string randomFolder)
    {
        AddressableAssetSettings addressableAssetSettings = AddressableAssetSettingsDefaultObject.GetSettings(true);// Obtener la configuración de los assets direccionables.
        List<ScriptableObject> groupTemplates = addressableAssetSettings.GroupTemplateObjects;
        AddressableAssetGroupTemplate template = groupTemplates.FirstOrDefault(t => t.name == "Packed Assets") as AddressableAssetGroupTemplate;// 

        string groupName = "SupportEngineQA_" + randomFolder;// Nombre del grupo de assets.

        AddressableAssetGroup group = addressableAssetSettings.CreateGroup(groupName, false, false, false, template.SchemaObjects);// Crea un nuevo grupo.

        string folderPath = Path.Combine($"{defaultFolder}", randomFolder);

        string[] guids = AssetDatabase.FindAssets("", new[] { folderPath }); // "" means search all asset types

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 3. Add asset to the group if it's not already there
            AddressableAssetEntry entry = group.entries.FirstOrDefault(e => e.address == assetPath);
            if (entry == null)  // Check for existing entry based on path.
            {
                AddressableAssetEntry assetEntry = addressableAssetSettings.CreateOrMoveEntry(guid, group);
                assetEntry.address = assetPath;
            }
            else
            {
                Debug.LogWarning($"Asset at path {assetPath} already exists in group {groupName}. Skipping.");
            }
        }

        // Guarda los cambios.
        EditorUtility.SetDirty(addressableAssetSettings);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Unity Support/Create Texture Data Settings", false, 2)]
    public static void CreateMyScriptableObject()
    {
        TextureCreatorData instance = ScriptableObject.CreateInstance<TextureCreatorData>();

        instance.name = "Texture Creator Settings";

        AssetDatabase.CreateAsset(instance, k_DefaultDataPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = instance;

        Debug.Log("MyScriptableObject instance created at: " + k_DefaultDataPath);
    }

    private static Texture2D CreateTextureObject(TextureCreatorData data)
    {
        Texture2D texture = new Texture2D(data.Width, data.Height);

        Color[] pixels = GetPixels(data);

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    private static Color[] GetPixels(TextureCreatorData data)
    {
        Color[] pixels = new Color[data.Width * data.Height];
        for (int y = 0; y < data.Height; y++)
        {
            for (int x = 0; x < data.Width; x++)
            {
                float r = Random.Range(0f, 1f);
                float g = Random.Range(0f, 1f);
                float b = Random.Range(0f, 1f);
                pixels[y * data.Width + x] = new Color(r, g, b);
            }
        }

        return pixels;
    }

}
