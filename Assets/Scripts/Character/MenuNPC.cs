using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNPC : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    float idleTimer = 0f;
    int currentPattern = 0;
    Character character;
    NPCState state;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (character.IsAbleToMove && state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }
    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.MoveWithoutConstraints(movementPattern[currentPattern]);


        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }
}
