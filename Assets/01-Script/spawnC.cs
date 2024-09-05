using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnC : MonoBehaviour
{
    [SerializeField] private GameObject croissant_prefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Lean.Pool.LeanPool.Spawn(croissant_prefab, transform.position + transform.forward, transform.rotation);
        }
    }
}
