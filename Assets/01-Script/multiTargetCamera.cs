using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    public string targetTag = "Croissant"; // Define the tag for the targets
    public List<Transform> targets;

    public Vector3 offset;
    public float smoothTime = 0.5f;

    public float maxFOV = 60f; // Maximum field of view
    public float minFOV = 20f; // Minimum field of view
    public float zoomLimiter = 50f;

    public Collider2D cameraConfiner; // Define the 2D camera confiner
    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = false; // Make sure the camera is set to perspective mode
    }

    void LateUpdate()
    {
        FindTargetsByTag();
        if (targets.Count == 0)
            return;

        Move();
        Zoom();
    }

    void FindTargetsByTag()
    {
        // Find all GameObjects with the specified tag
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(targetTag);

        // Clear the existing targets list and add only those within the camera confiner
        targets.Clear();
        foreach (GameObject obj in foundObjects)
        {
            if (cameraConfiner.OverlapPoint(obj.transform.position))
            {
                targets.Add(obj.transform);
            }
        }
    }

    void Zoom()
    {
        float greatestDistance = GetGreatestDistance();
        float requiredFOV = GetRequiredFOV(greatestDistance);

        // Clamp the FOV value to ensure it fits within the confiner
        float clampedFOV = Mathf.Clamp(requiredFOV, minFOV, maxFOV);

        // Smoothly transition to the new FOV
        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, clampedFOV, ref velocity.z, smoothTime);
    }

    float GetRequiredFOV(float greatestDistance)
    {
        if (cameraConfiner == null) return cam.fieldOfView;

        // Determine the camera's distance to the center point
        float cameraDistance = (GetCenterPoint() - transform.position).magnitude;

        // Calculate the required FOV to fit all targets
        float requiredFOV = 2.0f * Mathf.Atan(greatestDistance / (2.0f * cameraDistance)) * Mathf.Rad2Deg;

        return requiredFOV;
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        // Clamp the camera position within the camera confiner bounds
        Vector3 clampedPosition = ClampPositionToConfiner(newPosition);

        transform.position = Vector3.SmoothDamp(transform.position, clampedPosition, ref velocity, smoothTime);
    }

    Vector3 ClampPositionToConfiner(Vector3 targetPosition)
    {
        if (cameraConfiner == null) return targetPosition;

        // Convert target position to 2D
        Vector2 targetPosition2D = new Vector2(targetPosition.x, targetPosition.y);

        // Ensure the target position stays within the bounds of the camera confiner
        if (!cameraConfiner.OverlapPoint(targetPosition2D))
        {
            targetPosition2D = cameraConfiner.ClosestPoint(targetPosition2D);
        }

        return new Vector3(targetPosition2D.x, targetPosition2D.y, transform.position.z);
    }

    float GetGreatestDistance()
    {
        if (targets.Count == 0) return 0f;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return Mathf.Max(bounds.size.x, bounds.size.y); // Use the largest distance in either direction
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 0) return Vector3.zero;

        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
