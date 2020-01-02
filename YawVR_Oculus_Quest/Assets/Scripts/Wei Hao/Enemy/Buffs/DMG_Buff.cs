using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMG_Buff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponentInParent<EnemyBase>().SetDamage(gameObject.GetComponentInParent<EnemyBase>().GetDamage() * 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
