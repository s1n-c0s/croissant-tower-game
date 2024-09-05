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

    public float maxZoom = 40f;
    public float minZoom = 10f;
    public float zoomLimiter = 50f;

    public Collider2D cameraConfiner; // Define the 2D camera confiner
    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
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
        float requiredSize = GetRequiredSize(greatestDistance);

        // Clamp the zoom value to ensure it fits within the confiner
        float clampedSize = Mathf.Clamp(requiredSize, minZoom, maxZoom);

        // Smoothly transition to the new zoom size
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, clampedSize, Time.deltaTime * smoothTime);
    }

    float GetRequiredSize(float greatestDistance)
    {
        if (cameraConfiner == null) return cam.orthographicSize;

        // Determine the aspect ratio of the camera
        float aspectRatio = cam.aspect;

        // Calculate the confiner bounds
        Bounds confinerBounds = cameraConfiner.bounds;
        float confinerWidth = confinerBounds.size.x;
        float confinerHeight = confinerBounds.size.y;

        // Calculate the required size to fit the targets within the confiner
        float requiredSize = Mathf.Max(
            greatestDistance / (2 * aspectRatio), // Width-based zoom
            greatestDistance / 2                  // Height-based zoom
        );

        // Ensure the required size does not exceed the confiner's limits
        float maxSize = Mathf.Min(confinerWidth / (2 * aspectRatio), confinerHeight / 2);
        return Mathf.Min(requiredSize, maxSize);
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
