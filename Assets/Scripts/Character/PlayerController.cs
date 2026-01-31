using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool CanPlayerPushObjects = false;
    public bool CanPlayerPullObjects = false;

    [SerializeField] ItemCursor itemCursor;
    [SerializeField] GameObject projectilePf;

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
        SwitchMask(MaskType.Ninja);
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

        var rigidBody = controllingCharacter.GetComponent<Rigidbody2D>();
        rigidBody.MovePosition(rigidBody.position + input.normalized * controllingCharacter.moveSpeed * Time.fixedDeltaTime);
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

        if(interactInput && !interactableGameObject && equippedMask == MaskType.Ninja) 
        {
            Projectile[] projectiles = FindObjectsOfType<Projectile>();
            if (projectiles.Length < 1)
            {
                var facingDir = new Vector3(controllingCharacter.Animator.MoveX, controllingCharacter.Animator.MoveY);
                GameObject projectile = Instantiate(projectilePf, controllingCharacter.transform.position, Quaternion.identity);
                projectile.GetComponent<Projectile>()?.Throw(interactFocusPos);
            }

        }

    }

    void SwitchMask(MaskType newMask)
    {
        if (equippedMask != newMask) 
        {
            equippedMask = newMask;
        }
    }

    public void HandleUpdate()
    {
        controllingCharacter.HandleUpdate();
    }

    public Character Character => controllingCharacter;
}
