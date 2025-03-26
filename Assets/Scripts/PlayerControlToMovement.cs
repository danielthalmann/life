using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlToMovement : MonoBehaviour
{

    public PlayerMovement playerMovement;

    public Vector3 moveDirection;

    public InputActionReference move;
    public InputActionReference jump;

    public bool jumping;

    // Update is called once per frame
    void Update()
    {
        if (jump.action.IsPressed() && !jumping)
        {
            jumping = true;
            playerMovement.Jump();
        }
        if (!jump.action.IsPressed() && jumping)
        {
            jumping = false;
            playerMovement.StopJump();
        }

        Vector2 direction = move.action.ReadValue<Vector2>();
        moveDirection = new Vector3(direction.x, 0, direction.y);
        playerMovement.Direction(moveDirection);
        
    }

    private void FixedUpdate()
    {


    }
}
