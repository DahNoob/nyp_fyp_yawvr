using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWorldPicker : MonoBehaviour
{
    [SerializeField]
    private Transform m_raycastOrigin;
    [SerializeField]
    private Transform m_raycastPointIndicator;

    private LineRenderer lineRenderer;
    private WorldPickable currentPickable;

    void Start()
    {
        lineRenderer = m_raycastOrigin.GetComponent<LineRenderer>();
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_raycastOrigin.position, m_raycastOrigin.forward, out hit, 999999))
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
    }
}
