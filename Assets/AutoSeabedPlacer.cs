using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class LowestPlaneSeabedPlacer : MonoBehaviour
{
    public GameObject seabedPrefab; // Assign your seabed prefab in the Inspector
    public ARPlaneManager planeManager;

    private GameObject placedSeabed;
    private ARPlane lowestPlane;

    void OnEnable()
    {
        // Subscribe to the event that fires when a plane is added or updated
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        // Unsubscribe from the event to prevent memory leaks
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Check for any added or updated planes
        foreach (ARPlane plane in args.added)
        {
            UpdateLowestPlane(plane);
        }

        foreach (ARPlane plane in args.updated)
        {
            UpdateLowestPlane(plane);
        }

        // Place the seabed on the lowest detected plane if it's not already placed
        if (lowestPlane != null && placedSeabed == null)
        {
            PlaceSeabed(lowestPlane.transform.position);
        }
    }

    private void UpdateLowestPlane(ARPlane plane)
    {
        // Ensure we only consider horizontal planes (likely representing the floor)
        if (plane.alignment == PlaneAlignment.HorizontalUp)
        {
            // Check if the current plane is lower than the lowest plane or if no lowest plane has been set
            if (lowestPlane == null || plane.transform.position.y < lowestPlane.transform.position.y)
            {
                // Update to the new lowest plane
                lowestPlane = plane;

                // If the seabed is already placed, move it to the new lowest plane
                if (placedSeabed != null)
                {
                    MoveSeabedToLowestPlane();
                }
            }
        }
    }

    private void PlaceSeabed(Vector3 position)
    {
        // Instantiate the seabed at the position of the lowest detected plane
        placedSeabed = Instantiate(seabedPrefab, position, Quaternion.identity);
    
        // Find the UpdateShaderPosition object (assuming it exists in your scene)
        UpdateShaderPosition updateShader = FindObjectOfType<UpdateShaderPosition>();
    
        if (updateShader != null && placedSeabed != null)
        {
            // Get the Renderer from the newly instantiated seabed and assign it to the UpdateShaderPosition script
            Renderer seabedRenderer = placedSeabed.GetComponent<Renderer>();
            updateShader.oceanFloorRenderer = seabedRenderer;
        
            Debug.Log("Seabed placed and shader renderer updated.");
        }
        else
        {
            Debug.LogError("UpdateShaderPosition or seabedRenderer is missing!");
        }
    }


    private void MoveSeabedToLowestPlane()
    {
        if (placedSeabed != null && lowestPlane != null)
        {
            // Move the seabed to the position of the lowest plane
            placedSeabed.transform.position = lowestPlane.transform.position;
            Debug.Log("Seabed moved to the new lowest detected plane.");
        }
    }
}
