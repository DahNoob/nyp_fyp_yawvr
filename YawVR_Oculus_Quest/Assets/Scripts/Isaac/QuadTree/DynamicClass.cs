using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicClass :  DynamicQuadTreeObject
{
    public Vector3 randomDirection;
    // Start is called before the first frame update
    void Start()
    {
        AddToQuadTree(this.gameObject);
        randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += randomDirection * 0.15f;
    }
}
