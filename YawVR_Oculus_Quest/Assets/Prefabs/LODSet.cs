using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODSet : MonoBehaviour
{
    [SerializeField]
    private Material[] lodMats;

    private GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        //Not really the best way I suppose but okay.
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
