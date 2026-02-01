using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Dependencies.NCalc;

public class InventoryItems : MonoBehaviour
{
    public List<ItemBase> itemList = new List<ItemBase>();
    private List<Image> itemImages = new List<Image>();
    public List<GameObject> frames = new List<GameObject>();
    private ItemBase equippedItem;

    public ItemBase EquippedItem
    {
        get { return equippedItem; }
    }

    public ItemBase selectedItem;

    public static InventoryItems Instance { get; private set; }

    public List<GameObject> DebugAddAllMasks;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            Image image = child.GetComponent<Image>();
            if (image != null)
            {
                itemImages.Add(image);
            }
        }
        UpdateItemSprites();
    }

    private void Update()
    {
         int? itemIndex = null;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            itemIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            itemIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            itemIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            itemIndex = 3;

      
        if (itemIndex.HasValue && itemIndex.Value < itemList.Count)
        {
            EquipItem(itemList[itemIndex.Value]);
        }

        if (DebugAddAllMasks != null && DebugAddAllMasks.Count > 0 && Input.GetKeyDown(KeyCode.P))
        {
            var i = GameObject.Instantiate(DebugAddAllMasks[0]);
            AddItem(i.GetComponent<ItemBase>());
            DebugAddAllMasks.RemoveAt(0);
        }
    }

    public void AddItem(ItemBase item)
    {
        if (itemList.Contains(item))
            return;

        item.gameObject.transform.parent = null;
        DontDestroyOnLoad(item.gameObject);
        item.transform.position = new Vector3(1000 + 100 * itemList.Count, 1000, 0);
        itemList.Add(item);
        UpdateItemSprites();
        item.OnPickupToInventory();

        // Play pickup sound
        AudioManager.i?.PlaySfx(AudioId.PickupMask);

        EquipItem(item);
    }

    public void RemoveItem(ItemBase itemPrefab)
    {
        // undo DontDestroyOnLoad
        SceneManager.MoveGameObjectToScene(itemPrefab.gameObject, SceneManager.GetActiveScene());

        ItemBase itemToRemove = itemPrefab;
        if (itemToRemove != null)
        {
            itemList.Remove(itemToRemove);
            UpdateItemSprites();
        }
    }

    private void UpdateItemSprites()
    {
        for (int i = 0; i < itemImages.Count; i++)
        {
            if (i < itemList.Count)
            {
                itemImages[i].enabled = true;
                itemImages[i].color = itemList[i].GetIconColor();
                itemImages[i].sprite = itemList[i].GetIcon();
            }
            else
            {
                itemImages[i].enabled = false;
            }
        }
    }

    public void EquipItem(ItemBase newItem)
    {
        if (equippedItem != newItem)
        {
            if (newItem != null && !newItem.CanEquip())
            {
                Debug.Log($"New item not equippable currently {newItem}");
                return;
            }            
            if (equippedItem != null)
            {
                if (!equippedItem.CanUnequip())
                {
                    Debug.Log($"Current item not unequippable {equippedItem}");
                    return;
                }

                equippedItem.OnUnequip();
                frames[itemList.IndexOf(equippedItem)].SetActive(false);
            }

            if (newItem != null)
            {
                // Play switch sound (only if switching from one mask to another, not initial equip)
                if (equippedItem != null)
                {
                    AudioManager.i?.PlaySfxWithVolume(AudioId.SwitchMask, 0.5f);
                }

                equippedItem = newItem;
                equippedItem.OnEquip();
                frames[itemList.IndexOf(equippedItem)].SetActive(true);
            }

            Debug.Log($"Equipped {newItem} {newItem.MaskType}");
        }
    }
}
