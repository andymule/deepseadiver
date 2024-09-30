using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class VehicleDataPoint
{
    public string timestamp;
    public float Salinity_psu;
    public float SoundVelocity_meters_per_second;
    public float Temperature_celsius;
    public float VehicleAltitude_meters;
    public float VehicleDepth_meters;
    public float VehicleHeading_degrees;
    public float x_meters;
    public float y_meters;
    public float z_meters;
}

[Serializable]
public class VehicleDataList
{
    public List<VehicleDataPoint> data;
}

public class DataParser : MonoBehaviour
{
    public List<VehicleDataPoint> dataPoints;
    public Material markerMaterial;
    public Material lineMaterial;
    public GameObject seafloorObject;
    public float scaleFactor = 1f;
    public Vector3 relativePosition = Vector3.zero;
    private float previousScaleFactor;
    private Vector3 previousRelativePosition;
    private GameObject visualizationParent;
    private bool dataPlotted = false;

    void Start()
    {
        LoadData();
        CalculateScalingFactor();
        previousScaleFactor = scaleFactor;
        previousRelativePosition = relativePosition;
    }

    void Update()
    {
        if (seafloorObject == null)
        {
            seafloorObject = GameObject.FindWithTag("Seafloor");
            if (seafloorObject != null && !dataPlotted)
            {
                PlotData();
                dataPlotted = true;
                previousScaleFactor = scaleFactor;
                previousRelativePosition = relativePosition;
            }
        }
        else
        {
            if ((scaleFactor != previousScaleFactor || relativePosition != previousRelativePosition) && dataPlotted)
            {
                PlotData();
                previousScaleFactor = scaleFactor;
                previousRelativePosition = relativePosition;
            }
        }
    }

    void LoadData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "DataSampleOctoTimespan_converted.json");
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            VehicleDataList vehicleDataList = JsonUtility.FromJson<VehicleDataList>(dataAsJson);
            dataPoints = vehicleDataList.data;
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }

    void CalculateScalingFactor()
    {
        if (dataPoints == null || dataPoints.Count == 0)
        {
            Debug.LogError("No data to calculate scaling factor.");
            return;
        }

        List<Vector2> positions2D = new List<Vector2>();
        foreach (var point in dataPoints)
        {
            positions2D.Add(new Vector2(point.x_meters, point.z_meters));
        }

        float minX = Mathf.Infinity, maxX = -Mathf.Infinity;
        float minZ = Mathf.Infinity, maxZ = -Mathf.Infinity;

        foreach (var pos in positions2D)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minZ) minZ = pos.y;
            if (pos.y > maxZ) maxZ = pos.y;
        }

        float extentX = maxX - minX;
        float extentZ = maxZ - minZ;

        if (scaleFactor <= 0f)
        {
            float targetSize = 2f;
            float scaleX = targetSize / extentX;
            float scaleZ = targetSize / extentZ;
            scaleFactor = Mathf.Min(scaleX, scaleZ);
        }
    }

    public void PlotData()
    {
        if (dataPoints == null || dataPoints.Count == 0)
        {
            Debug.LogError("No data to plot.");
            return;
        }

        if (visualizationParent != null)
        {
            Destroy(visualizationParent);
        }

        visualizationParent = new GameObject("VisualizationParent");
        visualizationParent.transform.parent = seafloorObject.transform;
        visualizationParent.transform.localPosition = Vector3.zero;

        Vector3[] positions = new Vector3[dataPoints.Count];

        DateTime startTime = DateTime.Parse(dataPoints[0].timestamp);
        List<float> timeNumeric = new List<float>();

        for (int i = 0; i < dataPoints.Count; i++)
        {
            DateTime currentTime = DateTime.Parse(dataPoints[i].timestamp);
            float seconds = (float)(currentTime - startTime).TotalSeconds;
            timeNumeric.Add(seconds);
        }

        float minTime = timeNumeric[0];
        float maxTime = timeNumeric[timeNumeric.Count - 1];

        for (int i = 0; i < dataPoints.Count; i++)
        {
            VehicleDataPoint point = dataPoints[i];
            Vector3 position = new Vector3(point.x_meters, point.y_meters, point.z_meters);
            position *= scaleFactor;
            position += relativePosition;
            positions[i] = position;
        }

        GameObject pathLineObj = new GameObject("PathLine");
        LineRenderer pathLine = pathLineObj.AddComponent<LineRenderer>();
        pathLine.positionCount = positions.Length;
        pathLine.SetPositions(positions);
        pathLine.startWidth = 0.04f * scaleFactor;
        pathLine.endWidth = 0.04f * scaleFactor;

        if (lineMaterial != null)
        {
            pathLine.material = lineMaterial;
        }
        else
        {
            pathLine.material = new Material(Shader.Find("Sprites/Default"));
        }

        pathLine.material.color = Color.cyan;
        pathLineObj.transform.parent = visualizationParent.transform;
    }
}
