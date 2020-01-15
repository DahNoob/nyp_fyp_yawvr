using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo_Spawn : MonoBehaviour
{
    [SerializeField]
    protected float spawnTimer;
    [SerializeField]
    protected GameObject m_lesserEnemy;
    [SerializeField]
    protected GameObject spawnPoint_1;
    [SerializeField]
    protected GameObject spawnPoint_2;
    protected bool startTimer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            spawnTimer -= 1.0f * Time.deltaTime;
            if (spawnTimer <= 0.0f && startTimer == true)
            {
                SpawnEnemy();
                startTimer = false;
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(m_lesserEnemy, spawnPoint_1.transform.position, transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
        GameObject newEnemy2 = Instantiate(m_lesserEnemy, spawnPoint_2.transform.position, transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        startTimer = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("ayy");
    }
}
