using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Player Minimap")]
    [SerializeField]
    [Tooltip("Player's Minimap Component")]
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
        if (Input.GetKeyDown(KeyCode.T))
            EdgeDetectionController.instance.AddWaveButton();

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
