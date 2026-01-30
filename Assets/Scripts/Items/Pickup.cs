using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup : MonoBehaviour, Interactable
{
    static int inInventoryCounter = 0;
    public bool CanInteract()
    {
        return true;
    }
    public IEnumerator Interact(Transform initiator)
    {
        if (InventoryItems.Instance.HasFreeSlots)
        {
            ItemBase item = GetComponent<ItemBase>();

            InventoryItems.Instance.AddItem(item);

            // Used = true;
            // GetComponent<SpriteRenderer>().enabled = false;
            // GetComponent<BoxCollider2D>().enabled = false;

            // string playerName = initiator.GetComponent<PlayerController>().Name;

            // AudioManager.i.PlaySfx(AudioId.ItemObtained, pauseMusic: true);
            //yield return DialogManager.Instance.ShowDialogText($"{playerName} found {item.Name}");

            transform.position = new Vector3(-1000 - 5 * inInventoryCounter, -1000, 0);
            inInventoryCounter++;

            item.OnPickupToInventory();
        }
        yield return null;
    }

}
