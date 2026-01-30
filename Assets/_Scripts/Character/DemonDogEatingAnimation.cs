using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonDogEatingAnimation : MonoBehaviour
{
    [SerializeField] GameObject originalSprite;
    [SerializeField] Transform animationCharacterRoot;

    public GameObject eatTarget;

    bool shouldUpdateTargetTransform = false;

    public void EatTarget(GameObject eatTarget)
    {        
        this.eatTarget = eatTarget;
        eatTarget.GetComponentInChildren<PlayerController>()?.OnStartBeingEatenByDog();
        gameObject.SetActive(true);
        originalSprite.SetActive(false);
    }
 

    public void OnStartUpdatingCharacterTransform()
    {
        shouldUpdateTargetTransform = true;
    }

    void Update()
    {
        if (eatTarget)
        {
            eatTarget.transform.position = animationCharacterRoot.position;
            eatTarget.transform.rotation = animationCharacterRoot.rotation;
            eatTarget.transform.localScale = animationCharacterRoot.localScale;
        }
    }

    public void OnFinished()
    {
        shouldUpdateTargetTransform = false;
        gameObject.SetActive(false);
        originalSprite.SetActive(true);
        eatTarget.GetComponentInChildren<Character>()?.OnEatenByDog();
        eatTarget = null;
    }
}
