using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectiveInfo
{
    public VariedObjectives.TYPE type;//dis is cancerous but wutever
    public Transform m_highlight;
    public bool m_completed = false;
    public bool m_inProgress = false;
    public float m_timeLeft = 30;
    public float m_timer = 0;
    public float m_spawnTime = 7;
    public UnityEngine.UI.Text m_panelUI;
}