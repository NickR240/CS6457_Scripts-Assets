using System.Collections;
using UnityEngine;

public class DoorMover : MonoBehaviour
{
    [SerializeField] private float moveUpDistance = 3f; // How far door moves up
    [SerializeField] private float moveSpeed = 2f; // How fast door moves

    private Vector3 closedPosition; // Starting closed position
    private Vector3 openPosition; // Open position above closed position
    private Coroutine moveRoutine; // Stores current move coroutine

    private void Awake()
    {
        closedPosition = transform.position; // Save starting position as closed
        openPosition = closedPosition + Vector3.up * moveUpDistance; // Set open position above it
    }

    public void OpenDoor()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine); // Stop current movement if already moving

        moveRoutine = StartCoroutine(MoveDoor(openPosition)); // Start moving door open
    }

    public void CloseDoor()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine); // Stop current movement if already moving

        moveRoutine = StartCoroutine(MoveDoor(closedPosition)); // Start moving door closed
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            ); // Move door toward target position

            yield return null; // Wait until next frame
        }

        transform.position = targetPosition; // Snap exactly to target
        moveRoutine = null; // Clear coroutine when done
    }
}
