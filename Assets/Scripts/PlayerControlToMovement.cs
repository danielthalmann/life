using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlToMovement : MonoBehaviour
{

    public PlayerMovement playerMovement;

    public Vector3 moveDirection;

    public InputActionReference move;
    public InputActionReference dash;
    public InputActionReference jump;

    public bool fixHorizontal;
    public bool fixVertical;

    private bool jumping;
    private bool dashed;



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

        if (dash.action.IsPressed() && !dashed)
        {
            dashed = true;
            playerMovement.Dash();
        }
        if (!dash.action.IsPressed() && dashed)
        {
            dashed = false;
        }


        Vector2 direction = move.action.ReadValue<Vector2>();
        moveDirection = new Vector3(fixHorizontal ? 0 : direction.x, 0, fixVertical ? 0 : direction.y);
        playerMovement.Direction(moveDirection);
        
    }

    private void FixedUpdate()
    {


    }
}
