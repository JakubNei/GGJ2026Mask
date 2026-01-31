using UnityEngine;

[RequireComponent(typeof(Character))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] float stopDistance = 2.4f;
    [SerializeField] float speedMultiplier = 0.5f;
    [SerializeField] float returnStopDistance = 0.1f;

    Character character;
    Rigidbody2D rb;
    Vector2 moveDirection;
    Vector3 initialPosition;

    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        character.CurrentMask = MaskType.Tough;
    }

    void Update()
    {
        var player = PlayerController.Instance?.controllingCharacter;
        if (player == null)
        {
            moveDirection = Vector2.zero;
            character.HandleUpdate();
            return;
        }

        // Only chase when player has Default mask equipped
        bool playerHasDefaultMask = InventoryItems.Instance?.EquippedItem?.MaskType == MaskType.Default;

        if (playerHasDefaultMask)
        {
            // Chase player
            Vector3 toPlayer = player.transform.position - transform.position;

            if (toPlayer.magnitude < stopDistance)
            {
                moveDirection = Vector2.zero;
            }
            else
            {
                moveDirection = toPlayer.normalized;
            }
        }
        else
        {
            // Return to initial position
            Vector3 toHome = initialPosition - transform.position;

            if (toHome.magnitude < returnStopDistance)
            {
                moveDirection = Vector2.zero;
            }
            else
            {
                moveDirection = toHome.normalized;
            }
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
