
using System.Threading;
using UnityEngine;
using UnityEngine.AI;


public class AgentPatrolAi : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Vector3 destination;
    protected float timeout;

    protected int index;
    protected AgentState state;
    protected float refreshAgentPos;

    public Vector3 velocity { get; private set; }

    public float wait;

    public float limitDistance;

    public GameObject[] patrolPoints;

    public FieldOfView fov;

    public enum AgentState
    {
        Wait,
        Patrol,
        PatrolFinish,
        CatchIt,
        CatchItFinish,
        Attack
    }

    // Start is called before the first frame update
    void Start()
    {
        state = AgentState.Wait;
        index = 0;
        agent = GetComponent<NavMeshAgent>();
        destination = this.transform.position;
        velocity = agent.velocity;
        timeout = 0;
        refreshAgentPos = 0.50f;

    }

    public void Update()
    {
        switch (state)
        {
            case AgentState.Wait:
                Wait();
                break;

            case AgentState.PatrolFinish:
                IsPatrolFinish();
                break;

            case AgentState.Patrol:
                Patrol();
                break;

            case AgentState.CatchIt:
                CatchIt();
                break;

            case AgentState.CatchItFinish:
                IsCatchItFinish();
                break;

            case AgentState.Attack:
                Attack();
                break;
        }

        if (patrolPoints.Length == 0)
        {
            return;
        }
        velocity = agent.velocity;

    }

    protected void Wait()
    {
        timeout += Time.deltaTime;
        if (timeout > wait)
        {
            state = AgentState.Patrol;
        }
        else
        {
            CanSeePlayer();
        }

    }

    protected bool CanSeePlayer()
    {
        if (fov != null)
        {
            if (fov.canSeePlayer)
            {
                state = AgentState.CatchIt;
                return true;
            }
        }
        return false;
    }

    protected void Patrol()
    {
        timeout = 0;
        state = AgentState.PatrolFinish;
        SetDestination(patrolPoints[index].transform.position);
    }

    protected void IsPatrolFinish()
    {
        timeout += Time.deltaTime;
        if (timeout > refreshAgentPos)
        {
            timeout = 0;
            CanSeePlayer();
            return;
        }

        if (Vector3.Distance(transform.position, destination) < .1f)
        {
            timeout = 0;
            state = AgentState.Wait;
            ++index;
            if (index > patrolPoints.Length - 1)
                index = 0;
        }
        else
        {
            CanSeePlayer();
        }
    }
        
    protected void CatchIt()
    {
        timeout = 0;
        Vector3 diff = fov.player.transform.position - transform.position;
        float length = diff.magnitude;
        Vector3 dir = Vector3.Normalize(diff);
        Vector3 dest = transform.position + (dir * (length - limitDistance));
        SetDestination(dest);
        state = AgentState.CatchItFinish;
    }

    protected void IsCatchItFinish()
    {
        timeout += Time.deltaTime;
        if (timeout > refreshAgentPos)
        {
            timeout = 0;
            CanSeePlayer();
            return;
        }
        if (Vector3.Distance(transform.position, destination) < .1f)
        {
            timeout = 0;
            state = AgentState.Attack;
        }
    }

    protected void Attack()
    {
        timeout += Time.deltaTime;
        if (timeout > 5.0f)
        {
            timeout = 0;
            if (!CanSeePlayer())
                state = AgentState.Wait;
        } 
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        agent.destination = destination;
        velocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawSphere(destination, .2f);

        Gizmos.DrawLine(transform.position, destination);
    }

    private void OnChangePlayerEnabled(bool enabled)
    {

        this.enabled = enabled;
    }

}
