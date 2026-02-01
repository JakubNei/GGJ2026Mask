using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool CanPlayerPushObjects = false;
    public bool CanPlayerPullObjects = false;

    [SerializeField] ItemCursor itemCursor;
    [SerializeField] GameObject projectilePf;
    [SerializeField] GameObject defaultMaskPf;
    [SerializeField] float shootCooldown = 0.3f;

    [Header("Movement")]
    [SerializeField] float accelerationTime = 0.08f;
    Vector2 currentVelocity;
    Vector2 smoothVelocityRef;

    // External push (from enemies, etc.)
    Vector2 pushVelocity;
    [SerializeField] float pushDecay = 10f;

    float lastShootTime;

    ItemBase defaultMask;


    public InventoryItems inventoryItems;


    public static PlayerController Instance { get; private set; }

    public Vector2 input;
    public Character controllingCharacter;

    public Vector3 lastMousePosition;
    public float lastMouseMovedTime;

    public Vector3 lastCharacterPosition;
    public Vector3 lastCharacterDeltaMovement;
    public Vector3 lastMostMovedDir;

    public Vector3 interactFocusPos;
    MaskType MaskSelectedInUI => inventoryItems.EquippedItem ? inventoryItems.EquippedItem.MaskType : MaskType.None;

    MaskType MaskOnCharacter
    {
        get
        {
            return controllingCharacter == null ? MaskType.None : controllingCharacter.CurrentMask;
        }
        set
        {
            if (controllingCharacter != null)
            {
                controllingCharacter.CurrentMask = value;
            }
        }
    }
    private void Awake()
    {
        Instance = this;
        defaultMask = Instantiate(defaultMaskPf, transform.position, Quaternion.identity).GetComponent<ItemBase>();
        inventoryItems.AddItem(defaultMask);
        MaskOnCharacter = MaskType.Default;
        inventoryItems.EquipItem(defaultMask);
    }

    public static bool OpenMenuKeyDown()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Backspace);
    }

    public static bool IsInteractInputKey()
    {
        return
            Input.GetKey(KeyCode.Q) ||
            Input.GetKey(KeyCode.E) ||
            Input.GetKey(KeyCode.LeftControl) ||
            Input.GetKey(KeyCode.Space) ||
            Input.GetKey(KeyCode.Return) ||
            Input.GetKey(KeyCode.Mouse0);
    }


    void FixedUpdate()
    {
        if (controllingCharacter == null)
            return;

        // Smooth acceleration using critically-damped spring (SmoothDamp)
        Vector2 targetVelocity = input.normalized;
        currentVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref smoothVelocityRef, accelerationTime);

        // Combine player input with external push
        Vector2 totalVelocity = currentVelocity * controllingCharacter.moveSpeed + pushVelocity;

        var rigidBody = controllingCharacter.GetComponent<Rigidbody2D>();
        rigidBody.MovePosition(rigidBody.position + totalVelocity * Time.fixedDeltaTime);

        // Track the most dominant movement direction
        if (totalVelocity.sqrMagnitude > 0.01f)
        {
            lastMouseMovedTime = 0;
            if (Mathf.Abs(totalVelocity.x) <= Mathf.Abs(totalVelocity.y))
                lastMostMovedDir = totalVelocity.y > 0 ? Vector3.up : Vector3.down;
            else
                lastMostMovedDir = totalVelocity.x > 0 ? Vector3.right : Vector3.left;
        }

        // Decay push velocity over time
        pushVelocity = Vector2.MoveTowards(pushVelocity, Vector2.zero, pushDecay * Time.fixedDeltaTime);
    }

    public void ApplyPush(Vector2 force)
    {
        pushVelocity += force;
    }


    void Update()
    {
        if (controllingCharacter == null)
        {
            List<GameObject> resultslist = new List<GameObject>();
            GameObject.FindGameObjectsWithTag("Player", resultslist);
            foreach (var c in resultslist)
            {
                var character = c.GetComponent<Character>();
                if (!character)
                    continue;
                if (!character.IsPlayerCharacter)
                    continue;
                controllingCharacter = character;
                MaskOnCharacter = MaskSelectedInUI;
                break;
            }
            lastCharacterPosition = controllingCharacter.transform.position;
        }

        if (controllingCharacter == null)
            return;

        var p = controllingCharacter.transform.position;
        lastCharacterDeltaMovement = p - lastCharacterPosition;
        lastCharacterPosition = p;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");


        var cameraPos = Camera.main.transform.position;
        cameraPos.x = controllingCharacter.transform.position.x;
        cameraPos.y = controllingCharacter.transform.position.y;
        Camera.main.transform.position = cameraPos;

        // Face towards mouse cursor (left/right) - flip sprite and mask
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float playerX = controllingCharacter.transform.position.x;
        bool shouldFlip = mouseWorldPos.x < playerX;

        // Flip player sprite
        var sr = controllingCharacter.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = shouldFlip;
        }

        // Flip mask holder
        var maskDisplay = controllingCharacter.MaskDisplay;
        if (maskDisplay != null)
        {
            Vector3 maskScale = maskDisplay.transform.localScale;
            maskScale.x = shouldFlip ? -Mathf.Abs(maskScale.x) : Mathf.Abs(maskScale.x);
            maskDisplay.transform.localScale = maskScale;
        }

        // Aim interaction with either mouse or movement
        Vector3 interactOriginPos = controllingCharacter.transform.position + Vector3.up * 0.3f;
        interactFocusPos = interactOriginPos + lastMostMovedDir;
        {
            if (lastMousePosition != Input.mousePosition ||
                Input.GetKeyDown(KeyCode.Mouse0) ||
                Input.GetKeyDown(KeyCode.Mouse1) ||
                Input.GetKeyDown(KeyCode.Mouse2)
            )
                lastMouseMovedTime = Time.time;
            lastMousePosition = Input.mousePosition;
            if (lastMouseMovedTime > Time.time - 1)
            {
                interactFocusPos = Camera.main.ScreenToWorldPoint(lastMousePosition);
            }

            // Clamp interact focus pos to be within 0.5 units of player
            var facing = interactFocusPos - interactOriginPos;
            facing.z = 0;
            var m = facing.magnitude;
            var mc = Mathf.Clamp(m, 0, 0.1f);
            facing = facing / m * mc;
            interactFocusPos = interactOriginPos + facing;
        }

        GameObject interactableGameObject = null;
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(interactFocusPos, 1.0f);
            float bestWeight = float.MaxValue;
            Collider2D bestCandidate = null;
            foreach (var collider in colliders)
            {
                var interactible = collider?.gameObject?.GetComponent<IInteractable>();
                if (interactible == null)
                    continue;
                if (!interactible.CanInteract() && !interactible.CanPickUp())
                    continue;
                float weight = Vector3.Distance(collider.transform.position, interactFocusPos);
                if (weight < bestWeight)
                {
                    bestCandidate = collider;
                    bestWeight = weight;
                }
            }
            interactableGameObject = bestCandidate?.gameObject;
        }

        /*itemCursor.transform.position = interactFocusPos;
        if (inventoryItems.selectedItem)
        {
            HighlightSprite.Highlight(itemCursor.itemPreview);
        }
        else */
        if (interactableGameObject)
        {
            HighlightSprite.Outline(interactableGameObject, Color.white);
        }

        bool interactInput = IsInteractInputKey();
        if (interactInput && interactableGameObject)
        {
            interactableGameObject.GetComponent<IInteractable>()?.UpdateWhileInteracting();
            if (interactableGameObject.GetComponent<IInteractable>().CanPickUp())
            {
                ItemBase item = interactableGameObject.GetComponent<ItemBase>();
                if (item)
                {
                    Debug.Log("Picking up " + item.name);
                    inventoryItems.AddItem(item);
                }
            }
        }

        //PROJECTILE
        if (interactInput && !interactableGameObject && MaskOnCharacter == MaskType.Ninja)
        {
            if (Time.time >= lastShootTime + shootCooldown)
            {
                var spawnPos = interactOriginPos;
                var throwDirection = interactFocusPos - spawnPos;
                throwDirection.z = 0;
                throwDirection.Normalize();

                GameObject projectile = Instantiate(projectilePf, spawnPos, Quaternion.identity);
                projectile.transform.localScale *= 0.65f;
                projectile.GetComponent<Projectile>()?.Throw(throwDirection);
                lastShootTime = Time.time;
            }
        }

        //MASK
        if (MaskOnCharacter != MaskSelectedInUI)
        {
            MaskOnCharacter = MaskSelectedInUI;
        }

        // Bob and wobble is now handled in Character.HandleUpdate()
        controllingCharacter.HandleUpdate();
    }


    public void HandleUpdate()
    {
        controllingCharacter.HandleUpdate();
    }

    public Character Character => controllingCharacter;

}