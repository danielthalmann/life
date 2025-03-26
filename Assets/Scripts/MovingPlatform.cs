using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    enum State
    {
        Wait,
        StartTraval,
        Travel
    };

    public Vector3[] points;
    public float stayDelay;
    public float speed;

    private float _wait;
    private int _index;
    private State state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _wait = 0;
        _index = -1;
        state = State.Wait;

        if (points.Length == 0)
        {
            this.enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Wait)
        {
            if (_wait < stayDelay)
            {
                _wait += Time.deltaTime;
            }
            else
            {
                state = State.StartTraval;
                _wait = 0;
            }
        }
        if (state == State.StartTraval)
        {
            _index++;
            if (_index >= points.Length)
                _index = 0;
            state = State.Travel;
        }
        if (state == State.Travel)
        {
            transform.position = Vector3.Lerp(transform.position, points[_index], Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, points[_index]) < 0.01f)
            {
                state = State.Wait;
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for(int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i], 0.2f);
        }

    }
}
