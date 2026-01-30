using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBase : MonoBehaviour
{

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

    }

    void Awake()
    {
        SnapToTileAtCurrentPosition();
    }
    
    public void SnapToTileAtCurrentPosition()
    {
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;

        transform.position = pos;
    }

    public virtual bool Interact(Vector3 position)
    {
        return true;
    }
}
