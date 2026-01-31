using UnityEngine;

public class SignalReceiver : MonoBehaviour
{
    public bool IsReceivingSignal => signalLastReceivedFrame == Time.frameCount;
    int signalLastReceivedFrame = -1;
    public void ReceiveSignal()
    {
        signalLastReceivedFrame = Time.frameCount;
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