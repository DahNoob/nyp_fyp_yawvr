using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIReticleHandler
{
    public static UIReticleHandler instance { private set; get; }

    [SerializeField]
    private bool m_componentEnabled;
    [SerializeField]
    private GUIReticleModule reticleModule;

    public void Awake()
    {
        if (!m_componentEnabled)
            return;

        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        if (!m_componentEnabled)
            return;

        reticleModule.SetupReticleModule();
        reticleModule.SetupReticleColors();


        //Disable both reticles
        EnableReticle(OVRInput.Controller.LTouch, false);
        EnableReticle(OVRInput.Controller.RTouch, false);
    }

    // Update is called once per frame
    public void Update()
    {
        if (!m_componentEnabled)
            return;

        reticleModule.UpdateEase();
    }

    public void SetReticleInformation(OVRInput.Controller _controller, Vector3 hitPoint, GameObject hitObject, bool useTag = true)
    {
        //Set the reticle position based on raycasted position
        SetReticlePosition(_controller, hitPoint);
        //Set hit object name by accessing the object
        //SetHitObjectName(_controller, hitObject);
        //Set the hit color by accessing the layer of the object
        SetReticleColor(_controller, hitObject, useTag);
        //Scale the reticle to be correct scale
        ScaleReticle(_controller);

        if (hitObject != null)
        {
            GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;
            //Temporary code
            reticleConfig.ObjectOfInterest(hitObject.tag == "Enemy");
        }
    }

    public void SetReticlePosition(OVRInput.Controller _controller, Vector3 _worldPosition)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        reticleConfig.reticleReference.transform.position = _worldPosition;
        reticleConfig.reticleReference.transform.LookAt(Camera.main.transform);
        reticleConfig.reticleReference.transform.Rotate(0, 180, 0);

        ScaleReticle(_controller);
    }

    public void SetHitObjectName(OVRInput.Controller _controller, GameObject hitObject)
    {
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        reticleConfig.reticleText.text = hitObject == null ? "N/A" : hitObject.name;
    }

    public void SetReticleColor(OVRInput.Controller _controller, GameObject hitObject, bool useTag)
    {
        //Access the reticle
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        if (hitObject == null)
        {
            reticleConfig.SetReticleDefaultColor();
            return;
        }

        //Access the color config
        GUIReticleColorConfig colorConfig = reticleModule.ReticleColors;

        //If the existing result is contained
        bool containsResult = useTag ? colorConfig.ContainsTag(hitObject.tag) : colorConfig.ContainsLayer(hitObject.layer);

        if (containsResult)
        {
            //If the result is contained
            Color resultantColor = useTag ? colorConfig.QueryTagColor(hitObject.tag) : colorConfig.QueryLayerColor(hitObject.layer);
            reticleConfig.SetReticleColor(resultantColor);
        }
        else
        {
            reticleConfig.SetReticleDefaultColor();
        }
    }

    //Scaling the reticle to make its same size no matter what
    public void ScaleReticle(OVRInput.Controller _controller)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        //Distance between camera and the reticle
        float pos = Vector3.Distance(Camera.main.transform.position, reticleConfig.reticleReference.transform.position);

        //Some scaling formula from my other fill stuff
        float h = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;
        reticleConfig.reticleReference.transform.localScale = new Vector3(h, h, h) * (reticleConfig.reticleSize * 0.01f);
    }

    public void EnableReticle(OVRInput.Controller _controller, bool enabled)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;
        reticleConfig.reticleReference.SetActive(enabled);
    }
}
