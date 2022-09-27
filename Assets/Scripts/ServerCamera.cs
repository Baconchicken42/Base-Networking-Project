using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerCamera : NetworkBehaviour
{
    public float movementSpeed = 1f;
    public float rotationSpeed = .5f;

    private void moveCamera()
    {
        Vector3 moveVect = Vector3.zero;
        Vector2 rotVect = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveVect.y = -1f;
            }
            else
            {
                moveVect.y = 1f;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveVect.x = Input.GetAxis("Horizontal");
            rotVect.x = Input.GetAxis("Vertical");
        }
        else
        {
            rotVect.y = Input.GetAxis("Horizontal");
            moveVect.z = Input.GetAxis("Vertical");
        }

        moveVect *= movementSpeed;
        rotVect *= rotationSpeed;

        transform.Translate(moveVect);
        transform.Rotate(rotVect);
    }


    public void Update()
    {
        if (IsServer)
            moveCamera();
    }
}
