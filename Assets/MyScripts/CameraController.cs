using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Rotation degrees per press
    public float rotationSpeed = 5f;

    // Methods to be called when buttons are pressed
    public void RotateUp()
    {
        transform.Rotate(rotationSpeed, 0f, 0f, Space.World);
    }

    public void RotateDown()
    {
        transform.Rotate(-rotationSpeed, 0f, 0f, Space.World);
    }

    public void RotateLeft()
    {
        transform.Rotate(0f, -rotationSpeed, 0f, Space.World);
    }

    public void RotateRight()
    {
        transform.Rotate(0f, rotationSpeed, 0f, Space.World);
    }
}