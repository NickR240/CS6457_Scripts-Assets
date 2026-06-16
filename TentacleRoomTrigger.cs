using UnityEngine;
public class TentacleRoomTrigger : MonoBehaviour
{
    // ref to tentacle script
    public TentacleAttack tentacleAttack;

    private void OnTriggerEnter(Collider other)
    {
        // check if player went in
        if (other.CompareTag("Player"))
        {
            // turn room active on
            if (tentacleAttack != null)
            {
                tentacleAttack.playerInActiveRoom = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // check if player left room
        if (other.CompareTag("Player"))
        {
            // turn room active off
            if (tentacleAttack != null)
            {
                tentacleAttack.playerInActiveRoom = false;
            }
        }
    }
}
