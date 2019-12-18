using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillScreen : MonoBehaviour
{
    //The camera to be used as a reference
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float distFromCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
            float pos = (cam.nearClipPlane + distFromCamera);

            transform.position = cam.transform.position + cam.transform.forward * pos;

            float h = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;

            transform.localScale = new Vector3(h * cam.aspect, h, 0f);
        
    }
}
