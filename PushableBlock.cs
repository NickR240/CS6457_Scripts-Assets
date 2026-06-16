using System.Collections;
using UnityEngine;

public class PushableBlock : MonoBehaviour
{
    [SerializeField] private float moveDistance = 1f; // How far block moves each push
    [SerializeField] private float moveSpeed = 2.5f; // How fast block slides
    [SerializeField] private string pushTriggerName = "Push"; // Animator trigger name for push

    [SerializeField] private LayerMask obstacleMask; // Layers checked for blocked tiles
    [SerializeField] private Vector3 tileCheckHalfExtents = new Vector3(0.9f, 0.9f, 0.9f); // Size of tile check box

    private Transform playerInRange; // Player currently able to push this block
    private Animator playerAnimator; // Player animator for push animation
    private bool isMoving; // True while block is sliding

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; // Ignore anything not player

        playerInRange = other.transform; // Save player in range
        playerAnimator = other.GetComponentInChildren<Animator>(); // Get player animator

        PlayerInteraction interaction = other.GetComponent<PlayerInteraction>(); // Get interaction script
        if (interaction != null)
            interaction.SetCurrentPushable(this); // Tell player this block can be pushed
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return; // Ignore anything not player

        PlayerInteraction interaction = other.GetComponent<PlayerInteraction>(); // Get interaction script
        if (interaction != null)
            interaction.ClearCurrentPushable(this); // Clear this block from player interaction

        if (playerInRange == other.transform)
        {
            playerInRange = null; // Clear saved player
            playerAnimator = null; // Clear saved animator
        }
    }

    public void TryPush(Transform player)
    {
        if (isMoving || playerInRange == null || player != playerInRange) return; // Stop if block moving or wrong player

        Vector3 moveDir = GetPushDirectionFromPlayerSide(); // Get move direction based on player side

        if (moveDir == Vector3.zero) return; // Stop if no valid direction

        Vector3 targetPos = transform.position + moveDir.normalized * moveDistance; // Position block wants to move to

        if (IsTileOccupied(targetPos))
            return; // Stop if another block is in the way

        Vector3 lookDir = transform.position - player.position; // Direction from player to block
        lookDir.y = 0f; // Ignore up and down

        if (lookDir != Vector3.zero)
            player.forward = lookDir.normalized; // Turn player toward block

        if (playerAnimator != null)
            playerAnimator.SetTrigger(pushTriggerName); // Play push animation

        StartCoroutine(MoveBlockOneStep(moveDir)); // Start moving block
    }

    private Vector3 GetPushDirectionFromPlayerSide()
    {
        Vector3 offset = playerInRange.position - transform.position; // Position difference between player and block
        offset.y = 0f; // Ignore up and down

        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.z))
            return offset.x > 0f ? Vector3.left : Vector3.right; // Push left or right
        else
            return offset.z > 0f ? Vector3.back : Vector3.forward; // Push back or forward
    }

    private bool IsTileOccupied(Vector3 targetPos)
    {
        Collider[] hits = Physics.OverlapBox(
            targetPos,
            tileCheckHalfExtents,
            Quaternion.identity,
            obstacleMask
        ); // Check for colliders in target tile

        foreach (Collider hit in hits)
        {
            PushableBlock otherBlock = hit.GetComponentInParent<PushableBlock>(); // See if collider belongs to a pushable block

            if (otherBlock == null)
                continue; // Ignore non-pushable objects

            if (otherBlock == this)
                continue; // Ignore this same block

            return true; // Another block is in target tile
        }

        return false; // Tile is free
    }

    private IEnumerator MoveBlockOneStep(Vector3 direction)
    {
        isMoving = true; // Mark block as moving

        Vector3 targetPos = transform.position + direction.normalized * moveDistance; // Final move position

        while (Vector3.Distance(transform.position, targetPos) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); // Slide block toward target
            yield return null; // Wait one frame
        }

        transform.position = targetPos; // Snap exactly to final position
        isMoving = false; // Mark block done moving
    }
}