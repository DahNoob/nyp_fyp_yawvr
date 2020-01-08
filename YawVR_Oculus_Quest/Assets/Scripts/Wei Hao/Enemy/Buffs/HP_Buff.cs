using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_Buff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EnemyBase en = GetComponentInParent<EnemyBase>();
        float prevRatio = (float)en.GetHealth() / en.GetMaxHealth();
        en.SetMaxHealthMultiplier(1.5f);
        en.SetHealth((int)(prevRatio * en.GetMaxHealth()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
