using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPickable : MonoBehaviour
{
    public delegate void Selected();
    public event Selected onSelected;
    private MeshRenderer outlineRenderer;
    private GameObject beacon;

    private bool isHighlighted = false;

    void Awake()
    {
        outlineRenderer = new GameObject("Outline").AddComponent<MeshRenderer>();
        outlineRenderer.gameObject.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
        outlineRenderer.transform.SetParent(transform);
        outlineRenderer.transform.localPosition = Vector3.zero;
        outlineRenderer.transform.localScale += Vector3.one * 0.02f;
    }
    void Start()
    {
        outlineRenderer.material = Persistent.instance.MAT_PICKABLE_OUTLINE;
        beacon = Instantiate(Persistent.instance.PREFAB_PICKABLES_BEACON, transform);
        beacon.transform.localPosition = new Vector3(0, -0.5f, 0);
        beacon.SetActive(false);
        outlineRenderer.gameObject.SetActive(false);
    }

    public void SetHighlighted(bool _var)
    {
        if(_var != isHighlighted)
        {
            beacon.SetActive(_var);
            outlineRenderer.gameObject.SetActive(_var);
        }
        isHighlighted = _var;
    }
}
