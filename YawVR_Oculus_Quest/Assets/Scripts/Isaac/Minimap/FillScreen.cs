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
        if (cam == null)
            cam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        float pos = (cam.nearClipPlane + distFromCamera);

        transform.position = cam.transform.position + cam.transform.forward * pos;
        transform.LookAt(cam.transform);
        transform.Rotate(90.0f, 0.0f, 0.0f);

        float h = (Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;

        transform.localScale = new Vector3(h * cam.aspect, 1.0f, h);

    }
}
