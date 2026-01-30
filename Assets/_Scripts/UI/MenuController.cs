using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] Image dogFace;

    List<MenuButton> menuItems = new();
    

    int selectedItem = 0;

    private void Awake()
    {
        menuItems = GetComponentsInChildren<MenuButton>().ToList();
    }

    public void OpenMenu_InGame()
    {
        SetGoActive("Resume", true);
        SetGoActive("Dog is happy", false);
        OpenMenu();
    }

    public void OpenMenu_PlayerDead()
    {
        SetGoActive("Resume", false);
        SetGoActive("Dog is happy", true);
        OpenMenu();        
    }


    void SetGoActive(string prefix, bool newActive)
    {
        foreach (var t in this.gameObject.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name.StartsWith(prefix))
            {
                t.gameObject.SetActive(newActive);
            }
        }
    }

    void OpenMenu()
    {
        gameObject.SetActive(true);
        UpdateItemSelection();
    }
    public void SetDogFaceSprite(Sprite dogFaceOnHover)
    {
        if (dogFaceOnHover)
            dogFace.sprite = dogFaceOnHover;
    }

    public void ClickedOnGoToMainMenu()
    {
        GoToMainMenu();
    }
    public void ClickedOnResume()
    {
        AudioManager.i.PlaySfx(AudioId.ButtonClick);
        CloseMenu();
    }
    public void ClickedOnRestart()
    {
        AudioManager.i.PlaySfx(AudioId.ButtonClick);
        RestartGame();
    }
    
    void LoadScene(int id)
    {
        SceneManager.MoveGameObjectToScene(EssentialObjects.Instance.gameObject, SceneManager.GetActiveScene());
        SceneManager.LoadScene(id);
    }
    void RestartGame()
    {
        LoadScene(1);
    }

    void GoToMainMenu()
    {
        LoadScene(0);
    }
    void CloseMenu()
    {
        gameObject.SetActive(false);
        GameController.Instance.gameState = GameState.FreeRoam;
    }

    public void HandleUpdate()
    {
        // int prevSelection = selectedItem;

        // if (Input.GetKeyDown(KeyCode.DownArrow))
        //     ++selectedItem;
        // else if (Input.GetKeyDown(KeyCode.UpArrow))
        //     --selectedItem;

        // selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        // if (prevSelection != selectedItem)
        //     UpdateItemSelection();

        // if (DialogManager.WaitForInput_GetKeyDown())
        // {
        //     //OnMenuSelected(selectedItem);
        //     AudioManager.i.PlaySfx(AudioId.ButtonClick);
        //     CloseMenu();
        // }
        // else
        if (PlayerController.OpenMenuKeyDown())
        {
            CloseMenu();
        }
    }


    void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
                menuItems[i].Button.Select();
        }
    }

}
