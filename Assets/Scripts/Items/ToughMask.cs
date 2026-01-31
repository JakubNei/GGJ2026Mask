using UnityEngine;

public class ToughMask : ItemBase
{

    public ToughMask()
    {
        maskType = MaskType.Tough;
    }

    public override void OnEquip()
    {
        PlayerController.Instance.CanPlayerPushObjects = true;
        PlayerController.Instance.CanPlayerPullObjects = true;
    }

    public override void OnUnequip()
    {
        PlayerController.Instance.CanPlayerPushObjects = false;
        PlayerController.Instance.CanPlayerPullObjects = false;
    }
}
