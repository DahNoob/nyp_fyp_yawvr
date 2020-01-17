using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    //Honestly liek idk what is this tbh liek if i have time i will get back to this but...

    public static PlayerUIManager instance;

    [Header("Player Minimap")]
    [SerializeField]
    [Tooltip("Player's Minimap Component")]
    private PlayerUIMinimap m_playerMinimap;

    [Header("Minimap Icon Configuration")]
    [SerializeField]
    [Tooltip("Player Minimap Icons Configuration")]
    private PlayerUIMinimapIcons m_playerMinimapIcons;

    //Get stuff
    public PlayerUIMinimap playerMinimap
    {
        get { return m_playerMinimap; } private set { }
    }
    //Get stuff
    public PlayerUIMinimapIcons playerMinimapIcons
    {
        get { return m_playerMinimapIcons; }
        private set { }
    }


    private void Awake()
    {
        if (instance == null)
            instance = this;

        m_playerMinimapIcons.Awake();
        //Check null functio nfor the icons
        StartCoroutine(m_playerMinimapIcons.CheckNull());
    }

    // Start is called before the first frame update
    void Start()
    {
        m_playerMinimap.Start();

    }
    // Update is called once per frame
    void Update()
    {
        m_playerMinimap.Update();
        m_playerMinimapIcons.Update();
        //if (Input.GetKeyDown(KeyCode.R))
        //    EdgeDetectionController.instance.AddWave(
        //        PlayerHandler.instance.transform.position,
        //        2,
        //        0,
        //        100,
        //        25,
        //        250,
        //        5,
        //       GenerateRandomColor(),
        //       GenerateRandomColor(),
        //       GenerateRandomColor(),
        //      GenerateRandomColor());
        //if (Input.GetKeyDown(KeyCode.T))
        //    EdgeDetectionController.instance.AddWaveButton();

    }

    //public Color GenerateRandomColor()
    //{
    //    return new Color(
    //        Random.Range(0f, 1f),
    //        Random.Range(0f, 1f),
    //        Random.Range(0f, 1f),
    //        1);
    //}
}
