using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour
{

    GameObject[] goalLocations;
    NavMeshAgent agent;
    Animator anim;
    float speedMult;

    public float detectionRadius = 5f;
    public float fleeRadius = 10f;

    // Use this for initialization
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        int i = Random.Range(0, goalLocations.Length);
        agent.SetDestination(goalLocations[i].transform.position);
        anim = this.GetComponent<Animator>();
        anim.SetFloat("wOffset", Random.Range(0.0f, 1.0f));
        ResetAgent();
    }

    void ResetAgent()
    {
        speedMult = Random.Range(0.5f, 2f);
        anim.SetFloat("speedMult", speedMult);
        agent.speed *= speedMult;
        anim.SetTrigger("isWalking");
        agent.angularSpeed = 120;
        agent.ResetPath();
    }

    public void DetectNewObstacle(Vector3 position)
    {
        if (Vector3.Distance(position, this.transform.position) < detectionRadius)
        {
            Vector3 fleeDirection = (this.transform.position - position).normalized;
            Vector3 newgoal = this.transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newgoal, path);

            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                anim.SetTrigger("isRunning");
                agent.speed = 10;
                agent.angularSpeed = 500;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 1)
        {
            ResetAgent();
            int i = Random.Range(0, goalLocations.Length);
            agent.SetDestination(goalLocations[i].transform.position);
        }
    }
}