using UnityEngine;
using UnityEngine.EventSystems;

public interface PlayerMovementInterface
{

    void Jump();

    void StopJump();

    void Dash();

    void Direction(Vector3 moveDirection);

}
