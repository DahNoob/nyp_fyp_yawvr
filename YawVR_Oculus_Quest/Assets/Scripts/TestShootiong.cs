using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShootiong : MonoBehaviour
{
    public GameObject bulletPrefab;

    // Update is called once per frame
    void Update()
    {
        BaseProjectile derp = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.TEST_PROJECTILE, transform.position, transform.rotation).GetComponent<BaseProjectile>();
        //BaseProjectile derp = Instantiate(bulletPrefab, transform.position, transform.rotation, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
        derp.Init(transform);
    }

}
