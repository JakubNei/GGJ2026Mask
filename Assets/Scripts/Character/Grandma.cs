using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : MonoBehaviour, Interactable
{
    [SerializeField] private Character character;
    [SerializeField] private Sprite scareSpite;
    
    
    SpriteRenderer grandmaRenderer;

    private void Awake() => grandmaRenderer = GetComponent<SpriteRenderer>();
    public void GetScared()
    {
        AudioManager.i.PlaySfx(AudioId.GrandmaScared);
        grandmaRenderer.sprite = scareSpite;

        GameController.Instance.ShowDialogThenFreeRoam(
            "Grandma: Aaaaaaaaaa, my heart !!",
            () =>
            {
                GameController.Instance.ShowRandomDialogThenFreeRoam(
                    new[]
                    {
                    "Grandma: Is it finally time?",
                    "Grandma: Oh grandpa is that you?",
                    "Grandma: I can finally leave this shitty village"
                    },
                    () =>
                    {
                        character.SetIsDead(true);
                        GameController.Instance.OnGrandmaScared();
                    }
                  );
            }
        );
  
    }

    public bool CanInteract()
    {
        if (character.HasFallen || character.IsDead)
            return false;
        return true;
    }
    public IEnumerator Interact(Transform initiator)
    {
        GameController.Instance.ShowRandomDialogThenFreeRoam(
            new[]
            {
                "Grandma: That dog's no good, I can tell.",
                "Grandma: Oh my heart :c",
                "Grandma: I'm not dead I'm dormant!",
                "Grandma: I hate ghosts.",
                "Grandma: I hate being scared, I might get a heart attack.",
            }
        );

        yield return null;
    }
}
