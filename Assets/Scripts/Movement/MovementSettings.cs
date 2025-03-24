using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Scriptable Objects/MovementOptions")]
public class MovementSettings : ScriptableObject
{

    [Header("Jump")]

    [Range(0.001f, 1f)]
    public float maxHeight;
    [Range(0, 40)]
    public float maxDistance;
    [Range(0, 40)]
    public float fallDistance;
    

    [Header("Jump speed")]

    [Range(0.5f, 10f)]
    public float speedUp;
    [Range(0.5f, 10f)]
    public float speedDown;

    [Header("Double jump")]

    [Range(0, 5)]
    public int doubleJump = 0;
    [Range(1f, 100f)]
    public float doubleJumpReduce = 100;

    [Header("Layers")]
    public LayerMask groundLayer;

    [Header("Walk")]

    [Range(0.5f, 20f)]
    public float maxSpeed;

    [Range(0.1f, 50f)]
    public float acceleration;
    
    [Range(0.1f, 50f)]
    public float deceleration;
    

}
