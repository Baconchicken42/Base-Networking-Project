using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.red);

    

    public float movementSpeed = .25f;

    
    
    public Vector3 CalcMovement()
    {
        Vector3 moveVect = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveVect *= movementSpeed;
        return moveVect;
    }

    
    //Server Remote Procedure Call
    [ServerRpc]
    void RequestPositionForMovementServerRpc(Vector3 movement)
    {
        Position.Value += movement;

        float planeSize = 5f;
        Vector3 newPosition = Position.Value += movement;
        newPosition.x = Mathf.Clamp(newPosition.x, planeSize * -1, planeSize);
        newPosition.z = Mathf.Clamp(newPosition.z, planeSize * -1, planeSize);
        Position.Value = newPosition;
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

    private void Start()
    {
        GameManager manager = FindObjectOfType<GameManager>();

        applyPlayerColor();
        playerColor.OnValueChanged += OnPlayerColorChanged;
        if (IsOwner)
            manager.RequestNewPlayerColorServerRpc();
    }

    public void applyPlayerColor()
    {
        GetComponent<MeshRenderer>().material.color = playerColor.Value;
    }

    public void OnPlayerColorChanged(Color previous, Color current)
    {
        applyPlayerColor();
    }

}
