using UnityEngine;
using UnityEngine.Rendering;

public class ShamanMask : ItemBase
{
    public ShamanMask()
    {
        maskType = MaskType.Shaman;
    }
    
    public override void OnEquip()
    {
        NormalPlane.Instance.gameObject.SetActive(false);
        AstralPlane.Instance.gameObject.SetActive(true);
    }

    public override bool CanUnequip()
    {
        return base.CanUnequip();
    }

    public override void OnUnequip()
    {
        AstralPlane.Instance.gameObject.SetActive(false);
        NormalPlane.Instance.gameObject.SetActive(true);
    }
}
