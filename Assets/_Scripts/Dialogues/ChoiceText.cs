using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Color textSelectedColor;
    [SerializeField] Color textDefaultColor;
    [SerializeField] GameObject selected;


    public void SetSelected(bool newSelected)
    {
        text.color = newSelected ? textSelectedColor : textDefaultColor;
        selected.SetActive(newSelected);
    }

    public Text TextField => text;
}

