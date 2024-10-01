using UnityEngine;
using UnityEngine.EventSystems;

public class ARInputHandler : MonoBehaviour
{
    private Camera arCamera;
    private int layerMask;
    private ShipController shipController;

    void Start()
    {
        arCamera = Camera.main;
        layerMask = LayerMask.GetMask("LineCollider");

        // Cache the ShipController reference
        GameObject shipObject = GameObject.FindWithTag("Ship");
        if (shipObject != null)
        {
            shipController = shipObject.GetComponent<ShipController>();
        }
        else
        {
            Debug.LogError("Ship object with tag 'Ship' not found.");
        }
    }

    void Update()
    {
        // Handle input differently in the Unity Editor and on mobile devices
        #if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
        #elif UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
        #endif
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition = Input.mousePosition;

            // Ignore clicks over UI elements
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clicked over UI element; ignoring.");
                return;
            }

            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider != null && hit.collider.gameObject.name.StartsWith("Collider_"))
                {
                    Debug.Log("Clicked on line collider: " + hit.collider.gameObject.name);

                    if (shipController != null)
                    {
                        shipController.StartMoving();
                    }
                }
            }
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Ignore touches over UI elements
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Debug.Log("Touched over UI element; ignoring.");
                return;
            }

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider != null && hit.collider.gameObject.name.StartsWith("Collider_"))
                    {
                        Debug.Log("Tapped on line collider: " + hit.collider.gameObject.name);

                        if (shipController != null)
                        {
                            shipController.StartMoving();
                        }
                    }
                }
            }
        }
    }
}
