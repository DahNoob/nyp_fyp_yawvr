using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestTargetFollower : DynamicQuadTreeObject
{
    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private List<Transform> randomPointsOnMap;

    [SerializeField]
    private Transform currentTargetedTransform;

    [SerializeField]
    private GameObject targetObject;

    public bool goRandom;

    public int fakeHP = 100;


    // Start is called before the first frame update
    void Start()
    {
        AddToQuadTree(this.gameObject);

        //Cheese it
        for (int i =0; i < targetObject.transform.childCount; ++i)
        {
            randomPointsOnMap.Add(targetObject.transform.GetChild(i));
        }

        if (goRandom)
        {
            //Assign the start way point
            AssignNewPoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (goRandom)
        {
            GoRandomPoints();
            navMeshAgent.SetDestination(currentTargetedTransform.position);
        }
    }

    void UseMouseMovement()
    {
        //Move towards the mouse cursor
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 10000);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, float.MaxValue, LayerMask.GetMask("Debris")))
        {
            navMeshAgent.SetDestination(hit.point);
        }
    }

    void GoRandomPoints()
    {
        if (Vector3.Distance(currentTargetedTransform.position, transform.position) < 1f)
        {
            AssignNewPoint();
        }
    }

    void AssignNewPoint()
    {
        //Randomly generate a point to go to
        currentTargetedTransform = randomPointsOnMap[Random.Range(0, randomPointsOnMap.Count)];
        navMeshAgent.SetDestination(currentTargetedTransform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, currentTargetedTransform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="Bullet")
        {
            fakeHP -= 10;
            if(fakeHP <=0)
            {
                QuadTreeManager.instance.Remove(this.gameObject, false);
                Destroy(this.gameObject);
  
            }
        }
    }
}
