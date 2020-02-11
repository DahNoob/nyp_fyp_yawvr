using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldPickable : MonoBehaviour
{
    public delegate void Selected();
    public event Selected onSelected;
    private MeshRenderer outlineRenderer;
    private GameObject beacon;
    [SerializeField]
    private TextMeshProUGUI m_textUi;
    [SerializeField]
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
        //beacon.SetActive(false);
        outlineRenderer.gameObject.SetActive(false);
        m_textUi.gameObject.SetActive(false);
    }

    void Update()
    {
        if(isHighlighted)
        {
            Vector2 asd = CustomUtility.ToVector2(Camera.main.transform.position - transform.position);
            m_textUi.transform.eulerAngles = new Vector3(0, Mathf.Atan2(-asd.x, -asd.y) * Mathf.Rad2Deg, 0);
        }
    }

    public void SetHighlighted(bool _var)
    {
        if(_var != isHighlighted)
        {
            //beacon.SetActive(_var);
            //outlineRenderer.gameObject.SetActive(_var);
            m_textUi.gameObject.SetActive(_var);
        }
        isHighlighted = _var;
    }

    public void TriggerSelect()
    {
        onSelected?.Invoke();
    }
}
