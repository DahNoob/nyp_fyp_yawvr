using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXHandler : MonoBehaviour , IPooledObject
{
    public float m_time = 1;

    public void OnObjectSpawn()
    {
        Start();
    }

     public void OnObjectDestroy()
    {
        this.gameObject.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(m_time);
        OnObjectDestroy();
    }
}
