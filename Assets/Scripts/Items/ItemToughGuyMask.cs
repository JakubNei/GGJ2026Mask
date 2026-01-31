using UnityEngine;

public class ItemToughGuyMask : ItemBase
{

    public ItemToughGuyMask() 
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
