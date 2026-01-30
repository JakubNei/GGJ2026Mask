
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PortalToLevel : MonoBehaviour
{
    public string levelName;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == PlayerController.Instance.gameObject)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
        }
    }
}
