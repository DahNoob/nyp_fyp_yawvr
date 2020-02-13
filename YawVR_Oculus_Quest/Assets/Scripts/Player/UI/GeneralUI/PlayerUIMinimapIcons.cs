using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides information of how a minimap icon should behave or look.
/// </summary>
[System.Serializable]
public class PlayerUIMinimapIcons 
{
    //For sorting in the editor, and for some reference
    public string dataName;
    //Pair of iconss
    public EnemyInfo.ENEMY_TYPE enemyType;

    //Sprite stuff for the appropriate stuff
    public Sprite m_circleSprite;
    public Sprite m_iconSprite;

    //Colors
    public Color lineRendererStartColor;
    public Color lineRendererEndColor;
    public Color m_circleTint;
    public Color m_iconTint;

    //Scale stuff
    public float scaleOffset;

}

/// <summary>
/// This class provides information of how a minimap icon for objectives should behave or look.
/// </summary>
[System.Serializable]
public class PlayerUIMinimapObjectiveIcons
{
    //For sorting in the editor, and for some reference
    public string dataName;
    //Pair of iconss
    public VariedObjectives.TYPE objectiveType;

    //Sprite stuff for the appropriate stuff
    public Sprite m_circleSprite;
    public Sprite m_iconSprite;

    //Colors
    public Color lineRendererStartColor;
    public Color lineRendererEndColor;
    public Color m_circleTint;
    public Color m_iconTint;

    //Scale stuff
    public float scaleOffset;
}