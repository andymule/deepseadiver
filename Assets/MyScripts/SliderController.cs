using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    private Slider slider;
    public ShipController shipController;
    public Text waypointText; // Optional: Display current waypoint

    void Start()
    {
        slider = GetComponent<Slider>();
        shipController = GameObject.FindObjectOfType<ShipController>();
        // TODO dyanmic slider max value 
        
        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // Initialize the slider's value to the ship's current position
        slider.value = 0;//shipController.currentWaypointIndex;
        UpdateWaypointText(slider.value);
    }

    void OnSliderValueChanged(float value)
    {
        int waypointIndex = Mathf.RoundToInt(value);
        shipController.MoveToWaypoint(waypointIndex);
        UpdateWaypointText(value);
    }

    void UpdateWaypointText(float value)
    {
        if (waypointText != null)
        {
            int waypointIndex = Mathf.RoundToInt(value);
            waypointText.text = "Waypoint: " + waypointIndex;
        }
    }
}