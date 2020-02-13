using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class provides variables for an objective.
/// </summary>
[System.Serializable]
public class ObjectiveInfo
{
    public VariedObjectives.TYPE type;//dis is cancerous but wutever
    public Transform m_highlight;
    public Vector3 m_mapPointPosition;
    public bool m_completed = false;
    public bool m_inProgress = false;
    public float m_timeLeft = 60;
    public float m_initialTime = 60;
    public float m_timer = 0;
    public float m_spawnTime = 7;
    // public UnityEngine.UI.Text m_panelUI;
    public ObjectivesGUIInfo panelInfo;
}

/// <summary>
/// This class provides GUI information for objectives so it can be updated.
/// </summary>
[System.Serializable]
public class ObjectivesGUIInfo
{
    public TextMeshProUGUI panelText;
    public TextMeshProUGUI panelProgress;
    public UnityEngine.UI.Image firstFill;
    public UnityEngine.UI.Image secondFill;
    public UnityEngine.UI.Image objectiveResult; //temporary solution
}