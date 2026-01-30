using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GuardFov : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] GuardController guard;

    public void OnPlayerTriggered(PlayerController player)
    {
        StartCoroutine(guard.TriggerKnockout(player));
    }
    public bool TriggerReapeatedly => true;

}
