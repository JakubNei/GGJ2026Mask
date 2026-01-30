using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuDog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite face;
    [SerializeField] Image dogImage;

    [SerializeField] GameObject buttons;

    public void OnPointerEnter(PointerEventData eventData)
    {                     
        AudioManager.i.PlaySfx(AudioId.HoveringOverMenu);
        dogImage.sprite = face;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        dogImage.sprite = face;
    }

    public void GoToControls(GameObject controls)
    {
        AudioManager.i.PlaySfx(AudioId.ButtonClick);
        buttons.SetActive(false);
        controls.SetActive(true);
    }

    public void ReturnFromControls(GameObject controls)
    {
        AudioManager.i.PlaySfx(AudioId.ButtonClick);
        buttons.SetActive(true);
        controls.SetActive(false);
    }
}
