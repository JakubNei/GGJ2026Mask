using UnityEngine;

public class ToughMask : ItemBase
{

    public ToughMask()
    {
        maskType = MaskType.Tough;
    }

    public override void OnEquip()
    {
        if (!PlayerController.Instance)
            return;
        PlayerController.Instance.CanPlayerPushObjects = true;
        PlayerController.Instance.CanPlayerPullObjects = true;
    }

    public override void OnUnequip()
    {
        if (!PlayerController.Instance)
            return;
        PlayerController.Instance.CanPlayerPushObjects = false;
        PlayerController.Instance.CanPlayerPullObjects = false;
    }
}
