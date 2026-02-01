using UnityEngine;

[RequireComponent(typeof(Character))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] float stopDistance = 2.4f;
    [SerializeField] float speedMultiplier = 0.5f;
    [SerializeField] float returnStopDistance = 0.1f;

    [Header("Push Player")]
    [SerializeField] float pushForce = 8f;
    [SerializeField] float pushRadius = 0.5f;

    Character character;
    Rigidbody2D rb;
    Vector2 moveDirection;
    Vector3 initialPosition;

    // Enemy push velocity (same system as player)
    Vector2 pushVelocity;
    [SerializeField] float pushDecay = 10f;


    [System.Serializable]
    public class WalkTarget
    {
        public Transform target;
        public float loiterTime;
    }
    // Cleaned when reached
    [Header("Temporarily Walk To Target")]
    [SerializeField] WalkTarget walkTarget;

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
        if (character.temporarilyForbidMovement)
            return;

        // Only chase when player has Default mask equipped
        bool playerHasDefaultMask = InventoryItems.Instance?.EquippedItem?.MaskType == MaskType.Default;

        if (walkTarget != null && walkTarget.target != null)
        {
            Vector3 toTarget = walkTarget.target.position - transform.position;
            if (toTarget.magnitude < stopDistance)
            {
                walkTarget.loiterTime -= Time.deltaTime;
                if (walkTarget.loiterTime <= 0f)
                {
                    // Reached target
                    walkTarget.target = null;
                    walkTarget = null;
                }
                moveDirection = Vector2.zero;
            }
            else
            {
                moveDirection = toTarget.normalized;
            }
        }
        else if (playerHasDefaultMask)
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
        // Combine normal movement with push velocity
        Vector2 totalVelocity = pushVelocity;
        if (moveDirection != Vector2.zero)
        {
            totalVelocity += moveDirection * character.moveSpeed * speedMultiplier;
        }

        if (totalVelocity != Vector2.zero)
        {
            rb.MovePosition(rb.position + totalVelocity * Time.fixedDeltaTime);
        }

        // Decay push velocity over time
        pushVelocity = Vector2.MoveTowards(pushVelocity, Vector2.zero, pushDecay * Time.fixedDeltaTime);

        // Push player away if too close (and enemy gets pushed back too)
        if (PlayerController.Instance != null)
        {
            var player = PlayerController.Instance.controllingCharacter;
            if (player != null)
            {
                Vector3 toPlayer = player.transform.position - transform.position;
                float dist = toPlayer.magnitude;

                if (dist < pushRadius)
                {
                    Vector2 pushDirection = ((Vector2)toPlayer).normalized;
                    // Push player away
                    PlayerController.Instance.ApplyPush(pushDirection * pushForce);
                    // Push enemy in opposite direction
                    pushVelocity += -pushDirection * pushForce;
                }
            }
        }
    }
}
