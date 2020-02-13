using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for planet holographs in the MainHub scene.
/// </summary>
abstract public class BasePlanetHolograph : MonoBehaviour
{
    [Header("Base Configuration")]
    [SerializeField]
    [Tooltip("The name of the scene file.")]
    public string m_sceneName;
    [SerializeField]
    [Tooltip("The planet's name to be shown in-game.")]
    public string m_planetName;
    [SerializeField]
    protected Transform m_holographRoot;
    [SerializeField]
    public Color m_holographColor;
    [SerializeField]
    [Range(0.0f, 10.0f)]
    protected float m_bobSpeed = 1;
    [SerializeField]
    [Range(0.0f, 0.25f)]
    protected float m_bobDistance = 0.1f;
    [SerializeField]
    protected float m_rotationSpeed = 22.5f;

    protected MeshRenderer[] meshes;

    virtual protected void Awake()
    {
        meshes = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; ++i)
        {
            meshes[i].material.SetColor("_RimColor", m_holographColor);
        }
    }

    virtual protected void Update()
    {
        m_holographRoot.localPosition = new Vector3(0, Mathf.Cos(Time.time * m_bobSpeed) * m_bobDistance, 0);
        m_holographRoot.Rotate(0, m_rotationSpeed * Time.deltaTime, 0);
    }
}
