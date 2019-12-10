using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Edge Detection Controller
** Desc: A class that controls the edge detection wave shader and calculations. (if there is a use it will stay)
** Author: Isaac
** Date: 10/12/2019, 11.31AM
**************************
** Change History
**************************
** PR   Date                            Author    Description 
** --   --------                                -------      ------------------------------------
** 1    10/12/2019, 11.31AM  Isaac   Created
** 2    10/12/2019, 11:42PM   Isaac   Implemented draft functions and variables
** 3    10/12/2019, 3.17PM     Isaac   Completed class and shaders
*******************************/
public class EdgeDetectionController : MonoBehaviour
{
    //Where to start the scan.
    [SerializeField]
    private Transform scanOrigin;
    //Edge detection material
    [SerializeField]
    public Material edgeDetectionMaterial;
    //Scan distance for the edge detection.
    [SerializeField]
    private float startScanDistance;
    [SerializeField]
    private float scanWidth;
    [SerializeField]
    private float scanSpeed;

    //Get camera
    private Camera m_camera;
    private float scanDistance;

    //Is the scan happening?
    bool isScanning;

    //   // Use this for initialization
    //   void Start () {

    //}
    private void OnEnable()
    {
        m_camera = GetComponent<Camera>();
        if (m_camera == null)
            m_camera = Camera.main;

        //Set depth texture mode to generate a depth map
        m_camera.depthTextureMode = DepthTextureMode.Depth;

        scanDistance = startScanDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            isScanning = true;
            scanDistance = startScanDistance;
        }
        if (isScanning)
        {
            scanDistance += Time.deltaTime * scanSpeed;
        }
    }

    //OnRenderImage callback
    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        edgeDetectionMaterial.SetVector("_ScanOrigin", scanOrigin.position);
        edgeDetectionMaterial.SetFloat("_ScanDistance", scanDistance);
        edgeDetectionMaterial.SetFloat("_ScanWidth", scanWidth);
        RaycastDepthToWorld(source, destination);
    }

    void RaycastDepthToWorld(RenderTexture source, RenderTexture destination)
    {
        //Get camera values
        float m_farPlane = m_camera.farClipPlane;
        float m_fov = m_camera.fieldOfView;
        float m_camAspect = m_camera.aspect;
        float m_nearPlane = m_camera.nearClipPlane;

        //Half FOV
        float m_halfFOV = m_fov * 0.5f;

        //Get top right coordinates of camera
        Vector3 width = m_camera.transform.right * Mathf.Tan(m_halfFOV * Mathf.Deg2Rad) * m_camAspect;
        Vector3 height = m_camera.transform.up * Mathf.Tan(m_halfFOV * Mathf.Deg2Rad);

        //Get all four sides of frustrum in world space.

        //- 1 1
        Vector3 TL = (m_camera.transform.forward - width + height);
        // 1  1
        Vector3 TR = (m_camera.transform.forward + width + height);     
        // -1 -1
        Vector3 BL = (m_camera.transform.forward - width - height);
        //1  -1
        Vector3 BR = (m_camera.transform.forward + width - height);

        //Normalize all and * scale
        TL.Normalize();
        TR.Normalize();
        BL.Normalize();
        BR.Normalize();

        //This is the scale between 0 and far plane when normalized.
        float m_scale = TL.magnitude * m_farPlane;

        TL *= m_scale;
        TR *= m_scale;
        BL *= m_scale;
        BR *= m_scale;

        //Set the render texture to the destination texture
        RenderTexture.active = destination;

        //Set some stuff in the material.
        edgeDetectionMaterial.SetTexture("_MainTex", source);

        //This is where i kind of get confused...
        GL.PushMatrix();
        GL.LoadOrtho();

        edgeDetectionMaterial.SetPass(0);

        //Screen coordinates
        //0,1---------1,1
        //|               |
        //0,0-------- 1,0

        //Draw a quad over whole screen
        GL.Begin(GL.QUADS);

        //Anti clockwise
        //Bottom left for unit 0
        GL.MultiTexCoord2(0, 0f, 0f);
        GL.MultiTexCoord(1, BL);
        GL.Vertex3(0f, 0f, 0f);

        //Bottom Right Unit 0
        GL.MultiTexCoord2(0, 1.0f, 0f);
        GL.MultiTexCoord(1, BR);
        GL.Vertex3(1.0f, 0f, 0f);

        //Top Right Unit 0
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, TR);
        GL.Vertex3(1.0f, 1.0f, 0f);

        //Top Left Unit 0
        GL.MultiTexCoord2(0, 0f, 1f);
        GL.MultiTexCoord(1, TL);
        GL.Vertex3(0f, 1.0f, 0f);

        GL.End();
        GL.PopMatrix();
    }

}
