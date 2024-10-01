using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.ARFoundation;

public class RoomPreScan : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject startButton;
    public GameObject objectPlacementUI; // The UI for placing objects

    private bool isScanning = true;

    void Start()
    {
        objectPlacementUI.SetActive(false); // Disable object placement UI at the start
        startButton.SetActive(false); // Start button appears after scanning phase

        // Start detecting planes
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Show the start button when planes are detected
        if (planeManager.trackables.count > 0 && isScanning)
        {
            startButton.SetActive(true);
        }
    }

    public void StartPlacementMode()
    {
        // End scanning phase, start object placement mode
        isScanning = false;
        objectPlacementUI.SetActive(true); // Enable object placement UI
        startButton.SetActive(false); // Hide start button
    }
}
