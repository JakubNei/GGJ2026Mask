using UnityEngine;
using UnityEngine.Tilemaps;

public class ShamanMask : ItemBase
{
    public ShamanMask()
    {
        maskType = MaskType.Shaman;
    }

    bool PlayersOverlaps(TilemapCollider2D tilemap)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is null when checking player overlap");
            return false;
        }

        var p = PlayerController.Instance.controllingCharacter.transform.position;

        var cs = Physics2D.OverlapPointAll(p);
        foreach (var col in cs)
        {
            if (col == tilemap)
            {
                Debug.Log($"Yes! Player overlaps {tilemap}");
                return true;
            }
        }

        Debug.Log($"Player does not overlap {tilemap}");
        return false;
    }

    public override bool CanEquip()
    {
        if (!AstralPlane.Instance)
            return true;
        if (AstralPlane.Instance?.gameObject.activeSelf ?? false)
            return true; // fallback, should not happen

        return PlayersOverlaps(AstralPlane.Instance.RequiredToSwitchInto);
    }
    public override void OnEquip()
    {
        if (NormalPlane.Instance)
            NormalPlane.Instance.gameObject.SetActive(false);
        if (AstralPlane.Instance)
            AstralPlane.Instance.gameObject.SetActive(true);
        AudioManager.i?.PlaySfx(AudioId.AstralTransition);
    }

    public override bool CanUnequip()
    {
        if (NormalPlane.Instance?.gameObject.activeSelf ?? true)
            return true; // fallback, should not happen

        return PlayersOverlaps(NormalPlane.Instance.RequiredToSwitchInto);
    }

    public override void OnUnequip()
    {
        if (AstralPlane.Instance)
            AstralPlane.Instance.gameObject.SetActive(false);
        if (NormalPlane.Instance)
            NormalPlane.Instance.gameObject.SetActive(true);
        AudioManager.i?.PlaySfx(AudioId.AstralTransition);
    }
}
