using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableDel : MonoBehaviour
{
    [SerializeField] float lifetime = 1f;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.CompareTag("Croissant"))
        {
            Lean.Pool.LeanPool.Despawn(other.gameObject, lifetime);
        }
    }
}
