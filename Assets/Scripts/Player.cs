using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

    public float movementSpeed = .5f;
    
    public Vector3 CalcMovement()
    {
        Vector3 moveVect = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveVect *= movementSpeed;
        return moveVect;
    }

    [ServerRpc]
    void RequestPositionForMovementServerRpc(Vector3 movement)
    {
        Position.Value += movement;
    }

    private void Update()
    {
        if (IsOwner)
        {
            Vector3 move = CalcMovement();
            if (move.magnitude > 0)
            {
                RequestPositionForMovementServerRpc(move);
            }
        }
        else
        {
            transform.position = Position.Value;
        }
    }

}
