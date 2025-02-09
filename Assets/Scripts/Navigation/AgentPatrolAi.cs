
using UnityEngine;
using UnityEngine.AI;


public class AgentPatrolAi : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Vector3 destination;
    protected float timeout;


    protected bool inMoving;
    protected int index;

    public Vector3 velocity { get; private set; }


    public float wait;
    protected bool inWating;
    protected bool inCatching;

    public GameObject[] patrolPoints;

    public FieldOfView fov;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        agent = GetComponent<NavMeshAgent>();
        destination = this.transform.position;
        inMoving = false;
        inWating = false;
        inCatching = false;
        velocity = agent.velocity;
        timeout = 0;

    }

    public void Update()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }
        velocity = agent.velocity;

        if (fov != null)
        {
            if (fov.canSeePlayer)
            {
                inCatching = true;
                SetDestination(fov.player.transform.position);
            } else
            {
                if (inCatching)
                {
                    SetDestination(patrolPoints[index].transform.position);
                    inCatching = false;
                }
            }
        }

        if (inMoving)
        {
            if (Vector3.Distance(transform.position, destination) < .1f)
            {
                inMoving = false;
                inWating = true;
                timeout = 0;
            }
        } 
        else
        {
            if (inWating)
            {
                timeout += Time.deltaTime;
                if (timeout > wait) 
                {
                    inWating = false;
                    timeout = 0;
                }

            } else
            {

                SetDestination(patrolPoints[index].transform.position);
                ++index;
                if (index > patrolPoints.Length - 1)
                    index = 0;
            }

        }

    }

    public void FixedUpdate()
    {
        
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        agent.destination = destination;
        inMoving = true;
        timeout = 0;
        velocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (inMoving)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawSphere(destination, .2f);

        Gizmos.DrawLine(transform.position, destination);
    }

    private void OnChangePlayerEnabled(bool enabled)
    {

        this.enabled = enabled;
    }

}
