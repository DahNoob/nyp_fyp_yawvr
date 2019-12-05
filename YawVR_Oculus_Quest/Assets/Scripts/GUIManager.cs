using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_cameraTransform;

    [Header("Experimental Resources")]
    [SerializeField]
    private GameObject m_dumbCubes;

    void Start()
    {
        transform.position = m_cameraTransform.position;
        transform.localEulerAngles = new Vector3(0, m_cameraTransform.rotation.y, 0);
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);
    }
    
    void Update()
    {
        transform.position = m_cameraTransform.position;
        transform.localEulerAngles = new Vector3(0, m_cameraTransform.rotation.y, 0);
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);
    }

    public void SpawnCubes()
    {
        Instantiate(m_dumbCubes).transform.SetParent(GameObject.Find("CubePile").transform);
    }
    public void ClearCubes()
    {
        foreach (Transform item in GameObject.Find("CubePile").transform)
        {
            Destroy(item.gameObject);
        }
    }
    public void RecenterPose()
    {
        OVRManager.display.RecenterPose();
    }
}
