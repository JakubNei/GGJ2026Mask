using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSprite : MonoBehaviour
{
    class CurrentlyHighlighted
    {
        public SpriteRenderer OutlineSprite_RespectRenderOrder;
       // public SpriteRenderer OutlineSprite_IgnoreRenderOrder;
        public float timeLeftToRevert;
    }

    Dictionary<SpriteRenderer, CurrentlyHighlighted> currentlyHighlighted = new();

    Material OutlineSprite_RespectRenderOrder;
    Material OutlineSprite_IgnoreRenderOrder;

    static HighlightSprite instance;
    static HighlightSprite Instance
    {
        get
        {
            if (!instance)
            {
                var go = new GameObject(nameof(HighlightSprite));
                instance = go.AddComponent<HighlightSprite>();
                instance.LoadResources();
            }
            return instance;
        }
    }

    public static void Outline(GameObject gameObject)
    {
        Outline(gameObject, Color.black);
    }
    public static void Outline(GameObject gameObject, Color outlineColor)
    {
        foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            Instance.OutlineInternal(spriteRenderer, outlineColor);
        }
    }
    public static void Outline(SpriteRenderer spriteRenderer)
    {
        Outline(spriteRenderer, Color.black);
    }
    public static void Outline(SpriteRenderer spriteRenderer, Color outlineColor)
    {
        Instance.OutlineInternal(spriteRenderer, outlineColor);
    }

    void OutlineInternal(SpriteRenderer spriteRenderer, Color outlineColor)
    {
        if (spriteRenderer.gameObject.name == OutlineSprite_RespectRenderOrder.name ||
            spriteRenderer.gameObject.name == OutlineSprite_IgnoreRenderOrder.name)
            return;
        if (spriteRenderer.sortingLayerName == "UI")
            return;
        CurrentlyHighlighted c;
        var colorName = "_OutlineColor";
        if (!currentlyHighlighted.TryGetValue(spriteRenderer, out c))
        {
            c = new CurrentlyHighlighted();
            c.OutlineSprite_RespectRenderOrder = AddSpriteRenderer(spriteRenderer, OutlineSprite_RespectRenderOrder);
            //c.OutlineSprite_IgnoreRenderOrder = AddSpriteRenderer(spriteRenderer, OutlineSprite_IgnoreRenderOrder);
            currentlyHighlighted.Add(spriteRenderer, c);
        }

        if (c.OutlineSprite_RespectRenderOrder.material.GetColor(colorName) != outlineColor)
            c.OutlineSprite_RespectRenderOrder.material.SetColor(colorName, outlineColor);

        // more subtle outline that ignores render order
        outlineColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, outlineColor.a * 0.5f);
        //if (c.OutlineSprite_IgnoreRenderOrder.material.GetColor(colorName) != outlineColor)
            //c.OutlineSprite_IgnoreRenderOrder.material.SetColor(colorName, outlineColor);
        
        c.timeLeftToRevert = 0.1f;
    }

    static SpriteRenderer AddSpriteRenderer(SpriteRenderer spriteRenderer, Material material)
    {
        var go = new GameObject(material.name);
        go.transform.parent = spriteRenderer.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        var sr = go.AddComponent<SpriteRenderer>();
        CopyProperties(sr, spriteRenderer);
        sr.material = material;
        return sr;
    }

    static void CopyProperties(SpriteRenderer to, SpriteRenderer from)
    {
        to.sprite = from.sprite;
        to.drawMode = from.drawMode;
        to.size = from.size;
        to.adaptiveModeThreshold = from.adaptiveModeThreshold;
        to.tileMode = from.tileMode;
        to.color = from.color;
        to.maskInteraction = from.maskInteraction;
        to.flipX = from.flipX;
        to.flipY = from.flipY;
        to.spriteSortPoint = from.spriteSortPoint;
        to.sortingLayerName = from.sortingLayerName;
        to.sortingLayerID = from.sortingLayerID;
        to.sortingOrder = from.sortingOrder;
    }

    void Awake()
    {
        LoadResources();
    }

    void LoadResources()
    {
        OutlineSprite_RespectRenderOrder = Resources.Load<Material>("OutlineSprite_RespectRenderOrder");
        OutlineSprite_IgnoreRenderOrder = Resources.Load<Material>("OutlineSprite_IgnoreRenderOrder");
    }
    List<SpriteRenderer> toRevertNow = new();
    void Update()
    {
        toRevertNow.Clear();
        var t = Time.deltaTime;
        foreach (var pair in currentlyHighlighted)
        {
            pair.Value.timeLeftToRevert -= t;
            if (pair.Value.timeLeftToRevert <= 0)
            {
                toRevertNow.Add(pair.Key);
            }
        }
        foreach (var spriteRenderer in toRevertNow)
        {
            var revertDataNow = currentlyHighlighted[spriteRenderer];
            currentlyHighlighted.Remove(spriteRenderer);
            if (revertDataNow.OutlineSprite_RespectRenderOrder && revertDataNow.OutlineSprite_RespectRenderOrder.gameObject)
                Destroy(revertDataNow.OutlineSprite_RespectRenderOrder.gameObject);
            //if (revertDataNow.OutlineSprite_IgnoreRenderOrder && revertDataNow.OutlineSprite_IgnoreRenderOrder.gameObject)
                //Destroy(revertDataNow.OutlineSprite_IgnoreRenderOrder.gameObject);
        }
    }

}
