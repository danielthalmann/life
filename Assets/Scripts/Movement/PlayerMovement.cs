using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum State
    {
        Ground,
        Jump,
        Summit,
        Fall,
    }

    public MovementSettings settings;
    
    public Collider feetCollider;

    private float verticalSpeed;
    private float distance;
    private Vector3 initPosition;
    private State state;
    private int doubleJumpCount;
    private float _maxHeight;
    private float _maxDistance;
    private float _speed;

    private bool _jump;
    private Vector3 _direction;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = State.Ground;
        initPosition = transform.position;
        _speed = 0;
        _jump = false;
    }

    public void Jump(bool jmp)
    {
        _jump = jmp;
    }

    public void Direction(Vector3 dir)
    {
        _direction = dir;
    }

    private void Update()
    {


        if (state == State.Summit)
        {
            _jump = false;
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
            if (_jump)
            {
                doubleJumpCount = 0;
                state = State.Jump;
                initPosition = transform.position;
                _maxHeight = settings.maxHeight;
                _maxDistance = settings.maxDistance;
                distance = 0;
            }
        }

        /*
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(0, -135f, 0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(0, -45f, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, -90f, 0);
            }
            move = true;

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
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
        */

        if (_direction != Vector3.zero)
        {
            if (_speed < settings.maxSpeed)
                _speed += Time.deltaTime * settings.acceleration;
            if (_speed > settings.maxSpeed)
                _speed = settings.maxSpeed;
        } else
        {
            if (_speed > 0)
                _speed -= Time.deltaTime * settings.deceleration;
            if (_speed < 0)
                _speed = 0;
        }
        if (_speed > 0)
            transform.position = transform.position + (_direction * _speed * Time.deltaTime);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }


    void UpdateJump()
    {
        if (_jump == false)
        {
            distance = _maxDistance - distance;
            state = State.Fall;
            return;
        }

        verticalSpeed = Mathf.Lerp(settings.speedUp, settings.speedDown, distance / _maxDistance);
        distance += Time.deltaTime * verticalSpeed;

        if (distance > _maxDistance / 2)
        {
            state = State.Summit;
        }
        else
        {
            float newHeight;
            newHeight = initPosition.y + HeightInStep(distance, _maxHeight, _maxDistance);

            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);

        }
    }


    void UpdateFall()
    {

        verticalSpeed = Mathf.Lerp(settings.speedUp, settings.speedDown, distance / _maxDistance);
        distance += Time.deltaTime * verticalSpeed;

        if (distance > (settings.maxDistance + settings.fallDistance))
        {
            state = State.Ground;
        }
        else
        {

            float newHeight;
            newHeight = initPosition.y + HeightInStep(distance, _maxHeight, _maxDistance);

            Vector3 boxCastOrigin = new Vector3(feetCollider.bounds.center.x, feetCollider.bounds.center.y, feetCollider.bounds.center.z);
            Vector3 boxCastSize = new Vector3(feetCollider.bounds.size.x, feetCollider.bounds.size.y, feetCollider.bounds.size.z);

            Vector3 oldOrigin = transform.position;
            Vector3 newOrigin = new Vector3(transform.position.x, newHeight, transform.position.z);
            Vector3 direction = newOrigin - oldOrigin;
            direction.Normalize();

            RaycastHit hit;
            bool groundHit = Physics.Raycast(oldOrigin, direction, out hit, feetCollider.bounds.size.y + Vector3.Distance(oldOrigin, newOrigin), settings.groundLayer);

            if (groundHit)
            {
                Debug.Log("hit");
                transform.position = hit.point + (Vector3.up * boxCastSize.y);
                state = State.Ground;
            }
            else
            {
                transform.position = newOrigin;
            }
            

        }
        /*
        if(IsGrounded())
        {
            state = State.Ground;
        }
        */

        if (_jump && settings.doubleJump > 0)
        {
            if (doubleJumpCount < settings.doubleJump)
            {
                doubleJumpCount++;
                state = State.Jump;
                initPosition = transform.position;
                //_maxHeight = _maxHeight * settings.doubleJumpReduce / 100;
                _maxDistance = _maxDistance * settings.doubleJumpReduce / 100;
                distance = 0;
                return;
            }
        }

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
        Vector3 pos;

        if (state == State.Ground)
        {
            pos = transform.position;
        } else
        {
            pos = initPosition;
        }

        Vector3 from;
        Vector3 to;

        float step = ((settings.maxDistance + settings.fallDistance) / 30.0f);
        float current = step;

        from = pos;

        for (int i = 1; i <= 30; i++)
        {
            current = step * i;
            to = pos + transform.up * HeightInStep(current, settings.maxHeight, settings.maxDistance) + transform.forward * current;
            Vector3 direction = to - from;
            direction.Normalize();

            if (Physics.Raycast(from, direction, out hit, Vector3.Distance(from, to), settings.groundLayer))
            {
                // If a hit is detected, stop drawing the arc at the hit point
                to = hit.point;
                i = 30;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, to);
            from = to;
        }

        
    }
}
