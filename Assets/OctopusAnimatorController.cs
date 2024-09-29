using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusAnimatorController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    private Rigidbody[] ragdollBodies; // Array of tentacle Rigidbodies

    void Start()
    {
        // Get all the Rigidbodies in the tentacles (or armature)
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        // Initially disable the Rigidbodies to allow for animation
        SetRagdollMode(false); // Start with animations
    }

    // Function to toggle between ragdoll and animation modes
    public void SetRagdollMode(bool isRagdoll)
    {
        if (isRagdoll)
        {
            // Disable Animator (switch to ragdoll mode)
            animator.enabled = false;

            // Enable physics (enable Rigidbodies)
            foreach (Rigidbody rb in ragdollBodies)
            {
                rb.isKinematic = false; // Physics control
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
            }
        }
    }

    // You can call this function based on a trigger or event (e.g., a keypress)
    void Update()
    {
        // Example: Switch to ragdoll mode when the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetRagdollMode(true); // Enable ragdoll
        }
        
        // Switch back to animation mode when the "A" key is pressed
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetRagdollMode(false); // Enable animation
        }
    }
}
