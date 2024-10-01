using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SetOverlayCameraViewport : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        float height = 0.3333f;
        cam.rect = new Rect(0f, 0f, 1f, height);
    }
}