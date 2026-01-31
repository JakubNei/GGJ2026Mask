using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class DoubleDoor : SignalReceiver
{
    public GameObject configLeftDoor;
    public GameObject configRightDoor;

    public float configMovementDistance = 1;
    public float configMovementSpeed = 0.3f;
    public MovementAxis configMovementAxis = MovementAxis.Horizontal;

    private Vector3 leftDoorOriginalPosition;
    private Vector3 rightDoorOriginalPosition;
    private Vector3 leftDoorVelocity = Vector3.zero;
    private Vector3 rightDoorVelocity = Vector3.zero;

    public enum MovementAxis
    {
        Horizontal,
        Vertical
    }
    

    public bool opening = false;

    void Start()
    {
        leftDoorOriginalPosition = configLeftDoor.transform.localPosition;
        rightDoorOriginalPosition = configRightDoor.transform.localPosition;
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
        if (opening)
        {
            Vector3 leftTargetPosition = leftDoorOriginalPosition + (configMovementAxis == MovementAxis.Horizontal ? new Vector3(configMovementDistance, 0, 0) : new Vector3(0, configMovementDistance, 0));
            Vector3 rightTargetPosition = rightDoorOriginalPosition + (configMovementAxis == MovementAxis.Horizontal ? new Vector3(-configMovementDistance, 0, 0) : new Vector3(0, -configMovementDistance, 0));
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
