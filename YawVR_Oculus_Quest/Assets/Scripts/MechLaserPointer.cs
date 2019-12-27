﻿using System.Collections;
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
        {
            RaycastHit hit;
            LineRenderer lr = GetComponent<LineRenderer>();
            //LayerMask projectileMask;
            //Ignore layer player projectile layer
            LayerMask projectileMask = ~LayerMask.GetMask("PlayerProjectile");
            if (Physics.Raycast(transform.position, transform.forward, out hit, m_maxDistance, projectileMask))
            {               
                hitPoint = hit.point;
                GUIManager.instance.SetHitObjectName(m_controller, hit.collider.gameObject.name);
            }
            else
            {
                hitPoint = transform.position + transform.forward * m_maxDistance;
                GUIManager.instance.SetHitObjectName(m_controller, "monkaS");
            }
            ApplyToReticle();
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hitPoint);
        }
    }

    //void ApplyToReticle(OVRInput.Controller _controller)
    //{
    //    GUIManager.instance.SetReticlePosition(_controller, hitPoint);
    //}

    void ApplyToReticle()
    {
        GUIManager.instance.SetReticlePosition(m_controller, hitPoint);
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
