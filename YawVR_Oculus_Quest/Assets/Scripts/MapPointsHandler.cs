using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapPointsHandler : MonoBehaviour
{
    public static MapPointsHandler instance;
    [Header("Unconfirmed Gizmos Configuration")]
    [SerializeField]
    private Vector3 m_cubeSizeUnconfirmed = new Vector3(1, 4, 1);
    [SerializeField]
    private Color m_colorUnconfirmed = new Color(0.0f, 1, 1, 0.75f);
    [Header("Confirmed Gizmos Configuration")]
    [SerializeField]
    private Vector3 m_cubeSizeConfirmed = new Vector3(0.75f, 6, 0.75f);
    [SerializeField]
    private Color m_colorConfirmed = new Color(0.8f, 0.8f, 0.3f, 0.75f);
    [SerializeField]
    private GUIStyle m_textStyle = new GUIStyle();

    [Header("Map Points Configuration")]
    [SerializeField]
    public List<Vector3> m_mapPoints;
    [SerializeField]
    public float m_pointHeight = 4.0f;

    [InspectorButton("OnResetSavedMapPoints", 175.0f)]
    public bool resetSavedMapPoints;

    [InspectorButton("OnSetPointsHeight", 150.0f)]
    public bool setPointsHeight;

    [InspectorButton("OnResetLabeling", 150.0f)]
    public bool resetLabeling;

    private void OnResetSavedMapPoints()
    {
        m_mapPoints.Clear();
        foreach (Transform t in transform)
        {
            m_mapPoints.Add(t.position);
        }
    }
    private void OnSetPointsHeight()
    {
        foreach (Transform t in transform)
        {
            RaycastHit hit;
            if (Physics.Raycast(t.position, -t.up, out hit))
            {
                t.position = hit.point;
                t.Translate(0, m_pointHeight, 0);
            }
        }
    }

    private void OnResetLabeling()
    {
        int index = 0;
        foreach (Transform t in transform)
        {
            t.name = string.Format("{0}{1}{2}", "(", index.ToString(), ")");
            ++index;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        print("MapPointsHandler Awake!");
    }

    void Start()
    {
#if !UNITY_EDITOR
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
#endif
        print("MapPointsHandler Start!");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = m_colorUnconfirmed;
        foreach (Transform t in transform)
        {
            Gizmos.DrawWireCube(t.position, m_cubeSizeUnconfirmed);
            //Gizmos.DrawIcon(t.position, t.name);
            UnityEditor.Handles.Label(t.position, t.name, m_textStyle);
        }
        Gizmos.color = m_colorConfirmed;
        for (int i = 0; i < m_mapPoints.Count; ++i)
        {
            Gizmos.DrawWireCube(m_mapPoints[i], m_cubeSizeConfirmed);
        }
    }

    public Vector3 GetClosestPoint(Vector3 _pos)
    {
        float dist = float.MaxValue;
        Vector3 point = Vector3.zero;
        for (int i = 0; i < m_mapPoints.Count; ++i)
        {
            float newDist = CustomUtility.HitCheckRadius(_pos, m_mapPoints[i]);
            if (newDist < dist)
            {
                dist = newDist;
                point = m_mapPoints[i];
            }
        }
        return point;
    }
}
