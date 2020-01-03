using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestTargetFollower : DynamicQuadTreeObject
{
    [SerializeField]
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.Warp(transform.position);
        AddToQuadTree(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Move towards the mouse cursor
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 10000);
        
        if(Physics.Raycast(ray.origin,ray.direction,out hit, float.MaxValue, LayerMask.GetMask("Debris")))
        {
            navMeshAgent.SetDestination(hit.point);
        }

   
    }
}
