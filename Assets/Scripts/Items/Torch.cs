using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : ItemBase
{
    public bool IsTorchLit { get; private set; }
    [SerializeField] Sprite spriteUnlit;
    [SerializeField] Sprite spriteLit;

    SpriteRenderer render;
    void Awake()
    {
        render = GetComponentInChildren<SpriteRenderer>();
    }

    public override void OnPickupToInventory()
    {
        render.sortingOrder = 3;
        render.sprite = IsTorchLit ? spriteLit : spriteUnlit;
        GameController.Instance.OnTorchTaken();
    }

    public void OnTorchLit()
    {
        render.sortingOrder = 3;
        IsTorchLit = true;
        render.sprite = spriteLit;
        GameController.Instance.OnTorchLit();
    }
}
