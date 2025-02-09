using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAnimation : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetFloat("speed", agent.velocity.magnitude);
    }
}
