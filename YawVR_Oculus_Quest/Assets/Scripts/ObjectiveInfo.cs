using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[System.Serializable]
public class ObjectiveInfo
{
    public VariedObjectives.TYPE type;//dis is cancerous but wutever
    public Transform m_highlight;
    public bool m_completed = false;
    public bool m_inProgress = false;
    public float m_timeLeft = 30;
    public float m_initialTime = 30;
    public float m_timer = 0;
    public float m_spawnTime = 7;
   // public UnityEngine.UI.Text m_panelUI;
    public TextMeshProUGUI m_panelUI;
}

[System.Serializable]
public class ObjectivesGUIInfo
{
    public TextMeshProUGUI panelText;
    public UnityEngine.UI.Image panelFirstFill;
    public UnityEngine.UI.Image panelSecondFill;
    public TextMeshProUGUI panelProgress;
}