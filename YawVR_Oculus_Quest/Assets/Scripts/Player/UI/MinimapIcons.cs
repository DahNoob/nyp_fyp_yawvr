using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcons : MonoBehaviour
{
    [SerializeField]
    private PlayerUIMinimapIcons.MINIMAP_ICONS m_iconType;

    public PlayerUIMinimapIcons.MINIMAP_ICONS iconType
    {
        get { return m_iconType; } private set { }
    }

    private void Start()
    {
        //if (PlayerUIManager.instance != null)
        //    PlayerUIManager.instance.playerMinimapIcons.UpdateSprite(this);
    }
}
