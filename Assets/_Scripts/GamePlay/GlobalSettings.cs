using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color textNormalColor = Color.white;
    [SerializeField] Color textHighlightedColor = Color.gray;

    public Color TextHighlightedColor => textHighlightedColor;
    public Color TextNormalColor => textHighlightedColor;

    public static GlobalSettings i { get; private set; }

    private void Awake()
    {
        i = this;
    }
}
