using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovableObject : MonoBehaviour, IInteractable
{
    public Rigidbody2D rb;
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
        
    }

    void Update()
    {
        rb.bodyType =
            PlayerController.Instance.CanPlayerPushObjects ?
            RigidbodyType2D.Dynamic :
            RigidbodyType2D.Static;
    }

}
