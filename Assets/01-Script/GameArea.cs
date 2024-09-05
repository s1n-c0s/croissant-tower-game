using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameArea : MonoBehaviour
{

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Croissant"))
        {
            Lean.Pool.LeanPool.Despawn(other);
        }
    }
}
