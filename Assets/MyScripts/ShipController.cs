using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float speed = 0.05f;           // Movement speed
    private Vector3[] pathPositions;      // Positions along the path
    private int currentWaypointIndex = 0; // Current index along the path
    private int targetWaypointIndex = 0;  // Target waypoint index
    private bool isMoving = false;
    private bool isSliderControl = false; // Indicates if movement is controlled by the slider
    private Quaternion rotationOffset;

    // Public Vector3 to set rotation offset in Euler angles from the editor
    public Vector3 rotationOffsetEuler = Vector3.zero;

    // Declare isMovingForward to control movement direction
    private bool isMovingForward = true;

    void Start()
    {
        // Convert Euler angles to Quaternion for rotation offset
        rotationOffset = Quaternion.Euler(rotationOffsetEuler);
    }

    void Update()
    {
        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    public void SetPathPositions(Vector3[] positions)
    {
        pathPositions = positions;

        // Initialize the ship's position to the first point on the path
        if (pathPositions != null && pathPositions.Length > 0)
        {
            transform.position = pathPositions[0];
            currentWaypointIndex = 0;
            targetWaypointIndex = 0;
        }
    }

    // Function called when the path is tapped
    public void StartMoving()
    {
        if (pathPositions == null || pathPositions.Length == 0)
            return;

        isMoving = true;
        isSliderControl = false; // Movement is initiated by tapping, not slider

        // Set the target waypoint index based on the movement direction
        if (isMovingForward)
        {
            targetWaypointIndex = pathPositions.Length - 1; // Move to the end of the path
        }
        else
        {
            targetWaypointIndex = 0; // Move to the start of the path
        }
    }

    // Function called when the slider value changes
    public void MoveToWaypoint(int index)
    {
        if (pathPositions == null || pathPositions.Length == 0)
            return;

        if (index >= 0 && index < pathPositions.Length)
        {
            targetWaypointIndex = index;
            isMoving = true;
            isSliderControl = true; // Movement is controlled by the slider
        }
    }

    private void MoveAlongPath()
    {
        if (pathPositions == null || pathPositions.Length == 0)
            return;

        if (currentWaypointIndex == targetWaypointIndex)
        {
            isMoving = false;
            return;
        }

        // Determine the direction of movement along the path
        int direction = targetWaypointIndex > currentWaypointIndex ? 1 : -1;

        int nextWaypointIndex = currentWaypointIndex + direction;

        if (nextWaypointIndex < 0 || nextWaypointIndex >= pathPositions.Length)
        {
            isMoving = false;
            return;
        }

        Vector3 targetPosition = pathPositions[nextWaypointIndex];
        Vector3 dir = (targetPosition - transform.position).normalized;

        // Move the ship towards the next waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Rotate the ship to face the target direction
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            targetRotation *= rotationOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Check if the ship has reached the next waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex = nextWaypointIndex;
        }
    }

    // Optional: Function to reverse the movement direction
    public void ReverseDirection()
    {
        isMovingForward = !isMovingForward;
    }

    // Property to expose isMoving state
    public bool IsMoving
    {
        get { return isMoving; }
    }
}
