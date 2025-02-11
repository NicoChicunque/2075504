using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;

public static class PackageAutoDownloader
{
    static string packageName = "com.unity.ide.visualstudio"; // Package name
    static AddRequest addRequest;

    [InitializeOnLoadMethod]
    static void Downloader()
    {        
        ListRequest listRequest = Client.List(); // Check if the package is already installed
        while (! listRequest.IsCompleted);

        if (listRequest.Status == StatusCode.Success)
        {
            foreach (UnityEditor.PackageManager.PackageInfo package in listRequest.Result)
            {
                if (package.name == packageName) { return; }
            }
            
            addRequest = Client.Add(packageName); // If the package is not installed, download it
            EditorApplication.update += OnPackageDownloaded;
            return;
        }

        Debug.LogError("Error listing packages: " + listRequest.Error.message);
    }
    
    static void OnPackageDownloaded() // Optional callback (called when the download finishes)
    {
        if (! addRequest.IsCompleted) { return; }

        EditorApplication.update -= OnPackageDownloaded;

        if (addRequest.Status == StatusCode.Success)
        {
            Debug.Log(packageName + " downloaded and installed.");
            return;
        }

        Debug.LogError("Error downloading " + packageName + ": " + addRequest.Error.message);
    }
}