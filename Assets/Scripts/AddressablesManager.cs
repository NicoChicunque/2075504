using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CCDAddressablesManager : MonoBehaviour
{
    [SerializeField] Button downloadAddressablesButton, cleanCacheButton;
    [SerializeField] string catalogPath = "CCDBuildData/9a7816b4-166e-4c80-9f6c-3ef6aecebe92/e940f260-680b-42b8-81ae-f5108073674a/latest/catalog_1.0.json";
    [SerializeField] string[] addressableKeys;
    [SerializeField] Image _image;

    void Start()
    {
        downloadAddressablesButton.onClick.AddListener(DownloadAddressables);
        cleanCacheButton.onClick.AddListener(CleanBundleCache);        
    }

    void DownloadAddressables()=>Addressables.InitializeAsync().Completed += OnAddressablesInitialized;

    void OnAddressablesInitialized(AsyncOperationHandle<IResourceLocator> initHandle)
    {
        Debug.Log("OnAddressablesInitialized!");
        Addressables.LoadContentCatalogAsync(catalogPath).Completed += OnCatalogLoaded;
        Addressables.Release(initHandle);
    }

    void OnCatalogLoaded(AsyncOperationHandle<IResourceLocator> catalogHandle)
    {
        Debug.Log("OnCatalogLoaded!");
        Addressables.LoadAssetAsync<Texture2D>(addressableKeys[0]).Completed += OnTextureLoaded;
        Addressables.Release(catalogHandle);
    }

    void OnTextureLoaded(AsyncOperationHandle<Texture2D> textureHandle)
    {
        Debug.Log("OnTextureLoaded!");
        Texture2D texture = textureHandle.Result;
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),  Vector2.zero,  100f );
        _image.sprite = newSprite;
        //Addressables.Release(textureHandle);
    }

    void CleanBundleCache() => Addressables.CleanBundleCache().Completed += OnCleanBundleCache;

    void OnCleanBundleCache(AsyncOperationHandle<bool> cleanBundleCachehandle)
    {
        if (cleanBundleCachehandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Successfully cleaned Addressables cache.");
        }
        else
        {
            Debug.LogError("Failed to clean Addressables cache.");
        }

        Addressables.Release(cleanBundleCachehandle);
    }
}