using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillCanvas : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private float distFromCamera;

    private RectTransform rectTransform;

    [SerializeField]
    private Vector2 scaleOffset;

    // Start is called before the first frame update
    void Start()
    {
        //Append the UI to the main canvas itself
        if (camera == null)
            camera = Camera.main;

        rectTransform = GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        float position = camera.nearClipPlane + distFromCamera;
        Vector3 finalPos = camera.transform.position + camera.transform.forward * position;

        //Make it look at the camera
        transform.LookAt(camera.transform);

        float h = (Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * position * 2f);

        Vector3 localScale = new Vector3(h * camera.aspect, 1.0f, h);

        rectTransform.sizeDelta = new Vector2(localScale.x + scaleOffset.x, localScale.z + scaleOffset.y);

        rectTransform.position = finalPos;

        //Get child and set, hacky solution
        //for(int i =0; i < )

 



    }

    void UpdateChildren()
    {
        
    }
}
