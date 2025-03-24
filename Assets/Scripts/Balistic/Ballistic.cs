using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Ballistic : MonoBehaviour
{
    public enum State
    {
        Ground,
        Jump,
        Summit,
        Fall,
    }


    [Range(0.001f,1f)]
    public float maxHeight;
    [Range(0, 40)]
    public float maxDistance;
    [Range(-40, 40)]
    public float fallDistance;

    [Range(0.5f, 10f)]
    public float speedUp;
    [Range(0.5f, 10f)]
    public float speedDown;

    [Range(0.5f, 10f)]
    public float speed;

    [Range(0, 5)]
    public int doubleJump = 0;

    [Range(1f, 100f)]
    public float doubleJumpReduce = 100;

    public bool balistic;

    public Collider feetCollider;

    public float groundDetectionRayLength;
    public LayerMask groundLayer;

    public bool DebugShow;

    private float verticalSpeed;
    private float distance;
    private Vector3 initPosition;
    private State state;
    private bool move;
    private int doubleJumpCount;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = State.Ground;
        initPosition = transform.position;
        move = false;
    }

    private void Update()
    {


        if (state == State.Summit)
        {
            state = State.Fall;
        }

        if (state == State.Fall)
        {
            UpdateFall();
        }
        if (state == State.Jump)
        {
            UpdateJump();
        }

        if (state == State.Ground)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                doubleJumpCount = 0;
                state = State.Jump;
                initPosition = transform.position;
                distance = 0;
            }
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(0, -135f, 0);
            } else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(0, -45f, 0);
            } else
            {
                transform.rotation = Quaternion.Euler(0, -90f,0);
            }
            move = true;

        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(0, 135f, 0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(0, 45f, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 90f, 0);
            }
            move = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0, -180f, 0);
            move = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0f, 0);
            move = true;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow) 
            || Input.GetKeyUp(KeyCode.LeftArrow)
            || Input.GetKeyUp(KeyCode.UpArrow)
            || Input.GetKeyUp(KeyCode.DownArrow))
        {
            move = false;
        }

        if (move)
        {
            transform.position = transform.position + (transform.forward * speed * Time.deltaTime);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }


    void UpdateJump()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            distance = maxDistance - distance;
            state = State.Fall;
            return;
        }

        verticalSpeed = Mathf.Lerp(speedUp, speedDown, distance / maxDistance);
        distance += Time.deltaTime * verticalSpeed;

        if (distance > maxDistance / 2)
        {
            state = State.Summit;
        }
        else
        {
            if (balistic)
            {
                transform.position = initPosition + transform.up * HeightInStep(distance, maxHeight, maxDistance) + transform.forward * distance;
            }
            else
            {
                float newHeight;
                newHeight = initPosition.y + HeightInStep(distance, maxHeight, maxDistance);

                transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);

            }
        }
    }


    void UpdateFall()
    {

        verticalSpeed = Mathf.Lerp(speedUp, speedDown, distance / maxDistance);
        distance += Time.deltaTime * verticalSpeed;

        if (distance > (maxDistance + fallDistance))
        {
            state = State.Ground;
        }
        else
        {
            if (balistic)
            {
                transform.position = initPosition + transform.up * HeightInStep(distance, maxHeight, maxDistance) + transform.forward * distance;
            }
            else
            {
                float newHeight;
                newHeight = initPosition.y + HeightInStep(distance, maxHeight, maxDistance);

                Vector3 boxCastOrigin = new Vector3(feetCollider.bounds.center.x, feetCollider.bounds.center.y, feetCollider.bounds.center.z);
                Vector3 boxCastSize = new Vector3(feetCollider.bounds.size.x, feetCollider.bounds.size.y, feetCollider.bounds.size.z);

                Vector3 oldOrigin = transform.position;
                Vector3 newOrigin = new Vector3(transform.position.x, newHeight, transform.position.z);
                Vector3 direction = newOrigin - oldOrigin;
                direction.Normalize();

                RaycastHit hit;
                bool groundHit = Physics.Raycast(oldOrigin, direction, out hit, feetCollider.bounds.size.y + Vector3.Distance(oldOrigin, newOrigin), groundLayer);

                if (groundHit)
                {
                           Debug.Log("hit");
                    transform.position = hit.point + (Vector3.up * boxCastSize.y);
                    state = State.Ground;
                } else
                {
                    transform.position = newOrigin;
                }
            }

        }
        /*
        if(IsGrounded())
        {
            state = State.Ground;
        }
        */

        if (Input.GetKeyDown(KeyCode.Space) && doubleJump > 0)
        {
            if (doubleJumpCount < doubleJump)
            {
                doubleJumpCount++;
                state = State.Jump;
                initPosition = transform.position;
                distance = 0;
                return;
            }
        }

    }

    private float fallMaxHeight;

    private bool IsGrounded()
    {
        RaycastHit hit;

        Vector3 boxCastOrigin = new Vector3(feetCollider.bounds.center.x, feetCollider.bounds.center.y, feetCollider.bounds.center.z);
        Vector3 boxCastSize = new Vector3(feetCollider.bounds.size.x, feetCollider.bounds.size.y, feetCollider.bounds.size.z);

        bool groundHit = Physics.BoxCast(boxCastOrigin, boxCastSize, Vector3.down, out hit, transform.rotation, groundDetectionRayLength, groundLayer);

        if(groundHit)
        {
            fallMaxHeight = Vector3.Distance(hit.point, boxCastOrigin);
        }

        return groundHit;

    }


    /// <summary>
    /// Retourne le temps de vol en seconde du projectile
    /// </summary>
    /// <param name="initialSpeed"></param>
    /// <param name="throwAngle"></param>
    /// <param name="worldGravity"></param>
    /// <returns></returns>
    private float FlyTime(float initialSpeed, float throwAngle, float worldGravity)
    {
        return (2 * initialSpeed * Mathf.Sin(Mathf.Deg2Rad * throwAngle)) / worldGravity;
    }

    /// <summary>
    /// retourne la hauteur selon la vitesse et l'angle
    /// </summary>
    /// <param name="initialSpeed"></param>
    /// <param name="throwAngle"></param>
    /// <param name="worldGravity"></param>
    /// <returns></returns>
    private float HeighMax(float initialSpeed, float throwAngle, float worldGravity)
    {
        float sinus = Mathf.Sin(Mathf.Deg2Rad * throwAngle);

        return (initialSpeed * initialSpeed * sinus * sinus) / (2 * worldGravity);
    }


    private float HeightInStep(float step, float height, float distance)
    {
        return (-height * (step * step)) + (height * distance * step);
    }



    private void OnDrawGizmos()
    {
        RaycastHit hit;

        if (state == State.Ground)
        {
            Vector3 from;
            Vector3 to;

            float step = ((maxDistance + fallDistance) / 30.0f);
            float current = step;

            from = transform.position;

            for (int i = 1; i <= 30; i++)
            {
                current = step * i;
                to = transform.position + transform.up * HeightInStep(current, maxHeight, maxDistance) + transform.forward * current;
                Vector3 direction = to - from;
                direction.Normalize();

                if (Physics.Raycast(from, direction, out hit, Vector3.Distance(from, to), groundLayer))
                {
                    // If a hit is detected, stop drawing the arc at the hit point
                    to = hit.point;
                    i = 30;
                }

                Gizmos.color = Color.red;
                Gizmos.DrawLine(from, to);
                from = to;
            }

            Vector3 boxCastOrigin = new Vector3(feetCollider.bounds.center.x, feetCollider.bounds.center.y, feetCollider.bounds.center.z);

            Gizmos.DrawLine(boxCastOrigin, boxCastOrigin + (Vector3.down * groundDetectionRayLength));

            // Gizmos.DrawWireSphere(initPosition, maxDistance);
        }

    }
}
