using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
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

    public static bool IsInteractInputKeyDown()
    {
        return
            Input.GetKeyDown(KeyCode.Q) ||
            Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.LeftControl) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Mouse0);
    }


    public void HandleUpdate()
    {
        if (!character.IsMoving && character.IsAbleToMove)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //odstranuje diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                interactPosMethod = InteractPosMethod.CharacterFacing;
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

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
                var interactible = collider?.gameObject?.GetComponent<Interactable>();
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

        

        // if (interactableGameObject != null || inventoryItems.selectedItem)
        //     itemCursor.cursor.color = new Color(1, 1, 1, 0.9f);
        // else
        //     itemCursor.cursor.color = new Color(1, 1, 1, 0.5f);

        bool interactInput = IsInteractInputKeyDown();
        if (interactInput)
        {
            if (inventoryItems.selectedItem)
            {
                PlaceOrInteractSelectedItem(itemCursor.transform.position);
            }
            else if (interactableGameObject)
            {
                character.LookTowards(interactableGameObject.transform.position);
                StartCoroutine(Interact(interactableGameObject.GetComponent<Interactable>()));
            }
        }

        if (inventoryItems.selectedItem)
        {
            itemCursor.itemPreview.color = inventoryItems.selectedItem.GetIconColor() * new Color(1, 1, 1, 0.6f);
            itemCursor.itemPreview.sprite = inventoryItems.selectedItem.GetIcon();
        }

        if (inventoryItems.hasDropped)
        {
            itemCursor.itemPreview.sprite = null;
            inventoryItems.hasDropped = false;
        }
    }

    IEnumerator Interact(Interactable interactlabe)
    {
        yield return character.Animator.IsMoving = false;
        yield return interactlabe?.Interact(transform);
    }

    IPlayerTriggerable currentlyInTrigger;

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);

        IPlayerTriggerable triggerable = null;
        foreach (var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerReapeatedly)
                    break;

                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }

        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
            currentlyInTrigger = null;
    }

    public object CaptureState() // btw tohle muze reprezentovat jakykoliv typ dat klidne bool atd.
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        // Restore position 
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);
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

    public string Name
    {
        get => name;
    }


    public Character Character => character;
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
}
