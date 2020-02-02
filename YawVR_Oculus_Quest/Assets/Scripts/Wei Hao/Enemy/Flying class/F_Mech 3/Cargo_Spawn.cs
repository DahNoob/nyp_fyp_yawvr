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

    protected bool startTimer = false;
    protected bool stopSpawn = false;

    Material m_Material;
    protected float alphaVal = 1.0f;

    protected float animationTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            //spawnTimer -= 1.0f * Time.deltaTime;
            if (spawnTimer <= 0.0f && !stopSpawn)
            {
                SpawnEnemy();
                stopSpawn = true;
            }

            alphaVal -= 1.0f * Time.deltaTime;
            m_Material.SetFloat("_Amount", alphaVal);

            if(m_Material.GetFloat("_Amount") <= -2.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(m_lesserEnemy, spawnPoint_1.transform.position, transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
        GameObject newEnemy2 = Instantiate(m_lesserEnemy, spawnPoint_2.transform.position, transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
        //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        startTimer = true;
        //gameObject.GetComponent<BoxCollider>().enabled = false;
        //Destroy(gameObject.GetComponent<Rigidbody>());
        //gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        if (!stopSpawn)
        {
            SpawnEnemy();
            stopSpawn = true;
            StartCoroutine(DestroyCargo());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("ayy");
    }

    private IEnumerator DestroyCargo()
    {
        yield return new WaitForSeconds(animationTime);
        Destroy(gameObject);
    }
}
