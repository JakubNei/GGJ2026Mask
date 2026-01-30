using System.Collections;
using UnityEngine;

public class CameraMenu : MonoBehaviour
{
    [SerializeField] Vector2 corner1;
    [SerializeField] Vector2 corner2;
    [SerializeField] Vector2 corner3;
    [SerializeField] Vector2 corner4;

    public float moveSpeed = 10f;
    float startMovement = 0f;

    void Update()
    {
        startMovement += Time.deltaTime;
        if (startMovement > 1.5f)
        {
            startMovement = 0f;

            StartCoroutine(MoveInSquare());
        }
    }

    IEnumerator MoveInSquare()
    {
        Vector2[] corners = new Vector2[] { corner1, corner2, corner3, corner4 };

        int currentIndex = 0;

        while (true)
        {
            Vector3 targetPosition = new Vector3(corners[currentIndex].x, corners[currentIndex].y, transform.position.z);

            yield return MoveTo(targetPosition);

            currentIndex = (currentIndex + 1) % corners.Length;

            if (currentIndex == 4)
                currentIndex = 0;
        }
    }

    IEnumerator MoveTo(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        yield return null;
    }
}
