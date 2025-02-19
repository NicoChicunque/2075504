using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CCDAddressablesManager : MonoBehaviour
{
    [SerializeField] Button _downloadAddressablesButton, _cleanCacheButton;
    [SerializeField] string _remoteCatalogPath = "https://8f75c7dd-ca77-4182-89c8-2c4ee0db415e.client-api.unity3dusercontent.com/client_api/v1/environments/testing/buckets/e940f260-680b-42b8-81ae-f5108073674a/release_by_badge/latest/entry_by_path/content/?path=catalog_1.0.json";
    [SerializeField] List<string> addressableKeys;
    [SerializeField] Image _templateImage;

#if UNITY_EDITOR
    [SerializeField] List<Object> assets;

    [ContextMenu("SetKeys")]
    void SetKeys()
    {
        addressableKeys.Clear();

        foreach (Object asset in assets)
        {
            string key = AssetDatabase.GetAssetPath(asset);
            addressableKeys.Add(key);
        }
    }
#endif

    void Start()
    {
        _downloadAddressablesButton.onClick.AddListener(DownloadAddressables);
        _cleanCacheButton.onClick.AddListener(CleanBundleCache);
    }

    void DownloadAddressables()
    {
        Addressables.InitializeAsync().Completed += OnAddressablesInitialized;
    }

    void OnAddressablesInitialized(AsyncOperationHandle<IResourceLocator> initHandle)
    {
        Debug.Log("OnAddressablesInitialized!");
        Addressables.LoadContentCatalogAsync(_remoteCatalogPath).Completed += OnCatalogLoaded;
        Addressables.Release(initHandle);
    }

    void OnCatalogLoaded(AsyncOperationHandle<IResourceLocator> catalogHandle)
    {
        Debug.Log("OnCatalogLoaded!");

        foreach (string key in addressableKeys)
        {
            Addressables.LoadAssetAsync<Texture2D>(key).Completed += OnTextureLoaded;
        }

        Addressables.Release(catalogHandle);
    }

    void OnTextureLoaded(AsyncOperationHandle<Texture2D> textureHandle)
    {
        if (textureHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Texture2D texture = textureHandle.Result;
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 10f);
            Image newImage = Instantiate(_templateImage, _templateImage.transform.parent);
            newImage.sprite = newSprite;
            newImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"Error al cargar textura:");
        }
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