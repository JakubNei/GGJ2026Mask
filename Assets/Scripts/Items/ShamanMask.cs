using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class ShamanMask : ItemBase
{
    public ShamanMask()
    {
        maskType = MaskType.Shaman;
    }

    bool PlayersOverlaps(Tilemap tilemap)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is null when checking player overlap");
            return false;
        }
        var c = PlayerController.Instance.controllingCharacter;
        var p = c.transform.position;

        var wc = tilemap.WorldToCell(p);
        var t = tilemap.GetTile(wc);
        if (t != null)
        {
            Debug.Log($"Player overlaps {tilemap}");
            return true;
        }
        Debug.Log($"Player does not overlap {tilemap}");
        return false;
    }

    public override bool CanEquip()
    {
        return PlayersOverlaps(AstralPlane.Instance.RequiredToSwitchInto);
    }
    public override void OnEquip()
    {
        NormalPlane.Instance.gameObject.SetActive(false);
        AstralPlane.Instance.gameObject.SetActive(true);
    }

    public override bool CanUnequip()
    {
        return PlayersOverlaps(NormalPlane.Instance.RequiredToSwitchInto);
    }

    public override void OnUnequip()
    {
        AstralPlane.Instance.gameObject.SetActive(false);
        NormalPlane.Instance.gameObject.SetActive(true);
    }
}
