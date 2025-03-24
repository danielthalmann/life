using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlToMovement : MonoBehaviour
{

    public PlayerMovement playerMovement;

    public Vector3 moveDirection;

    public InputActionReference move;
    public InputActionReference jump;

    public bool isJump;

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = move.action.ReadValue<Vector2>();
        isJump = jump.action.IsPressed();

        moveDirection = new Vector3(direction.x, 0, direction.y);

        playerMovement.Direction(moveDirection);
        playerMovement.Jump(isJump);
    }

    private void FixedUpdate()
    {


    }
}
