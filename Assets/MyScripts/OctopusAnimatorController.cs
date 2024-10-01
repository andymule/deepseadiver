using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusAnimatorController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    private Rigidbody[] ragdollBodies; // Array of tentacle Rigidbodies
    private bool isRagdoll = false;
    private bool seafloorFound = false;
    private bool isSwimming = false;
    private GameObject seafloorObject;
    private GameObject shipObject; // Reference to the ship
    public float dropHeight = 0.3f; // 30 cm above the seafloor
    public float swimSpeed = 0.1f; // Adjust as needed
    public float detectionRadius = 0.15f; // 15 cm detection radius
    private Renderer[] renderers; // Array to hold renderer components
    private Transform armatureTransform; // Reference to the Armature

    void Start()
    {
        // Get all the Rigidbodies in the tentacles (or armature)
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        // Initially disable the Rigidbodies to allow for animation
        SetRagdollMode(false); // Start with animations

        // Get all Renderer components
        renderers = GetComponentsInChildren<Renderer>();

        // Hide the octopus by disabling its renderers
        SetOctopusVisible(false);

        // Find the Armature child object
        armatureTransform = GameObject.Find("HeadReal").transform;

        if (armatureTransform == null)
        {
            Debug.LogError("Armature object not found in octopus hierarchy.");
        }
    }

    void Update()
    {
        // Wait until the seafloor spawns
        if (!seafloorFound)
        {
            seafloorObject = GameObject.FindWithTag("Seafloor");
            if (seafloorObject != null)
            {
                seafloorFound = true;
                PositionOctopusAboveSeafloor();
            }
        }
        else if (!isSwimming)
        {
            // Check if the ship is within detection radius
            if (shipObject == null)
            {
                // Find the ship object (ensure it has the tag "Ship")
                shipObject = GameObject.FindWithTag("Ship");
            }

            if (shipObject != null && armatureTransform != null)
            {
                // Use the Armature's position for accurate detection
                Vector3 octopusPosition = armatureTransform.position;
                Vector3 shipPosition = shipObject.transform.position;

                // Calculate the distance in world space
                float distance = Vector3.Distance(octopusPosition, shipPosition);

                // Debugging statements
                // Debug.Log("Octopus Position: " + octopusPosition);
                // Debug.Log("Ship Position: " + shipPosition);
                // Debug.Log("Distance: " + distance);

                if (distance <= detectionRadius)
                {
                    // Switch to swimming mode
                    SetRagdollMode(false);
                    isSwimming = true;

                    // Trigger the swimming animation
                    animator.SetBool("isSwimming", true);

                    // Start moving upwards
                    StartCoroutine(SwimUpwards());
                }
            }
        }
    }

    void PositionOctopusAboveSeafloor()
    {
        // Show the octopus
        SetOctopusVisible(true);

        // Position the octopus off-center above the seafloor's position
        Vector3 seafloorPosition = seafloorObject.transform.position;

        // Keep the initial X and Z positions (off-center spawning)
        float initialX = transform.position.x;
        float initialZ = transform.position.z;

        // Set the new position
        transform.position = new Vector3(
            seafloorPosition.x + initialX,
            seafloorPosition.y + dropHeight,
            seafloorPosition.z + initialZ
        );

        // Ensure the octopus is not parented to avoid scaling issues
        transform.parent = null;

        // Enable ragdoll mode to allow the octopus to drop onto the seafloor
        SetRagdollMode(true);
    }

    // Function to toggle between ragdoll and animation modes
    public void SetRagdollMode(bool enableRagdoll)
    {
        isRagdoll = enableRagdoll;

        if (isRagdoll)
        {
            // Disable Animator (switch to ragdoll mode)
            animator.enabled = false;

            // Enable physics (enable Rigidbodies)
            foreach (Rigidbody rb in ragdollBodies)
            {
                rb.isKinematic = false; // Physics control
                rb.useGravity = true;
            }
        }
        else
        {
            // Enable Animator (switch to animation mode)
            animator.enabled = true;

            // Disable physics (disable Rigidbodies)
            foreach (Rigidbody rb in ragdollBodies)
            {
                rb.isKinematic = true; // Animator control
                rb.useGravity = false;
            }

            // Ensure the Animator parameter isSwimming is set appropriately
            animator.SetBool("isSwimming", false);
        }
    }

    IEnumerator SwimUpwards()
    {
        // Ensure the Animator is enabled
        animator.enabled = true;

        // Set the isSwimming parameter to true to trigger swimming animation
        animator.SetBool("isSwimming", true);

        // Move upwards indefinitely
        while (true)
        {
            // Move the octopus upwards slowly
            transform.position += Vector3.up * swimSpeed * Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }
    }

    // Function to hide or show the octopus
    void SetOctopusVisible(bool isVisible)
    {
        foreach (Renderer rend in renderers)
        {
            rend.enabled = isVisible;
        }
    }
}
