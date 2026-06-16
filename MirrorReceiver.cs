using UnityEngine;

public class BeamReceiver : MonoBehaviour
{
    [SerializeField] private DoorMover doorToUnlock;

    private bool powered;

    public void SetPowered(bool value)
    {
        if (powered == value) return;

        powered = value;

        Debug.Log("Receiver powered: " + powered);

        if (doorToUnlock != null)
        {
            if (powered)
                doorToUnlock.OpenDoor();
            else
                doorToUnlock.CloseDoor();
        }
    }
}