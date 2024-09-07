using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PredictionLine2D : MonoBehaviour
{
    [Header("Prediction Settings")]
    public Transform startPoint;        // The initial position of the prediction
    public Vector2 initialVelocity;     // The initial velocity of the prediction
    public int resolution = 30;         // The number of points in the prediction line
    public float timeStep = 0.1f;       // The time step between each point
    public LayerMask collisionMask;     // The layer to detect collisions

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        VisualizePrediction();
    }

    private void VisualizePrediction()
    {
        List<Vector3> points = new List<Vector3>();
        Vector2 startPosition = startPoint.position;
        Vector2 velocity = initialVelocity;

        for (int i = 0; i < resolution; i++)
        {
            points.Add(startPosition);

            // Check for collision with 2D physics
            RaycastHit2D hit = Physics2D.Raycast(startPosition, velocity.normalized, velocity.magnitude * timeStep, collisionMask);
            if (hit.collider != null)
            {
                // Stop the prediction at the collision point
                points.Add(hit.point);
                break;
            }

            // Update position based on velocity and gravity
            startPosition += velocity * timeStep;
            velocity += Physics2D.gravity * timeStep; // Use 2D gravity
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
