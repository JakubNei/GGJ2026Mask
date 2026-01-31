using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

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

    [Header("Mask Bob")]
    [SerializeField] float maskBobFrequency = 12f;
    [SerializeField] float maskBobAmplitude = 0.08f;
    [SerializeField] float maskBobDelay = 0.05f; // Delay in seconds for mask to follow
    Transform maskTransform;
    Vector3 maskBasePosition;
    float maskBobTimer;
    float delayedBobOffset; // Smoothed/delayed bob value

    [Header("Walk Wobble")]
    [SerializeField] float wobbleFrequency = 24f;
    [SerializeField] float wobbleAmplitude = 36f; // degrees
    [SerializeField] float bodyBobAmplitude = 0.05f; // vertical bob
    [SerializeField] float bodyBobPhaseOffset = 1.5f; // offset from wobble (radians)
    Transform spriteTransform;
    Vector3 spriteBasePosition;
    float wobbleTimer;

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

            // Find mask container for bobbing
            var maskDisplay = controllingCharacter.MaskDisplay;
            if (maskDisplay != null)
            {
                maskTransform = maskDisplay.transform;
                maskBasePosition = maskTransform.localPosition;
            }

            // Find sprite for wobble (or use character transform)
            var spriteRenderer = controllingCharacter.GetComponentInChildren<SpriteRenderer>();
            spriteTransform = spriteRenderer != null ? spriteRenderer.transform : controllingCharacter.transform;
            spriteBasePosition = spriteTransform.localPosition;
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

        // Mask bob with delay (uses smoothed velocity so bob matches actual movement)
        if (maskTransform != null)
        {
            if (currentVelocity.magnitude > 0.1f)
            {
                maskBobTimer += Time.deltaTime * maskBobFrequency;
                float targetBobOffset = Mathf.Sin(maskBobTimer) * maskBobAmplitude;
                // Smooth delay effect - mask follows with slight lag
                float delaySpeed = maskBobDelay > 0 ? 1f / maskBobDelay : 100f;
                delayedBobOffset = Mathf.Lerp(delayedBobOffset, targetBobOffset, Time.deltaTime * delaySpeed);
                maskTransform.localPosition = maskBasePosition + new Vector3(0, delayedBobOffset, 0);
            }
            else
            {
                // Smoothly return to rest
                delayedBobOffset = Mathf.Lerp(delayedBobOffset, 0, Time.deltaTime * 10f);
                maskTransform.localPosition = Vector3.Lerp(maskTransform.localPosition, maskBasePosition, Time.deltaTime * 10f);
                maskBobTimer = 0;
            }
        }

        // Walk wobble - sway rotation + vertical bob like holding a figurine
        if (spriteTransform != null)
        {
            if (currentVelocity.magnitude > 0.1f)
            {
                wobbleTimer += Time.deltaTime * wobbleFrequency;
                // Rotation wobble
                float wobbleAngle = Mathf.Sin(wobbleTimer) * wobbleAmplitude;
                spriteTransform.localRotation = Quaternion.Euler(0, 0, wobbleAngle);
                // Vertical bob (with phase offset)
                float bobOffset = Mathf.Sin(wobbleTimer + bodyBobPhaseOffset) * bodyBobAmplitude;
                spriteTransform.localPosition = spriteBasePosition + new Vector3(0, bobOffset, 0);
            }
            else
            {
                // Smoothly return to upright and base position
                spriteTransform.localRotation = Quaternion.Slerp(spriteTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
                spriteTransform.localPosition = Vector3.Lerp(spriteTransform.localPosition, spriteBasePosition, Time.deltaTime * 10f);
                wobbleTimer = 0;
            }
        }

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
