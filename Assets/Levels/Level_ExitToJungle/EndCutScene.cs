using UnityEngine;
using UnityEngine.Playables;

public class EndCutScene : MonoBehaviour
{
    void Start()
    {
        PlayerController.Instance.controllingCharacter.CurrentMask = MaskType.Default;
    }

    [ContextMenu("RemovePlayerMask")]
    public void RemovePlayerMask()
    {
        PlayerController.Instance.controllingCharacter.CurrentMask = MaskType.CutSceneNone;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        StartCutScene();
    }
    
    [ContextMenu("Start Cut Scene")]
    void StartCutScene()
    {
        var d = GetComponent<PlayableDirector>();
        d.enabled = true;
    }
}
