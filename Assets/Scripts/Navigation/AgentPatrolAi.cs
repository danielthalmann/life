
using UnityEngine;
using UnityEngine.AI;


public class AgentPatrolAi : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Vector3 destination;
    protected float timeout;

    protected bool inMoving;
    protected int index;

    [field: SerializeField]
    public Vector3 velocity { get; private set; }

    public GameObject[] patrolPoints;


    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        agent = GetComponent<NavMeshAgent>();
        destination = this.transform.position;
        inMoving = false;
        velocity = agent.velocity;
        timeout = 0;

    }

    public void Update()
    {

        if (inMoving)
        {
            timeout += Time.deltaTime;
            if (timeout > 0.1  && agent.velocity == Vector3.zero)
            {
                timeout = 0;
                inMoving = false;
            }
        }

        velocity = agent.velocity;

        if (!inMoving && patrolPoints.Length > 0)
        {

            SetDestination(patrolPoints[index].transform.position);
            ++index;
            if (index > patrolPoints.Length - 1)
                index = 0;
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
