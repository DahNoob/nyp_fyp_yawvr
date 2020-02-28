using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWorldPicker : MonoBehaviour
{
    [SerializeField]
    private OVRInput.Controller m_controller;
    [SerializeField]
    private Transform m_raycastOrigin;
    [SerializeField]
    private Transform m_raycastPointIndicator;

    private LineRenderer lineRenderer;
    private WorldPickable currentPickable;
    private bool indexTriggered = false;

    void Start()
    {
        lineRenderer = m_raycastOrigin.GetComponent<LineRenderer>();
    }
    void Update()
    {
        bool frameTriggered = false;
        //if (!indexTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) > 0.55f)
        //    frameTriggered = indexTriggered = true;
        //else if (indexTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) < 0.35f)
        //    indexTriggered = false;
        if (!indexTriggered && OVRInput.Get(OVRInput.Button.One, m_controller))
            frameTriggered = indexTriggered = true;
        else
            indexTriggered = false;
        string[] layers = { "Props", "Pickable" };

        RaycastHit hit;
        if (Physics.Raycast(m_raycastOrigin.position, m_raycastOrigin.forward, out hit, 999999, LayerMask.GetMask(layers)))
        {
            m_raycastPointIndicator.position = hit.point;
            WorldPickable prevPickable = currentPickable;
            currentPickable = hit.collider.GetComponent<WorldPickable>();
            if(currentPickable)
            {
                currentPickable.SetHighlighted(true);
            }
            if (currentPickable != prevPickable)
                prevPickable?.SetHighlighted(false);
        }
        if(currentPickable && frameTriggered)
        {
            currentPickable.TriggerSelect();
        }
    }
}
