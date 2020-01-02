using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_Buff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponentInParent<EnemyBase>().SetHealth(gameObject.GetComponentInParent<EnemyBase>().GetHealth() * 1.5f);
        gameObject.GetComponentInParent<EnemyBase>().SetMaxHealth(gameObject.GetComponentInParent<EnemyBase>().GetMaxHealth() * 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
