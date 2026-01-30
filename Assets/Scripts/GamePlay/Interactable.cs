using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool canPickUp();
    void UpdateWhileInteracting();
    bool CanInteract();
}
