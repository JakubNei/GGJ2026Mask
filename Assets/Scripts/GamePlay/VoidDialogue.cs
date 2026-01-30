using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidDialogue : MonoBehaviour, IPlayerTriggerable
{

    private bool isPlayerColliding = false;
    private float collisionTimer = 0f;
    private float collisionThreshold = 10f;

    public bool TriggerReapeatedly => true;

    public void OnPlayerTriggered(PlayerController player)
    {
        isPlayerColliding = true;
    }

    private void Update()
    {
        if (isPlayerColliding)
        {
            collisionTimer += Time.deltaTime;

            if (collisionTimer >= collisionThreshold)
            {
                //StartCoroutine(DialogManager.Instance.TypeDialog("Whoever fights monsters should see to it that in the process he does not become a monster."));
                //StartCoroutine(DialogManager.Instance.TypeDialog("And if you gaze long enough into an abyss, the abyss will gaze back into you."));
            }
        }
    }
}
