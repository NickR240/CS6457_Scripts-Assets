using UnityEngine;
using System.Collections;

public class Approach : MonoBehaviour
{
    public float targetZ = 50f;
    public float moveSpeed = 5f;

    private bool isMoving = false;
    private bool hasMoved = false;

    public void MoveForward()
    {
        if (!isMoving)
            StartCoroutine(MoveRoutine());
        hasMoved = true;
    }

    IEnumerator MoveRoutine()
    {
        isMoving = true;

        Vector3 targetPosition = new Vector3(
            transform.position.x,
            transform.position.y,
            targetZ
        );

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}
