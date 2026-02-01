using UnityEngine;
using UnityEngine.Playables;

public class EndCutScene : MonoBehaviour
{
    [ContextMenu("RemovePlayerMask")]
    public void RemovePlayerMask()
    {
        var character = PlayerController.Instance.controllingCharacter;
        character.CurrentMask = MaskType.CutSceneNone;
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
