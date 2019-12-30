using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: MechLaserPointer monobehaviour
** Desc: A monobehaviour to be used by projectile-related mech modules
** Author: DahNoob
** Date: 26/12/2019, 3:13PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    26/12/2019, 3:13PM      DahNoob   Created
*******************************/
public class MechLaserPointer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private float m_maxDistance = 500.0f;
    [SerializeField]
    private OVRInput.Controller m_controller;

    //Local variables
    public Vector3 hitPoint { private set; get; }

    void Update()
    {
        //if(m_enabled)
        //{
        RaycastHit hit;
        LineRenderer lr = GetComponent<LineRenderer>();
        //LayerMask projectileMask;
        //Ignore layer player projectile layer
        LayerMask projectileMask = ~LayerMask.GetMask("PlayerProjectile");

        //if the raycast retursn true, set point to be hit, else its the max point
        //Conditional operator doesn't work?
        if (Physics.Raycast(transform.position, transform.forward, out hit, m_maxDistance, projectileMask))
        {
            hitPoint = hit.point;
            //Apply it to the instance
            GUIManager.instance.SetReticleInformation(m_controller, hitPoint, hit.collider.gameObject, true);
        }
        else
        {
            hitPoint = transform.position + transform.forward * m_maxDistance;
            //Apply it to the instance
            GUIManager.instance.SetReticleInformation(m_controller, hitPoint, null, true);
        }

        Vector3[] points = new Vector3[3];
        points[0] = transform.position;
        //points[1] = Vector3.Lerp(transform.position, hitPoint, 0.5f);
        points[1] = hitPoint;

        //lr.SetPositions(points);
        //}
    }

    //void OnEnable()
    //{
    //    GetComponent<LineRenderer>().enabled = true;
    //}
    //void OnDisable()
    //{
    //    GetComponent<LineRenderer>().enabled = false;
    //}
}
