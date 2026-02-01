
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PortalToLevel : MonoBehaviour
{
    public string levelName;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (
            PlayerController.Instance != null &&
            PlayerController.Instance.controllingCharacter &&
            other.gameObject == PlayerController.Instance.controllingCharacter.gameObject)
        {
            GoToNextLevel();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GoToNextLevel();
        }
    }

    void GoToNextLevel()
    {
        Debug.Log("Loading level: " + levelName);

        // Use smooth transition if available, otherwise fallback to direct load
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(levelName);
        else
            SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }
}
