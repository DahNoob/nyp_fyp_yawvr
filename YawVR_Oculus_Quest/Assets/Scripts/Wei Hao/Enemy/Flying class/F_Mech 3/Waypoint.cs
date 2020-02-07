using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    Vector3 NearestWaypointToPlayer;
    GameObject player;
    [SerializeField]
    private GameObject[] waypoints;

    [SerializeField]
    float movSpeed;
    [SerializeField]
    float rotSpeed;

    //Fetch the Animator
    Animator m_Animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        m_Animator = GetComponentInChildren<Animator>();
        NearestWaypointToPlayer = findNearest().transform.position;
    }

    void Update()
    {
        NearestWaypointToPlayer = findNearest().transform.position;
        Debug.Log("Nearest waypoint is: " + NearestWaypointToPlayer);
        gameObject.transform.position = Vector3.MoveTowards(transform.position, NearestWaypointToPlayer, movSpeed * Time.deltaTime);
        
        if (transform.position.x == NearestWaypointToPlayer.x && transform.position.z == NearestWaypointToPlayer.z)
        {
            m_Animator.SetBool("Spawn_GetReady", true);
        }
        else
        {
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(NearestWaypointToPlayer.x, 0, NearestWaypointToPlayer.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotSpeed * Time.deltaTime);
        }
    }
    public GameObject findNearest()
    {
         //waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
         int j = 0;
         float smallest = Vector3.Distance(player.transform.position, waypoints[0].transform.position);
         for(int i = 1; i<waypoints.Length; i++)
         {
    	    float dist = Vector3.Distance(player.transform.position, waypoints[i].transform.position);

            if(dist < smallest)
            {
                smallest = dist;
                j = i;
            }
         }
        return waypoints[j];
    }
}
