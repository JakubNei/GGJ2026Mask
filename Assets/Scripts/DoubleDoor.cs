using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class DoubleDoor : SignalReceiver
{
  
    public GameObject[] configFrames;
    public float configFrameDuration = 0.1f;

    private int currentFrameIndex = 0;
    private float frameTimer = 0f;

    public enum MovementAxis
    {
        Horizontal,
        Vertical
    }
    

    public bool opening = false;

    void Start()
    {
  
        // Initialize with first frame active
        if (configFrames != null && configFrames.Length > 0)
        {
            for (int i = 0; i < configFrames.Length; i++)
            {
                configFrames[i].SetActive(i == 0);
            }
        }
    }

    override public void OnReceiveSignalOn()
    {
        opening = true;
    }
    override public void OnReceiveSignalOff()
    {
        opening = false;
    }

    void Update()
    {
        if (configFrames == null || configFrames.Length == 0) return;

        frameTimer += Time.deltaTime;
        
        if (frameTimer >= configFrameDuration)
        {
            frameTimer = 0f;
            
            if (opening)
            {
                // Move forward through frames
                if (currentFrameIndex < configFrames.Length - 1)
                {
                    configFrames[currentFrameIndex].SetActive(false);
                    currentFrameIndex++;
                    configFrames[currentFrameIndex].SetActive(true);
                }
            }
            else
            {
                // Move backward through frames
                if (currentFrameIndex > 0)
                {
                    configFrames[currentFrameIndex].SetActive(false);
                    currentFrameIndex--;
                    configFrames[currentFrameIndex].SetActive(true);
                }
            }
        }
    }
}
