using UnityEngine;

public class CelluleMovement : MonoBehaviour, PlayerMovementInterface
{

    Vector3 velocity;
    Rigidbody rb;


    void PlayerMovementInterface.Dash()
    {
        // throw new System.NotImplementedException();
    }

    void PlayerMovementInterface.Direction(Vector3 moveDirection)
    {
        velocity = moveDirection;
    }

    void PlayerMovementInterface.Jump()
    {
        // throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    void PlayerMovementInterface.StopJump()
    {
        // throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(velocity); 
    }
}
