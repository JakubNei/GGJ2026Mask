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
    }

    [SerializeField] List<MaskEntry> masks = new List<MaskEntry>();

    Dictionary<MaskType, GameObject> maskLookup;
    public MaskType currentMask = MaskType.Default;

    void Awake()
    {
        BuildLookup();
    }

    void BuildLookup()
    {
        maskLookup = new Dictionary<MaskType, GameObject>();
        foreach (var entry in masks)
        {
            if (entry.maskObject != null)
            {
                maskLookup[entry.maskType] = entry.maskObject;
            }
        }
    }

    public void SwitchMask(MaskType newMask)
    {
        if (maskLookup == null)
            BuildLookup();

        // Hide current mask
        if (maskLookup.TryGetValue(currentMask, out var currentObj))
        {
            currentObj.SetActive(false);
        }

        currentMask = newMask;

        if (newMask != MaskType.None)
        {
            // Show new mask
            if (maskLookup.TryGetValue(currentMask, out var newObj))
            {
                newObj.SetActive(true);
            }
        }
    }

    public MaskType CurrentMask => currentMask;
}
