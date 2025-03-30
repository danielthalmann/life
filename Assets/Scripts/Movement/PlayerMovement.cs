using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum State
    {
        Ground,
        Walk,
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
    public State state;
    private int _doubleJumpCount;
    private float _maxHeight;
    private float _maxDistance;
    private float _speed;

    private bool _jump;
    private bool _stopJump;
    private Vector3 _direction;
    private Vector3 _lastDirection;
    private bool _dash;
    private bool _stopDash;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = State.Ground;
        initPosition = Feet();
        _centerOffset = initPosition.y - transform.position.y;
        _speed = 0;
        _jump = false;
        _stopJump = false;
        _dash = false;
        _stopDash = false;
    }

    public Vector3 Feet()
    {
        //return transform.position;
        return new Vector3(feetCollider.bounds.center.x, feetCollider.bounds.center.y - (feetCollider.bounds.size.y / 2), feetCollider.bounds.center.z);
    }

    public void Jump()
    {
        _jump = true;
        _stopJump = false;
    }

    public void StopJump()
    {
        _stopJump = true;
    }

    public void Dash()
    {
        _dash = true;
        _stopDash = false;
    }

    public void Direction(Vector3 dir)
    {
        _direction = dir;
    }

    private void UpdateMove()
    {

        if (_direction != Vector3.zero)
        {
            if (_dash)
            {
                if (!_stopDash)
                {
                    if (_speed < settings.maxDashSpeed)
                        _speed += Time.deltaTime * (settings.acceleration + settings.dashAcceleration);
                    if (_speed > settings.maxDashSpeed)
                    {
                        _speed = settings.maxDashSpeed;
                        _stopDash = true;
                    }
                }
                else
                {
                    if (_speed > settings.maxSpeed)
                        _speed -= Time.deltaTime * (settings.deceleration + settings.acceleration);
                    if (_speed < settings.maxSpeed)
                    {
                        _speed = settings.maxSpeed;
                        _dash = false;
                    }
                }
            }
            else
            {
                if (_speed < settings.maxSpeed)
                    _speed += Time.deltaTime * settings.acceleration;
                if (_speed > settings.maxSpeed)
                    _speed = settings.maxSpeed;
            }
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

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, settings.frontDistanceDetection)) // , settings.groundLayer))
            {
                _speed = 0;
                transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z) - (transform.forward * settings.frontDistanceDetection);
            }

        }

    }

    private void Update()
    {
        UpdateMove();

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

            RaycastHit hit;
            bool groundHit = Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f, settings.groundLayer);

            if (groundHit)
            {
                transform.position = hit.point - (Vector3.up * _centerOffset);
                this.transform.parent = hit.collider.gameObject.transform;
            }
            else
            {
                this.transform.parent = null;
                initPosition = Feet();
                _lastPosition = Feet();
                _maxHeight = settings.maxHeight;
                distance = settings.maxDistance;
                state = State.Fall;
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
            //if (newHeight < Mathf.Abs(_centerOffset) + (feetCollider.bounds.size.y / 2))
            //    newHeight = Mathf.Abs(_centerOffset) + (feetCollider.bounds.size.y / 2);

            _lastPosition = transform.position;
            Vector3 newOrigin = new Vector3(_lastPosition.x, newHeight, _lastPosition.z);

            RaycastHit hit;
            if (Physics.Raycast(_lastPosition, Vector3.up, out hit, settings.headDistanceDetection, settings.groundLayer))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y - settings.headDistanceDetection, transform.position.z);
                initPosition = Feet();
                state = State.Summit;
            }
            else
            {
                transform.position = newOrigin;
                transform.parent = null;
            }


        }
    }

    void UpdateFall()
    {

        verticalSpeed = Mathf.Lerp(settings.speedUp, settings.speedDown, distance / _maxDistance);
        distance += Time.deltaTime * verticalSpeed;

        float newHeight;
        newHeight = initPosition.y + HeightInStep(distance, _maxHeight, _maxDistance) - _centerOffset;
        //if (newHeight < _centerOffset + (feetCollider.bounds.size.y / 2))
        //    newHeight = _centerOffset + (feetCollider.bounds.size.y / 2);
            
        _lastPosition = transform.position;
        Vector3 newOrigin = new Vector3(_lastPosition.x, newHeight, _lastPosition.z);

        RaycastHit hit;
        if (Physics.Raycast(_lastPosition, Vector3.down, out hit, 1.0f, settings.groundLayer))
        {
            transform.position = hit.point - (Vector3.up * _centerOffset);
            transform.parent = hit.collider.gameObject.transform;
            state = State.Ground;
        }
        else
        {
            transform.position = newOrigin;
            transform.parent = null;
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

        float step = ((settings.maxDistance + 10) / 30.0f);
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
        Gizmos.color = Color.blue;
        //float thickness = 30;
        //Vector3 endPos = transform.position + (transform.forward * settings.frontDistanceDetection);
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * settings.frontDistanceDetection));
        //Handles.DrawBezier(transform.position, endPos, transform.position, endPos, Color.magenta, null, thickness);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * settings.headDistanceDetection));

    }
}
