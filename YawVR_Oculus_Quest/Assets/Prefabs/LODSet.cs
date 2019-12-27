using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODSet : MonoBehaviour
{
    public Material[] lodMats;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        target = transform.GetChild(0).gameObject;
        for(int i =0; i < target.transform.childCount; i++)
        {
            Transform t = target.transform.GetChild(i);
            t.gameObject.GetComponent<MeshRenderer>().material = lodMats[i];
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
