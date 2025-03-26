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
    private Vector3 _lastPosition;
    private float _centerOffset;
    private State state;
    private int _doubleJumpCount;
    private float _maxHeight;
    private float _maxDistance;
    public float _speed;

    public bool _jump;
    public bool _stopJump;
    private Vector3 _direction;
    private Vector3 _lastDirection;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = State.Ground;
        initPosition = Feet();
        _centerOffset = initPosition.y - transform.position.y;
        Debug.Log(_centerOffset);
        _speed = 0;
        _jump = false;
        _stopJump = false;
    }

    public Vector3 Feet()
    {
        return new Vector3(feetCollider.bounds.center.x, feetCollider.bounds.center.y - (feetCollider.bounds.size.y / 2), feetCollider.bounds.center.z);
    }

    public void Jump()
    {
        _jump = true;
    }
    public void StopJump()
    {
        _stopJump = true;
    }

    public void Direction(Vector3 dir)
    {
        _direction = dir;
    }

    private void Update()
    {

        if (_direction != Vector3.zero)
        {
            if (_speed < settings.maxSpeed)
                _speed += Time.deltaTime * settings.acceleration;
            if (_speed > settings.maxSpeed)
                _speed = settings.maxSpeed;
            _lastDirection = _direction;
        }
        else
        {
            if (_speed > 0)
                _speed -= Time.deltaTime * settings.deceleration;
            if (_speed < 0)
                _speed = 0;
        }
        if (_speed != 0)
        {
            transform.rotation = Quaternion.LookRotation(_lastDirection, Vector3.up);
            transform.position = transform.position + (_lastDirection * _speed * Time.deltaTime);
        }


        if (state == State.Summit)
        {
            _lastPosition = Feet();
            state = State.Fall;
        }

        if (state == State.Fall)
        {
            UpdateFall();

            if (_jump && settings.doubleJump > 0)
            {
                _jump = false;
                _stopJump = false;

                if (_doubleJumpCount < settings.doubleJump)
                {
                    _doubleJumpCount++;
                    state = State.Jump;
                    initPosition = Feet();
                    //_maxHeight = _maxHeight * settings.doubleJumpReduce / 100;
                    _maxDistance = _maxDistance * settings.doubleJumpReduce / 100;
                    distance = 0;
                }
            }

        }

        if (state == State.Jump)
        {
            UpdateJump();

            if (_stopJump)
            {
                _stopJump = false;
                distance = _maxDistance - distance;
                state = State.Summit;
            }

        }

        if (state == State.Ground)
        {
            if (_jump)
            {
                _stopJump = false;
                _jump = false;
                _doubleJumpCount = 0;
                state = State.Jump;
                initPosition = Feet();
                _maxHeight = settings.maxHeight;
                _maxDistance = settings.maxDistance;
                distance = 0;
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }


    void UpdateJump()
    {

        verticalSpeed = Mathf.Lerp(settings.speedUp, settings.speedDown, distance / _maxDistance);
        distance += Time.deltaTime * verticalSpeed;

        if (distance > _maxDistance / 2)
        {
            state = State.Summit;
        }
        else
        {
            float newHeight;
            newHeight = initPosition.y + HeightInStep(distance, _maxHeight, _maxDistance) - _centerOffset;
            if (newHeight < Mathf.Abs(_centerOffset) + (feetCollider.bounds.size.y / 2))
                newHeight = Mathf.Abs(_centerOffset) + (feetCollider.bounds.size.y / 2);

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
            newHeight = initPosition.y + HeightInStep(distance, _maxHeight, _maxDistance) - _centerOffset;
            if (newHeight < Mathf.Abs(_centerOffset) + (feetCollider.bounds.size.y / 2))
                newHeight = Mathf.Abs(_centerOffset) + (feetCollider.bounds.size.y / 2);
            
            _lastPosition = transform.position;
            Vector3 newOrigin = new Vector3(_lastPosition.x, newHeight, _lastPosition.z);

            RaycastHit hit;
            bool groundHit = Physics.Raycast(_lastPosition, Vector3.down, out hit, 1.0f + Vector3.Distance(_lastPosition, newOrigin), settings.groundLayer);
            
            if (groundHit)
            {
                Debug.Log("hit");
                transform.position = hit.point - (Vector3.up * _centerOffset);
                state = State.Ground;
            }
            else
            {
                transform.position = newOrigin;
            }


        }

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
            pos = Feet();
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
