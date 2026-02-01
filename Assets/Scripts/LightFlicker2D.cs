using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker2D : MonoBehaviour
{
    private Light2D _light;
    
    [Header("Settings")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    
    [Tooltip("How fast the flicker happens")]
    public float flickerSpeed = 0.1f;

    private float _targetIntensity;
    private float _lastChangeTime;

    void Start()
    {
        _light = GetComponent<Light2D>();
    }

    void Update()
    {
        // Smoothly transition to the target intensity
        _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, Time.deltaTime * (1f / flickerSpeed));

        // If enough time has passed, pick a new random intensity
        if (Time.time - _lastChangeTime > flickerSpeed)
        {
            _targetIntensity = Random.Range(minIntensity, maxIntensity);
            _lastChangeTime = Time.time;
        }
    }
}