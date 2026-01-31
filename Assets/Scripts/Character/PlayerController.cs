using UnityEngine;

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

    float lastShootTime;

    ItemBase defaultMask;


    public InventoryItems inventoryItems;
   

    public static PlayerController Instance { get; private set; }

    public Vector2 input;
    public Character controllingCharacter;

    public Vector3 lastMousePosition;

    public Vector3 lastCharacterPosition;
    public Vector3 lastCharacterDeltaMovement;

    MaskType equippedMask = MaskType.Default;

    private void Awake()
    {
        Instance = this;
        defaultMask = Instantiate(defaultMaskPf, transform.position, Quaternion.identity).GetComponent<ItemBase>();
        inventoryItems.AddItem(defaultMask);
        SwitchMask(MaskType.Default);
        inventoryItems.EquipItem(defaultMask);
    }

    enum InteractPosMethod
    {
        CharacterFacing,
        TowardsMouse,
    }
    InteractPosMethod interactPosMethod;

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

        var rigidBody = controllingCharacter.GetComponent<Rigidbody2D>();
        rigidBody.MovePosition(rigidBody.position + currentVelocity * controllingCharacter.moveSpeed * Time.fixedDeltaTime);
    }


    void Update()
    {
        if (controllingCharacter == null)
        {
            foreach (var character in FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (character.IsPlayerCharacter)
                {
                    controllingCharacter = character;
                    break;
                }
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
        Vector3 interactFocusPos = controllingCharacter.transform.position;
        {
            if (lastMousePosition != Input.mousePosition)
            {
                interactPosMethod = InteractPosMethod.TowardsMouse;
            }
            lastMousePosition = Input.mousePosition;
            if (interactPosMethod == InteractPosMethod.CharacterFacing)
            {
                var facingDir = new Vector3(controllingCharacter.Animator.MoveX, controllingCharacter.Animator.MoveY);
                interactFocusPos = controllingCharacter.transform.position + facingDir * (inventoryItems.selectedItem ? 1f : 0.4f);
            }
            else if (interactPosMethod == InteractPosMethod.TowardsMouse)
            {
                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var facingDir = mouseWorldPosition - controllingCharacter.transform.position;
                facingDir.z = 0;
                var m = facingDir.magnitude;
                var mc = Mathf.Clamp(m, 0, 1);
                facingDir = facingDir / m * mc;
                interactFocusPos = controllingCharacter.transform.position + facingDir;
            }
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
            HighlightSprite.Highlight(interactableGameObject);
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
        if(interactInput && !interactableGameObject /* && equippedMask == MaskType.Ninja */) 
        {
            if (Time.time >= lastShootTime + shootCooldown)
            {
                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var throwDirection = (mouseWorldPosition - controllingCharacter.transform.position);
                throwDirection.z = 0;
                throwDirection.Normalize();

                var spawnPos = controllingCharacter.transform.position + new Vector3(0, -0.35f, 0);
                GameObject projectile = Instantiate(projectilePf, spawnPos, Quaternion.identity);
                projectile.transform.localScale *= 0.33f;
                projectile.GetComponent<Projectile>()?.Throw(throwDirection);
                lastShootTime = Time.time;
            }
        }

        //MASK
        if(inventoryItems.EquippedItem.MaskType != equippedMask)
        {
            SwitchMask(inventoryItems.EquippedItem.MaskType);
        }

        // Bob and wobble is now handled in Character.HandleUpdate()
        controllingCharacter.HandleUpdate();
    }

    void SwitchMask(MaskType newMask)
    {
        if (equippedMask != newMask) 
        {
            equippedMask = newMask;
            controllingCharacter.switchMask(equippedMask);
        }
    }

    public void HandleUpdate()
    {
        controllingCharacter.HandleUpdate();
    }

    public Character Character => controllingCharacter;
}
