using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public interface IInteractable
{
    bool CanPickUp();
    void UpdateWhileInteracting();
    bool CanInteract();
    string GetInteractText();
    Color GetHighlightColor();
}
