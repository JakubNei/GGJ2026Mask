using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] Dialog dialog;

    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;

    Character character;
    IEnumerator walkInProgress;
    List<Vector2> movementPatternInWorldCoords = new(); // so movement can be interupted
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    void Start()
    {
        var point = new Vector2(transform.position.x, transform.position.y);
        foreach (var delta in movementPattern)
        {
            point += delta;
            movementPatternInWorldCoords.Add(point);
        }
    }

    private void Update()
    {
        if (DialogManager.Instance.isShowing) return;

        if (character.IsAbleToMove && state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                {
                    StopWalk();
                    walkInProgress = Walk();
                    StartCoroutine(walkInProgress);
                }
            }
        }
        character.HandleUpdate();
    }
    void StopWalk()
    {
        if (walkInProgress != null)
        {
            StopCoroutine(walkInProgress);
            walkInProgress = null;
        }
        state = NPCState.Idle;
        character.Animator.IsMoving = false;
    }
    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        var t = movementPatternInWorldCoords[currentPattern];
        t.x -= transform.position.x;
        t.y -= transform.position.y;
        yield return character.Move(t);


        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPatternInWorldCoords.Count;

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
        StopWalk();

        character.LookTowards(initiator.position);
        GameController.Instance.OnGenericNPCTalkedTo();

        yield return null;
    }

}

public enum NPCState { Idle, Walking, Dialog }