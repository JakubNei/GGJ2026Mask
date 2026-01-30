using System.Collections;
using UnityEngine;

public class MovableObject : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }

    public void UpdateWhileInteracting()
    {
        
    }

}
