using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool CanPickUp();
    void UpdateWhileInteracting();
    bool CanInteract();
}
