using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryItems : MonoBehaviour
{
    public List<ItemBase> itemList = new List<ItemBase>();
    private List<Image> itemImages = new List<Image>();

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
    GetItemBasedOnKeyPressed();
}

public void AddItem(ItemBase item)
{
    if (itemList.Contains(item))
    { 
        return;
    }
    itemList.Add(item);
    UpdateItemSprites();
        item.OnPickupToInventory();
}

public void RemoveItem(ItemBase itemPrefab)
{
    ItemBase itemToRemove = itemPrefab;
    if (itemToRemove != null)
    {
        itemList.Remove(itemToRemove);
        UpdateItemSprites();
    }
}

public void DropAllItemsAndPutThemOnANewPlace(PlayerController player)
{
    hasDropped = true;
    float maxOffset = 5f;

    foreach (var item in itemList)
    {
        GameObject itemGameObject = item.gameObject;

        itemGameObject.transform.position = player.transform.position;

        float randomXOffset = Random.Range(-maxOffset, maxOffset);
        float randomYOffset = Random.Range(-maxOffset, maxOffset);

        Vector3 destination = new Vector3(randomXOffset, randomYOffset, 0);

        itemGameObject.transform.DOMove(destination, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            itemGameObject.GetComponent<ItemBase>().SnapToTileAtCurrentPosition();
        });
    }

    itemList.Clear();
    currentItemIndex = 0;
    selectedItem = null;
    UpdateItemSprites();
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
    else if (Input.GetKeyDown(KeyCode.Tab))
    {
        if (itemList.Count != 0)
            currentItemIndex = (currentItemIndex + 1) % itemList.Count;
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
}
