using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class PlayerHandler : MonoBehaviour
{
    [Header("Hands")]
    public GameObject rightHand;
    public GameObject leftHand;

    [Header("Debug")]
    [SerializeField]
    private GameObject dumbCubesPrefab;
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        //if (OVRInput.GetDown(OVRInput.Button.One))
        //{
        //    OVRManager.display.RecenterPose();
        //}
        //if (OVRInput.GetDown(OVRInput.Button.Three))
        //{
        //    Instantiate(dumbCubesPrefab).transform.SetParent(GameObject.Find("CubePile").transform);
        //}
        //if(OVRInput.GetDown(OVRInput.Button.Four))
        //{
        //    foreach (Transform item in GameObject.Find("CubePile").transform)
        //    {
        //        Destroy(item.gameObject);
        //    }
        //}
    }
}
