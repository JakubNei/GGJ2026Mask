using UnityEngine;

[RequireComponent(typeof(Character))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] float stopDistance = 2.4f;
    [SerializeField] float speedMultiplier = 0.5f;

    Character character;
    Rigidbody2D rb;
    Vector2 moveDirection;

    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        character.switchMask(MaskType.Default);
    }

    void Update()
    {
        var player = PlayerController.Instance?.controllingCharacter;
        if (player == null)
        {
            moveDirection = Vector2.zero;
            return;
        }

        Vector3 toPlayer = player.transform.position - transform.position;

        if (toPlayer.magnitude < stopDistance)
        {
            moveDirection = Vector2.zero;
        }
        else
        {
            moveDirection = toPlayer.normalized;
        }

        character.HandleUpdate();
    }

    void FixedUpdate()
    {
        if (moveDirection != Vector2.zero)
        {
            rb.MovePosition(rb.position + moveDirection * character.moveSpeed * speedMultiplier * Time.fixedDeltaTime);
        }
    }
}
