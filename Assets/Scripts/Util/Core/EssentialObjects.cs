using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    public static EssentialObjects Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure SceneTransition exists
        if (GetComponentInChildren<SceneTransition>() == null)
        {
            var transitionGO = new GameObject("SceneTransition");
            transitionGO.transform.SetParent(transform);
            transitionGO.AddComponent<SceneTransition>();
        }
    }
}
