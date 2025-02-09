using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{

    public float radius;

    [Range(0,360)]
    public float angle;

    public GameObject player;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    public float timeRecurence;

    private float currentWating;

    private 


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentWating = 0;


    }

    // Update is called once per frame
    void Update()
    {
        currentWating += Time.deltaTime;
        if (currentWating > timeRecurence)
        {
            FieldOfViewCheck();
        }

    }

    protected void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if ( !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                } else
                {
                    canSeePlayer = false;
                }
            } else
            {
                canSeePlayer = false;
            }
        } else if (canSeePlayer)
        {
            canSeePlayer = false;
        }

    }
}
