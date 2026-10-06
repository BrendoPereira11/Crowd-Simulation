using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Necessário se estiveres a usar o novo Input System das imagens

public class DropCylinder : MonoBehaviour
{
    public GameObject obstacle;
    GameObject[] agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("agent");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            {
                Instantiate(obstacle, hitInfo.point, obstacle.transform.rotation);
                foreach (GameObject a in agents)
                {
                    a.GetComponent<AIControl>().DetectNewObstacle(hitInfo.point);
                }
            }
        }
    }
}