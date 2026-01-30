using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] ItemCursor itemCursor;
    [SerializeField] public InventoryItems inventoryItems;

    public static PlayerController Instance { get; private set; }
    public CharacterAnimator characterAnimator;
    private Vector2 input;
    private Character character;
    public bool connectCamera = true;

    [HideInInspector] public bool isInteractingWithGuard;

    Vector3 lastMousePosition;

    private void Awake()
    {
        Instance = this;
        character = GetComponent<Character>();
    }

    enum InteractPosMethod
    {
        CharacterFacing,
        TowardsMouse,
    }
    InteractPosMethod interactPosMethod;

    public void OnStartBeingEatenByDog()
    {
        connectCamera = false;
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


    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        var rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(rb.position + input.normalized * character.moveSpeed * Time.deltaTime);


        var pos = Camera.main.transform.position;
        pos.x = transform.position.x;
        pos.y = transform.position.y;
        Camera.main.transform.position = pos;


        if (lastMousePosition != Input.mousePosition)
        {
            interactPosMethod = InteractPosMethod.TowardsMouse;
        }
        lastMousePosition = Input.mousePosition;

        Vector3 interactFocusPos = transform.position;
        if (interactPosMethod == InteractPosMethod.CharacterFacing)
        {
            var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
            interactFocusPos = transform.position + facingDir * (inventoryItems.selectedItem ? 1f : 0.4f);
        }
        else if (interactPosMethod == InteractPosMethod.TowardsMouse)
        {
            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var facingDir = mouseWorldPosition - character.transform.position;
            facingDir.z = 0;
            var m = facingDir.magnitude;
            var mc = Mathf.Clamp(m, 0, 1);
            facingDir = facingDir / m * mc;
            interactFocusPos = transform.position + facingDir;
        }

        GameObject interactableGameObject = null;
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(character.transform.position, 1.0f, GameLayers.i.InteractableLayer | GameLayers.i.WaterLayer);
            float bestWeight = float.MaxValue;
            Collider2D bestCandidate = null;
            foreach (var collider in colliders)
            {
                var interactible = collider?.gameObject?.GetComponent<IInteractable>();
                if (interactible == null || !interactible.CanInteract())
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

        itemCursor.transform.position = interactFocusPos;
        if (inventoryItems.selectedItem)
        {
            HighlightSprite.Highlight(itemCursor.itemPreview);
        }
        else if (interactableGameObject)
        {
            HighlightSprite.Highlight(interactableGameObject);
        }

        bool interactInput = IsInteractInputKey();
        if (interactInput && interactableGameObject)
        {
            var interactible = interactableGameObject.GetComponent<IInteractable>();
            if (interactible != null)
            {
                interactible.UpdateWhileInteracting();
            }
        }

    }

    public void HandleUpdate()
    {
        character.HandleUpdate();
    }


    void PlaceOrInteractSelectedItem(Vector2 placeAtPosition)
    {
        ItemBase item = inventoryItems.selectedItem;
        if (item.InteractInsteadOfPlace)
        {
            if (item.Interact(transform.position))
            {
                inventoryItems.RemoveItem(inventoryItems.selectedItem);
                inventoryItems.selectedItem = null;
                itemCursor.itemPreview.sprite = null;
            }
        }
        else
        {
            //Instantiate(inventoryItems.selectedItem, position, Quaternion.identity);
            inventoryItems.selectedItem.transform.position = placeAtPosition;
            inventoryItems.selectedItem.transform.rotation = Quaternion.identity;

            inventoryItems.RemoveItem(inventoryItems.selectedItem);
            inventoryItems.selectedItem = null;
            itemCursor.itemPreview.sprite = null;
        }
    }

    public Character Character => character;
}
