using System;
using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CCDAddressablesManager : MonoBehaviour
{
    // Lista de paths de Addressables en CCD a descargar
    [SerializeField] string[] ccdAddressablePaths;
    [SerializeField] Button downloadAddressablesButton, cleanCacheButton;

    AsyncOperationHandle<Texture2D> bundleOperation;

    void Start()
    {
        downloadAddressablesButton.onClick.AddListener(RequestBundle);
        cleanCacheButton.onClick.AddListener(CleanCache);
    }

    //void DownloadAddressables()
    //{
    //    // Inicializar Addressables y comenzar la descarga en segundo plano
    //    StartCoroutine(InitializeAndDownloadAddressables());
    //}

    void RequestBundle() { 

        bundleOperation = Addressables.LoadAssetAsync<Texture2D>(ccdAddressablePaths[0]);
        bundleOperation.Completed += (operation) =>
        {
            if (operation.Status.Equals(AsyncOperationStatus.Succeeded))
            {
                Debug.Log("Asset bundle loaded successfully.");
                return;
            }
            Debug.LogError("Sprite load failed. Using default sprite.");
        };
    }

    void OnDestroy()
    {
        if (bundleOperation.IsValid())
        {
            Addressables.Release(bundleOperation);
            Debug.Log("Successfully released atlasOperation.");
        }
    }

    //private IEnumerator InitializeAndDownloadAddressables()
    //{
    //    // Inicializar Addressables
    //    AsyncOperationHandle initHandle = Addressables.InitializeAsync();
    //    yield return initHandle;

    //    if (initHandle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        Debug.Log("Addressables initialized successfully.");

    //        // Descargar cada Addressable en segundo plano usando su path
    //        foreach (string path in ccdAddressablePaths)
    //        {
    //            StartCoroutine(DownloadAddressable(path));
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("Failed to initialize Addressables.");
    //    }

    //    // Liberar el handle de inicialización
    //    Addressables.Release(initHandle);
    //}

    //private IEnumerator DownloadAddressable(string path)
    //{
    //    // Descargar el Addressable desde CCD usando su path
    //    AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(path);
    //    yield return downloadHandle;

    //    if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        Debug.Log($"Successfully downloaded Addressable from CCD: {path}");
    //    }
    //    else
    //    {
    //        Debug.LogError($"Failed to download Addressable from CCD: {path}");
    //    }

    //    // Liberar el handle para evitar fugas de memoria
    //    Addressables.Release(downloadHandle);
    //}

    public void CleanCache()
    {
        // Limpiar la caché de Addressables
        AsyncOperationHandle cleanHandle = Addressables.CleanBundleCache();
        cleanHandle.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Successfully cleaned Addressables cache.");
            }
            else
            {
                Debug.LogError("Failed to clean Addressables cache.");
            }

            // Liberar el handle de limpieza
            Addressables.Release(cleanHandle);
        };
    }
}