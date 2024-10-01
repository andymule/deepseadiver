using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShaderPosition : MonoBehaviour
{
    public Renderer oceanFloorRenderer; // Assign your ocean floor renderer
    public Transform shipTransform;     // Assign the DummyShip transform
    public float radius = 5.0f;         // Public radius value you can adjust in the Inspector or modify dynamically
    public float rolloff = 2.0f;        // Public rolloff (falloff) value for controlling the softness of the light falloff

    void Update()
    {
        if (oceanFloorRenderer != null && shipTransform != null)
        {
            // Update the shader's _ShipPosition
            oceanFloorRenderer.material.SetVector("_ShipPosition", shipTransform.position);
            
            // Update the shader's _Radius
            oceanFloorRenderer.material.SetFloat("_Radius", radius);
            
            // Update the shader's _FalloffFactor (rolloff control)
            oceanFloorRenderer.material.SetFloat("_FalloffFactor", rolloff);

            // Debug.Log($"Updating Shader: Position = {shipTransform.position}, Radius = {radius}, Rolloff = {rolloff}");
        }
        else
        {
            Debug.LogError("Renderer or shipTransform not assigned!");
        }
    }
}