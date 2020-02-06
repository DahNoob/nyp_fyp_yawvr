using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyCratesDropHandler : MonoBehaviour
{
    [Header("Configuration")]
    public Transform m_crateA;
    public Transform m_crateB;
    public Transform m_crateC;

    private void Start()
    {
        StartDrop();
    }

    public void StartDrop()
    {
        GetComponent<Animator>().Play("Drop");
    }

    void _AnimationEnd()
    {
        RaycastHit hit;
        Physics.Raycast(m_crateA.position + Vector3.up * 5, Vector3.down, out hit, 500, LayerMask.GetMask("Terrain"));
        Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE, hit.point, m_crateA.rotation, Persistent.instance.GO_STATIC.transform);
        Physics.Raycast(m_crateB.position + Vector3.up * 5, Vector3.down, out hit, 500, LayerMask.GetMask("Terrain"));
        Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE, hit.point, m_crateB.rotation, Persistent.instance.GO_STATIC.transform);
        Physics.Raycast(m_crateC.position + Vector3.up * 5, Vector3.down, out hit, 500, LayerMask.GetMask("Terrain"));
        Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE, hit.point, m_crateC.rotation, Persistent.instance.GO_STATIC.transform);
        Destroy(gameObject);
    }
}
