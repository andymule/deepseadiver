using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DummyShipController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotateSpeed = 100f;

    void Update()
    {
        // Movement input
        float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveSide = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        // Apply movement
        transform.Translate(moveSide, moveForward, 0);

        // Optional: Rotate with Q and E keys
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
