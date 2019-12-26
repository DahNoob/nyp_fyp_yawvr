using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldModuleTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 10000);

        if (Input.GetMouseButton(0))
        {
         
            if (Physics.Raycast(ray, out hit, float.MaxValue,LayerMask.GetMask("PlaneTest")))
            {
                ShieldModule shieldModule = hit.collider.gameObject.GetComponent<ShieldModule>();
                if(shieldModule != null)
                {
                    shieldModule.AddHit(hit.point);
                    shieldModule.TakeDamage(1);
                }

            }
        }
    }
}
