using UnityEngine;

/// <summary>
/// Trigger collider that blocks player movement but lets projectiles through.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class HoleCollider : MonoBehaviour
{
    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
}
