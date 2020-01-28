using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
