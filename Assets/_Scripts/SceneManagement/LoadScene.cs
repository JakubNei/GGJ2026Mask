using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public void Play(int buildIndex)
    {
        AudioManager.i.PlaySfx(AudioId.ButtonClick);
        SceneManager.LoadScene(buildIndex);
    }

    public void QuitGame()
    {
        AudioManager.i.PlaySfx(AudioId.ClosingMenu);
        Application.Quit();
    }
}
