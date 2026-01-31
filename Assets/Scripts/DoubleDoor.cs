using UnityEngine;

public class DoubleDoor : SignalReceiver
{
    public GameObject configLeftDoor;
    public GameObject configRightDoor;

    public float configMovementDistance = 1;
    public float configMovementSpeed = 0.3f;

    private Vector3 leftDoorOriginalPosition;
    private Vector3 rightDoorOriginalPosition;
    private Vector3 leftDoorVelocity = Vector3.zero;
    private Vector3 rightDoorVelocity = Vector3.zero;

    void Start()
    {
        leftDoorOriginalPosition = configLeftDoor.transform.localPosition;
        rightDoorOriginalPosition = configRightDoor.transform.localPosition;
    }

    void Update()
    {
        if (IsReceivingSignal)
        {
            Vector3 leftTargetPosition = leftDoorOriginalPosition + new Vector3(configMovementDistance, 0, 0);
            Vector3 rightTargetPosition = rightDoorOriginalPosition + new Vector3(-configMovementDistance, 0, 0);
            configLeftDoor.transform.localPosition = Vector3.SmoothDamp(configLeftDoor.transform.localPosition, leftTargetPosition, ref leftDoorVelocity, configMovementSpeed);
            configRightDoor.transform.localPosition = Vector3.SmoothDamp(configRightDoor.transform.localPosition, rightTargetPosition, ref rightDoorVelocity, configMovementSpeed);
        }
        else
        {
            configLeftDoor.transform.localPosition = Vector3.SmoothDamp(configLeftDoor.transform.localPosition, leftDoorOriginalPosition, ref leftDoorVelocity, configMovementSpeed);
            configRightDoor.transform.localPosition = Vector3.SmoothDamp(configRightDoor.transform.localPosition, rightDoorOriginalPosition, ref rightDoorVelocity, configMovementSpeed);
        }
    }
}
