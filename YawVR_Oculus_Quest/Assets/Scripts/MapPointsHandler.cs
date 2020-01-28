using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapPointsHandler : MonoBehaviour
{
    public static MapPointsHandler instance { private set; get; }

    //Classes
    [System.Serializable]
    public struct ObjectiveInfo
    {
        public enum TYPE
        {
            BOUNTYHUNT,

            TOTAL
        }

        public TYPE type;

    }


    [Header("General Configuration")]
    [SerializeField]
    private bool m_showGizmos = false;
    [SerializeField]
    public float m_pointHeight = 4.0f;
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
    [Header("Objective Gizmos Configuration")]
    [SerializeField]
    private float m_radiusSizeObjective = 10.0f;
    [SerializeField]
    private Color m_colorObjective = new Color(1, 1, 1, 0.75f);

    [Header("Map Points Configuration")]
    [SerializeField]
    public List<Vector3> m_mapPoints;
    //[SerializeField]
    //public int[] m_possibleObjectivePoints;

    [Header("Objectives Configuration")]
    [SerializeField]
    public VariedObjectives m_variedObjectives;

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
        //foreach (Transform t in transform)
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(t.position, -t.up, out hit))
        //    {
        //        t.position = hit.point;
        //        t.Translate(0, m_pointHeight, 0);
        //    }
        //}
        for (int i = 0; i < m_mapPoints.Count; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_mapPoints[i], -Vector3.up, out hit))
            {
                m_mapPoints[i] = hit.point + new Vector3(0, m_pointHeight);
            }
        }
    }

    private void OnResetLabeling()
    {
        int index = 0;
        foreach (Transform t in transform)
        {
            t.name = string.Format("{0}{1}{2}", " (", index.ToString(), ")");
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
        
        m_showGizmos = false;
#endif
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        for (int i = 0; i < m_mapPoints.Count; ++i)
        {
            GameObject ayyy = new GameObject();
            ayyy.name = string.Format("{0}{1}{2}", " (", i.ToString(), ")");
            ayyy.transform.SetParent(transform);
            ayyy.transform.position = m_mapPoints[i];
            ayyy.AddComponent<MapPointObject>();

        }
        print("MapPointsHandler Start!");
    }

    void OnDrawGizmos()
    {
        if (!m_showGizmos) return;
        Gizmos.color = m_colorUnconfirmed;
        foreach (Transform t in transform)
        {
            Gizmos.DrawWireCube(t.position, m_cubeSizeUnconfirmed);
            //Gizmos.DrawIcon(t.position, t.name);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(t.position, t.name, m_textStyle);
#endif
        }
        Gizmos.color = m_colorConfirmed;
        for (int i = 0; i < m_mapPoints.Count; ++i)
        {
            Gizmos.DrawWireCube(m_mapPoints[i], m_cubeSizeConfirmed);
        }
        Gizmos.color = m_colorObjective;
        for (int i = 0; i < m_variedObjectives.possibleObjectivePoints.Length; ++i)
        {
            Gizmos.DrawWireSphere(m_mapPoints[m_variedObjectives.possibleObjectivePoints[i]], m_radiusSizeObjective);
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


    public Vector3 GetClosestPoint(QuadRect queryBounds)
    {
        List<GameObject> nearbyMapPoints = QuadTreeManager.instance.QueryStaticObjects(queryBounds, QuadTreeManager.STATIC_TYPES.MAP_POINTS);
        float dist = float.MaxValue;
        Vector3 point = Vector3.zero;
        for (int i = 0; i < nearbyMapPoints.Count; ++i)
        {
            float newDist = CustomUtility.HitCheckRadius(queryBounds.position, nearbyMapPoints[i].transform.position);
            if (newDist < dist)
            {
                dist = newDist;
                point = nearbyMapPoints[i].transform.position;
            }
        }
        return point;
    }
}
