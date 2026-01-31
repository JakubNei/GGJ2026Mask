using UnityEngine;

public class EndCutScene : MonoBehaviour
{
    public void RemovePlayerMask()
    {
        var player = PlayerController.Instance?.controllingCharacter;
        if (player != null)
        {
            player.SwitchMask(MaskType.None);
        }
    }
}
