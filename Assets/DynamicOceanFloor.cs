using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConsistentSeabedMeshGenerator : MonoBehaviour
{
    public ARPlaneManager planeManager; // Reference to the AR Plane Manager
    public Material oceanFloorMaterial; // Material for the ocean floor mesh
    public float meshHeightVariation = 0.1f; // Bumpiness variation for the ocean floor

    private Mesh oceanMesh;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private Dictionary<ARPlane, List<Vector3>> processedPlanes = new Dictionary<ARPlane, List<Vector3>>(); // Track already processed planes
    private float lowestElevation = Mathf.Infinity;

    private void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
        InitMesh();
    }

    private void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void InitMesh()
    {
        // Initialize the mesh and assign it to the MeshFilter
        oceanMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = oceanMesh;
        GetComponent<MeshRenderer>().material = oceanFloorMaterial;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Process newly added and updated planes
        foreach (ARPlane plane in args.added)
        {
            ExtendMeshWithPlane(plane);
        }

        foreach (ARPlane plane in args.updated)
        {
            ExtendMeshWithPlane(plane);
        }

        // Update the mesh with new vertices and triangles
        UpdateMesh();
    }

    private void ExtendMeshWithPlane(ARPlane plane)
    {
        // Get the boundary points in 2D (XZ plane)
        var boundaryPoints = plane.boundary;

        // Skip this plane if we have already processed it
        if (processedPlanes.ContainsKey(plane))
        {
            return; // Plane has been processed already, no need to add it again
        }

        if (boundaryPoints.Length > 0)
        {
            // Track the lowest elevation and prevent going below it
            if (plane.transform.position.y < lowestElevation)
            {
                lowestElevation = plane.transform.position.y;
            }

            // Start from the current number of vertices
            int vertexStartIndex = vertices.Count;

            // Create a list to store the new boundary vertices
            List<Vector3> planeVertices = new List<Vector3>();

            // Iterate over boundary points and create vertices
            for (int i = 0; i < boundaryPoints.Length; i++)
            {
                // Convert the 2D boundary to 3D
                Vector3 boundaryPoint = new Vector3(boundaryPoints[i].x, plane.transform.position.y, boundaryPoints[i].y);

                // Add a small height variation but clamp to avoid going below the lowest plane
                float yPos = Mathf.Max(plane.transform.position.y + Random.Range(0f, meshHeightVariation), lowestElevation);
                Vector3 vertex = new Vector3(boundaryPoint.x, yPos, boundaryPoint.z);

                // Add the new vertex to both the mesh and the list tracking this plane's boundary
                vertices.Add(vertex);
                planeVertices.Add(vertex);
            }

            // Mark this plane as processed
            processedPlanes[plane] = planeVertices;

            // Create triangles for this section using simple fan triangulation
            for (int i = 1; i < boundaryPoints.Length - 1; i++)
            {
                triangles.Add(vertexStartIndex);         // Center vertex
                triangles.Add(vertexStartIndex + i);     // Current vertex
                triangles.Add(vertexStartIndex + i + 1); // Next vertex
            }
        }
    }

    private void UpdateMesh()
    {
        // Update the mesh with the current vertices and triangles
        oceanMesh.Clear();
        oceanMesh.vertices = vertices.ToArray();
        oceanMesh.triangles = triangles.ToArray();
        oceanMesh.RecalculateNormals(); // Ensure smooth lighting for the mesh
    }
}
