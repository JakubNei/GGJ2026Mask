using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovableObject : MonoBehaviour, IInteractable
{
    Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public bool CanInteract()
    {
        return true;
    }


    public void UpdateWhileInteracting()
    {
        if (!PlayerController.Instance.CanPlayerPullObjects)
            return;

        // if (rb.bodyType == RigidbodyType2D.Dynamic)
        //     rb.MovePosition(rb.position + (Vector2)PlayerController.Instance.lastCharacterDeltaMovement);
        // else
        transform.position += PlayerController.Instance.lastCharacterDeltaMovement;
    }

    void Update()
    {
        rb.bodyType =
            PlayerController.Instance.CanPlayerPushObjects ?
            RigidbodyType2D.Dynamic :
            RigidbodyType2D.Static;
    }

}
