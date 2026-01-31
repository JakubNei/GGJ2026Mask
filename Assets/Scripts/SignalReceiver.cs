using UnityEngine;

public class SignalReceiver : MonoBehaviour
{
    public bool IsReceivingSignal => signalLastReceivedFrame >= Time.frameCount - 1;
    public int signalLastReceivedFrame = -1;
    
    public bool lastIsReceivingSignal = false;
    public void ReceiveSignal()
    {
        signalLastReceivedFrame = Time.frameCount;

        Update();
    }

    void Update()
    {
        if (IsReceivingSignal && !lastIsReceivingSignal)
            OnReceiveSignalOn();
        else if (!IsReceivingSignal && lastIsReceivingSignal)
            OnReceiveSignalOff();
        lastIsReceivingSignal = IsReceivingSignal;
    }
    public virtual void OnReceiveSignalOn()
    {

    }
    public virtual void OnReceiveSignalOff()
    {

    }
}

public static class SignalReceiverExtensions
{
    public static void ReceiveSignal(this SignalReceiver[] receivers)
    {
        foreach (var receiver in receivers)
        {
            receiver.ReceiveSignal();
        }
    }
}