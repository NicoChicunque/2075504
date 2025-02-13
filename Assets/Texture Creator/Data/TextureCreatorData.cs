using UnityEngine;

[CreateAssetMenu]
public class TextureCreatorData : ScriptableObject
{
    [SerializeField]
    private int m_Width = 256;
    
    [SerializeField]
    private int m_Height = 256;

    [SerializeField]
    private int m_Textures = 100;
    
    [SerializeField]
    private int m_Bundles = 10;
    
    [SerializeField] 
    private string m_AssetBundleDefaultName = "test";
    
    [SerializeField] 
    private string m_DefaultFolder = "Assets/Textures";

    public int Width => m_Width;
    
    public int Height => m_Height;

    public int Textures => m_Textures;

    public int Bundles => m_Bundles;
    
    public string AssetBundleDefaultName => m_AssetBundleDefaultName;
    
    public string DefaultFolder => m_DefaultFolder;
}
