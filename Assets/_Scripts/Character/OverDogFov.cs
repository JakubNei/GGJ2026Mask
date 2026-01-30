using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverDogFov : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        /*player.characterAnimator.IsMoving = false;
        GameController.Instance.OnPlayerInteractedWithOverDog();*/
    }
    public bool TriggerReapeatedly => false;
}
