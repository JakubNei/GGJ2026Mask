using UnityEngine;

public class EndCutScene : MonoBehaviour
{
    public void RemovePlayerMask()
    {
        var character = PlayerController.Instance?.controllingCharacter;
        if (character != null)
        {
            character.CurrentMask = MaskType.None;
        }
    }
}
