using UnityEngine;

public class ToughMask : ItemBase
{

    public ToughMask() 
    {
        maskType = MaskType.Tough;
    }
    public override void OnPickupToInventory()
    {
        base.OnPickupToInventory();

        PlayerController.Instance.CanPlayerPushObjects = true;
        PlayerController.Instance.CanPlayerPullObjects = true;
    }

}
