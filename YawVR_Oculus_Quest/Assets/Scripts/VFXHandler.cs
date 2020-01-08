using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXHandler : MonoBehaviour
{
    public float m_time = 1;

    void Start()
    {
        StartCoroutine(destroy());
    }

    IEnumerator destroy()
    {
        yield return new WaitForSeconds(m_time);
        Destroy(gameObject);
    }
}
