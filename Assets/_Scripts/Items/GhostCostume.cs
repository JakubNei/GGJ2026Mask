using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GhostCostume : ItemBase
{

    public override bool InteractInsteadOfPlace => true;
    public override bool Interact(Vector3 position)
    {
        PlayerController playerController = PlayerController.Instance;
        playerController.characterAnimator.SetIsScaring(true);
        StartCoroutine(ChangeBackToNormal(playerController));

        foreach (var collider in Physics2D.OverlapCircleAll(playerController.Character.transform.position, 2))
        {
            if (collider)
            {
                Grandma grandma = collider.GetComponent<Grandma>();
                if (grandma)
                {
                    grandma.GetScared();
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator ChangeBackToNormal(PlayerController playerController)
    {
        yield return new WaitForSeconds(2);

        playerController.characterAnimator.SetIsScaring(false);
    }
}
