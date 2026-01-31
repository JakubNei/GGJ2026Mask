using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

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

    private int currentItemIndex = 0;
    public bool hasDropped = false;
    public bool HasFreeSlots => itemList.Count < 3;
    public static InventoryItems Instance { get; private set; }

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
        ItemBase newItem = GetItemBasedOnKeyPressed();
        if (newItem != null)
        {
            EquipItem(newItem);
        }
    }

    public void AddItem(ItemBase item)
    {
        DontDestroyOnLoad(item.gameObject);
        item.transform.position = new Vector3(1000 + 100 * itemList.Count, 1000, 0);
        if (itemList.Contains(item))
        {
            return;
        }
        itemList.Add(item);
        UpdateItemSprites();
        item.OnPickupToInventory();
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

    public ItemBase GetItemBasedOnKeyPressed()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentItemIndex = 0;
            return selectedItem = itemList.Count >= 1 ? itemList[currentItemIndex] : null;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentItemIndex = 1;
            return selectedItem = itemList.Count >= 2 ? itemList[currentItemIndex] : null;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentItemIndex = 2;
            return selectedItem = itemList.Count >= 3 ? itemList[currentItemIndex] : null;
        }
        else if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.E))
        {
            if (itemList.Count != 0)
                currentItemIndex = (currentItemIndex + 1) % itemList.Count;
            return selectedItem = itemList.Count > 0 ? itemList[currentItemIndex] : null;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (itemList.Count != 0)
                currentItemIndex = (currentItemIndex - 1 + itemList.Count) % itemList.Count;
            return selectedItem = itemList.Count > 0 ? itemList[currentItemIndex] : null;
        }
        else
        {
            return null;
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
            if (equippedItem != null)
            {
                equippedItem.OnUnequip();
                frames[itemList.IndexOf(equippedItem)].SetActive(false);
            }

            equippedItem = newItem;
            equippedItem.OnEquip();
            frames[itemList.IndexOf(equippedItem)].SetActive(true);

            Debug.Log("Equipped " + newItem.MaskType);
        }
    }
}
