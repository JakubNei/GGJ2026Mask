using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum MaskType
{
    Default,
    Shaman, 
    Tough,
    Ninja,
    CutSceneNone, // Final end cut scene no mask
}

public class ItemBase : MonoBehaviour, IInteractable
{
    protected MaskType maskType;
    public MaskType MaskType
    {
        get { return maskType; }  
        set { maskType = value; } 
    }

    public virtual bool InteractInsteadOfPlace => false;


    public virtual Sprite GetIcon()
    {
        return GetComponentInChildren<SpriteRenderer>()?.sprite;
    }
    public virtual Color GetIconColor()
    {
        var c = GetComponentInChildren<SpriteRenderer>();
        if (c)
            return c.color;
        return Color.white;
    }

    public virtual void OnPickupToInventory()
    {

        GetComponent<SpriteRenderer>().enabled = false;
    }

    public virtual bool CanEquip() { return true; }
    public virtual void OnEquip() { }
    public virtual bool CanUnequip() { return true; }
    public virtual void OnUnequip() { }

  
    public void UpdateWhileInteracting()
    {
        Debug.Log("UpdateWhileInteracting");
        return;
    }

    public bool CanInteract()
    {
        return true;
    }

    public bool CanPickUp()
    {
       return true;
    }
}
