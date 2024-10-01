using UnityEngine;
using GLTFast;
using System;

public class GLBLoader : MonoBehaviour
{
    string streamingAsset = "rover.glb";
    public Transform parentTransform; // Optional: parent object to place the loaded model under

    async void Start()
    {
        // Create a new GltfImport instance
        var gltf = new GltfImport();

        // Form the path to the GLB file using Uri to handle various platforms
        string path = Application.streamingAssetsPath + "/" + streamingAsset;
        string fileUri = new Uri(path).AbsoluteUri;  // Converts path to URI format

        // Load the GLB model from the URI
        bool success = await gltf.Load(fileUri);

        if (success)
        {
            // Instantiate the loaded model asynchronously
            success = await gltf.InstantiateMainSceneAsync(parentTransform);
            if (success)
            {
                Debug.Log("glTF model instantiated successfully!");
            }
            else
            {
                Debug.LogError("Failed to instantiate GLB model.");
            }
        }
        else
        {
            Debug.LogError("Failed to load GLB model from path: " + fileUri);
        }
    }
}