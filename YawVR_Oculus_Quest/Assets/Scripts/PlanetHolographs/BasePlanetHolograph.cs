using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BasePlanetHolograph : MonoBehaviour
{
    [Header("Base Configuration")]
    [SerializeField]
    protected GameObject m_holographPrefab;

    protected Transform holographRoot;
}
