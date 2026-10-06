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

        anim = this.GetComponent<Animator>();

        // Tenta definir o wOffset apenas se o Animator tiver o parâmetro
        if (anim != null)
        {
            anim.SetFloat("wOffset", Random.Range(0.0f, 1.0f));
        }

        ResetAgent();

        // Define um destino inicial se houver goals disponíveis
        if (goalLocations.Length > 0 && agent != null && agent.isOnNavMesh)
        {
            int i = Random.Range(0, goalLocations.Length);
            agent.SetDestination(goalLocations[i].transform.position);
        }
    }

    void ResetAgent()
    {
        speedMult = Random.Range(0.5f, 2f);

        if (anim != null)
        {
            anim.SetFloat("speedMult", speedMult);
            anim.SetTrigger("isWalking");
        }

        if (agent != null)
        {
            agent.speed *= speedMult;
            agent.angularSpeed = 120;
            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }
    }

    public void DetectNewObstacle(Vector3 position)
    {
        if (Vector3.Distance(position, this.transform.position) < detectionRadius)
        {
            Vector3 fleeDirection = (this.transform.position - position).normalized;
            Vector3 newgoal = this.transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();

            if (agent != null && agent.isOnNavMesh)
            {
                agent.CalculatePath(newgoal, path);

                if (path.status != NavMeshPathStatus.PathInvalid && path.corners.Length > 0)
                {
                    agent.SetDestination(path.corners[path.corners.Length - 1]);
                    if (anim != null) anim.SetTrigger("isRunning");
                    agent.speed = 10;
                    agent.angularSpeed = 500;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Proteção: só verifica remainingDistance se o agente estiver ativo e no NavMesh
        if (agent != null && agent.isOnNavMesh && !agent.pathPending)
        {
            if (agent.remainingDistance < 1f)
            {
                ResetAgent();
                if (goalLocations.Length > 0)
                {
                    int i = Random.Range(0, goalLocations.Length);
                    agent.SetDestination(goalLocations[i].transform.position);
                }
            }
        }
    }
}