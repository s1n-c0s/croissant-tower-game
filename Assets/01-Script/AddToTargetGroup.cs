using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class AddToTargetGroup : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup; // Reference to the Cinemachine Target Group
    public string targetTag = "Croissant";     // Tag used to find targets in the scene

    private HashSet<Transform> cachedTargets = new HashSet<Transform>(); // Set to keep track of already added targets

    private void Start()
    {
        if (targetGroup != null && !string.IsNullOrEmpty(targetTag))
        {
            // Initialize with current targets in the Cinemachine Target Group
            CacheExistingTargets();
            // Add new targets found by tag
            AddTargetsByTag(targetTag);
        }
    }

    // Cache existing targets already present in the Cinemachine Target Group
    private void CacheExistingTargets()
    {
        foreach (var t in targetGroup.m_Targets)
        {
            cachedTargets.Add(t.target); // Cache each existing target
        }
    }

    private void LateUpdate()
    {
        // Continuously check for new targets by tag and add them if not already present
        if (targetGroup != null && !string.IsNullOrEmpty(targetTag))
        {
            AddTargetsByTag(targetTag);
        }
    }

    // Method to find all targets by tag and add them to the Cinemachine Target Group
    public void AddTargetsByTag(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag); // Find all GameObjects with the specified tag

        foreach (GameObject target in targets)
        {
            AddTarget(target.transform);
        }
    }

    // Method to add a target to the Cinemachine Target Group if it hasn't been added already
    private void AddTarget(Transform target)
    {
        if (!cachedTargets.Contains(target)) // Only add if not already cached
        {
            cachedTargets.Add(target); // Add to cache to prevent future duplicates

            // Convert existing targets to a list and add the new target
            List<CinemachineTargetGroup.Target> newTargets = new List<CinemachineTargetGroup.Target>(targetGroup.m_Targets)
            {
                new CinemachineTargetGroup.Target
                {
                    target = target,
                    weight = 1f,
                    radius = 1f
                }
            };

            targetGroup.m_Targets = newTargets.ToArray(); // Update the target group
        }
    }
}
