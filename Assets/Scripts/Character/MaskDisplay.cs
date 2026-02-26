using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages mask display on a character. Attach to a "Mask" parent GameObject
/// with individual mask sprites as children.
/// </summary>
public class MaskDisplay : MonoBehaviour
{
    [System.Serializable]
    public class MaskEntry
    {
        public MaskType maskType;
        public GameObject maskObject;
        public Sprite spriteFront;  // sprite when facing down (front view)
        public Sprite spriteBack;   // sprite when facing up (back view)
    }

    [SerializeField] List<MaskEntry> masks = new List<MaskEntry>();
    [SerializeField] int sortingOrderFront = 1;   // sorting order when in front of body
    [SerializeField] int sortingOrderBack = -1;   // sorting order when behind body

    Dictionary<MaskType, MaskEntry> maskLookup;
    public MaskType currentMask = MaskType.Default;
    private FacingDirection currentFacing = FacingDirection.Down;

    void Awake()
    {
        BuildLookup();
    }

    void BuildLookup()
    {
        maskLookup = new Dictionary<MaskType, MaskEntry>();
        foreach (var entry in masks)
        {
            if (entry.maskObject != null)
            {
                maskLookup[entry.maskType] = entry;
            }
        }
    }

    public void SwitchMask(MaskType newMask)
    {
        if (maskLookup == null)
            BuildLookup();

        // Hide current mask
        if (maskLookup.TryGetValue(currentMask, out var currentEntry))
        {
            currentEntry.maskObject.SetActive(false);
        }

        currentMask = newMask;

        if (newMask != MaskType.CutSceneNone)
        {
            // Show new mask
            if (maskLookup.TryGetValue(currentMask, out var newEntry))
            {
                newEntry.maskObject.SetActive(true);
                UpdateMaskSprite(newEntry);
            }
        }
    }

    public void SetFacingDirection(FacingDirection facing)
    {
        if (currentFacing == facing)
            return;

        currentFacing = facing;

        if (maskLookup == null)
            BuildLookup();

        if (maskLookup.TryGetValue(currentMask, out var entry))
        {
            UpdateMaskSprite(entry);
        }

        // Flip mask holder (only when facing down)
        Vector3 maskScale = transform.localScale;
        maskScale.x = currentFacing == FacingDirection.Up ? Mathf.Abs(maskScale.x) : (currentFacing == FacingDirection.Left ? -Mathf.Abs(maskScale.x) : Mathf.Abs(maskScale.x));
        transform.localScale = maskScale;
    }

    private void UpdateMaskSprite(MaskEntry entry)
    {
        var sr = entry.maskObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError($"[MaskDisplay] No SpriteRenderer on mask {entry.maskType}");
            return;
        }

        if (currentFacing == FacingDirection.Up)
        {
            // Facing up - show back sprite, render behind body
            if (entry.spriteBack != null)
                sr.sprite = entry.spriteBack;
            else
                Debug.LogWarning($"[MaskDisplay] spriteBack is null for {entry.maskType}");
            sr.sortingOrder = sortingOrderBack;
        }
        else
        {
            // Facing down/left/right - show front sprite, render in front of body
            if (entry.spriteFront != null)
                sr.sprite = entry.spriteFront;
            else
                Debug.LogWarning($"[MaskDisplay] spriteFront is null for {entry.maskType}");
            sr.sortingOrder = sortingOrderFront;
        }
    }

    public MaskType CurrentMask => currentMask;
    public FacingDirection CurrentFacing => currentFacing;
}
