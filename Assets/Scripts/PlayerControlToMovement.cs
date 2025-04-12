using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlToMovement : MonoBehaviour
{

    private PlayerMovementInterface playerMovement;

    public Vector3 moveDirection;

    public InputActionReference move;
    public InputActionReference dash;
    public InputActionReference jump;

    public bool fixHorizontal;
    public bool fixVertical;
    public bool InverseAxe;

    private bool jumping;
    private bool dashed;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovementInterface>();
        if (playerMovement == null)
        {
            Debug.LogError("Need Player Movement Object");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (jump != null && jump.action.IsPressed() && !jumping)
        {
            jumping = true;
            playerMovement.Jump();
        }
        if (jump != null && !jump.action.IsPressed() && jumping)
        {
            jumping = false;
            playerMovement.StopJump();
        }

        if (dash != null && dash.action.IsPressed() && !dashed)
        {
            dashed = true;
            playerMovement.Dash();
        }
        if (dash != null && !dash.action.IsPressed() && dashed)
        {
            dashed = false;
        }

        if (move != null)
        {
            Vector2 direction = move.action.ReadValue<Vector2>();
            if(InverseAxe)
            {
                moveDirection = new Vector3(fixHorizontal ? 0 : direction.x, fixVertical ? 0 : direction.y, 0);
            }
            else
            {
                moveDirection = new Vector3(fixHorizontal ? 0 : direction.x, 0, fixVertical ? 0 : direction.y);
            }
            playerMovement.Direction(moveDirection);
        }
        
    }

    private void FixedUpdate()
    {


    }
}
