using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player movement
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 movement; // Store the player's movement direction

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    void Update()
    {
        // Get horizontal input (A/D keys or Left/Right arrows)
        float moveInput = Input.GetAxisRaw("Horizontal");
        movement = new Vector2(moveInput, 0); // Create a movement vector
    }

    void FixedUpdate()
    {
        // Apply movement to the Rigidbody2D
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }
}
