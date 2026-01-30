using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : ItemBase
{
    [SerializeField] Sprite[] defaultSprites;
    [SerializeField] Sprite[] poisonedSprites;

    public bool IsPoisoned { get; private set; } = false;

    public CakeQuest cakeQuest;


    void Update()
    {
        if (IsPoisoned)
        {
            CycleSprites(poisonedSprites);
        }
        else
        {
            CycleSprites(defaultSprites);

            foreach (var collider in Physics2D.OverlapCircleAll(transform.position, 1))
            {
                if (collider.gameObject.GetComponent<Poison>())
                {
                    SetIsPoisoned();
                    Destroy(collider.gameObject);
                    break;
                }
            }
        }


        foreach (var collider in Physics2D.OverlapCircleAll(transform.position, 1))
        {
            var torch = collider.gameObject.GetComponent<Torch>();
            if (torch != null && !torch.IsTorchLit)
            {
                torch.OnTorchLit();
            }
        }
    }

    void SetIsPoisoned()
    {
        IsPoisoned = true;
        cakeQuest.QuestDone();
    }

    void CycleSprites(Sprite[] sprites)
    {
        SetSprite(sprites[Mathf.FloorToInt((float)(Time.timeSinceLevelLoadAsDouble / 0.2f)) % sprites.Length]);
    }
    void SetSprite(Sprite sprite)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            spriteRenderer.sprite = sprite;
    }

}
