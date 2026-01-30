using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCFireController : MonoBehaviour
{
    [SerializeField] Vector2 movement;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public IEnumerator WalkOverAndFall()
    {
        yield return character.Move(movement);

        character.SetHasFallen(true);

        character.HandleUpdate();
    }

}