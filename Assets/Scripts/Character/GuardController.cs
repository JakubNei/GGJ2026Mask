using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuardController : MonoBehaviour
{

    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    [SerializeField] AudioClip playerFoundClip;
    [SerializeField] AudioClip punchSfx;

    [SerializeField] Dialog dialog;
    [SerializeField] string playerFoundDialog;
    //[SerializeField] List<string> badJokes;
    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    Character character;
    bool canTriggerPlayer = true;

    private void Awake() => character = GetComponent<Character>();

    private void Update()
    {
        if (!canTriggerPlayer)
        {
            StartCoroutine(CooldownTimer());
        }

        SetFovRotation(character.Animator.GetCurrentFacingDirection());

        if (character.IsAbleToMove && state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
            }
        }
        character.HandleUpdate();
    }

    IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(3f);
        canTriggerPlayer = true;
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);

        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }

    public bool CanInteract()
    {
        if (character.HasFallen || character.IsDead)
            return false;
        return true;
    }
    public IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);
        yield return DialogManager.Instance.ShowDialog(dialog);
    }

    public IEnumerator TriggerKnockout(PlayerController player)
    {
        if (!canTriggerPlayer)
            yield break;
        if (!character.IsAbleToMove)
            yield break;
        if (DialogManager.Instance.isShowing)
            yield return new WaitForEndOfFrame();

        var originalPos = transform.position;

        player.isInteractingWithGuard = true;
        GameController.Instance.TriggerKnockout();
        AudioManager.i.PlaySfx(playerFoundClip);

        yield return player.Character.Animator.IsMoving = false;

        // Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        exclamation.SetActive(false);

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        yield return character.Move(moveVec);

        // Show dialog
        yield return TellRandomJoke();

        // Make player fall 
        AudioManager.i.PlaySfx(punchSfx);
        player.Character.SetHasFallen(true);

        // Make him drop items
        player.inventoryItems.DropAllItemsAndPutThemOnANewPlace(player);

        // Free roam
        GameController.Instance.gameState = GameState.FreeRoam;
        player.isInteractingWithGuard = false;

        //Walk back after knocking out player
        var diffToOriginalPos = originalPos - transform.position;
        yield return character.Move(diffToOriginalPos, () => character.SetCurrentFacingDirection(FacingDirection.Right));
        canTriggerPlayer = false;
    }

    IEnumerator TellRandomJoke()
    {
        //string randomJoke = badJokes[Random.Range(0, badJokes.Count)];
        yield return DialogManager.Instance.ShowDialogText($"{character.Name}: {playerFoundDialog}");
        //yield return DialogManager.Instance.ShowDialogText($"Jester: {randomJoke}");
        yield return GameController.Instance.TellPlayerSelectedBadJoke();
        yield return DialogManager.Instance.ShowDialogText($"{character.Name}: Oh, shut up.");
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}