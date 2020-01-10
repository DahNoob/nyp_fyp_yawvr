using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Player Minimap")]
    [SerializeField] [Tooltip("Player's Minimap Component")]
    private PlayerUIMinimap m_playerMinimap;



    // Start is called before the first frame update
    void Start()
    {
        m_playerMinimap.Start();
    }

    // Update is called once per frame
    void Update()
    {
        m_playerMinimap.Update();
    }
}
