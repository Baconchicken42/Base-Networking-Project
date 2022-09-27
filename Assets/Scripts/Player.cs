using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour {
    public NetworkVariable<Vector3> PositionChange = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> RotationChange = new NetworkVariable<Vector3>();
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.red);

    

    public float movementSpeed = .25f;
    public float rotationSpeed = .25f;
    public Camera _camera;

    public Transform bulletSpawn;
    public Rigidbody bullet;
    public float bulletSpeed = 10f;

    
    
    public Vector3[] CalcMovement()
    {
        float x_move = 0f;
        float z_move = Input.GetAxis("Vertical");
        float y_rot = 0f;

        if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            x_move = Input.GetAxis("Horizontal");
        }
        else
        {
            y_rot = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed;

        Vector3 rotVect = new Vector3(0, y_rot, 0);
        rotVect *= rotationSpeed;

        return new[] { moveVect, rotVect };
    }

    
    //Server Remote Procedure Call
    [ServerRpc]
    void RequestPositionForMovementServerRpc(Vector3 movement, Vector3 rotation)
    {
        if (!IsServer && !IsHost) return;

        PositionChange.Value = movement;
        RotationChange.Value = rotation;
    }

    

    private void Update()
    {
        if (IsOwner)
        {
            Vector3[] movementResults = CalcMovement();
            if (movementResults[0].magnitude > 0 || movementResults[1].magnitude > 0)
            {
                RequestPositionForMovementServerRpc(movementResults[0], movementResults[1]);
            }
        }
        if (!IsOwner || IsHost)
        {
            transform.Translate(PositionChange.Value);
            transform.Rotate(RotationChange.Value);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && IsOwner)
        {
            fireBulletServerRPC();
        }
    }

    private void Start()
    {
        GameManager manager = FindObjectOfType<GameManager>();

        applyPlayerColor();
        playerColor.OnValueChanged += OnPlayerColorChanged;
        if (IsOwner)
            manager.RequestNewPlayerColorServerRpc();
        else
        {
            _camera.enabled = false;
        }
    }

    public void applyPlayerColor()
    {
        GetComponent<MeshRenderer>().material.color = playerColor.Value;
    }

    public void OnPlayerColorChanged(Color previous, Color current)
    {
        applyPlayerColor();
    }

    [ServerRpc]
    public void fireBulletServerRPC()
    {
        Rigidbody newBullet = Instantiate(bullet, bulletSpawn.position, bullet.rotation);
        newBullet.velocity = bulletSpawn.forward * bulletSpeed;
        newBullet.gameObject.GetComponent<NetworkObject>().Spawn();
        Destroy(newBullet.gameObject, 3);
    }
}
