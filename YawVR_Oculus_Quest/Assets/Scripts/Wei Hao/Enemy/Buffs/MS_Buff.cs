using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MS_Buff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponentInParent<EnemyBase>().SetMoveSpeed(gameObject.GetComponentInParent<EnemyBase>().GetSpeed() * 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
