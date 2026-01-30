using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite dogFaceOnHover;
    public Button Button { get; private set; }

    void Awake()
    {
        Button = GetComponentInChildren<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.i.PlaySfx(AudioId.HoveringOverMenu);
        GetComponentInParent<MenuController>(true)?.SetDogFaceSprite(dogFaceOnHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}