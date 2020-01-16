using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolObject
{
    //Tag reference
    public string poolName;
    //Prefab for the pool
    public GameObject objToPool;
    //Amount to pool
    public int amountToPool;
    //How much to increase it by each time
    [Range(1, 5)]
    public int amountToExpand = 1;

    //Hierachy stuff
    public GameObject parentInHierachy;

    public PoolObject(string _poolName, GameObject _objToPool, int _amountToPool, int _amountToExpand)
    {
        poolName = _poolName;
        objToPool = _objToPool;
        amountToPool = _amountToPool;
        amountToExpand = _amountToExpand;
    }
}

public class ObjectPooler : MonoBehaviour
{
    //Instance object
    public static ObjectPooler instance;
    //Dictionary to store the queues
    private Dictionary<string, List<GameObject>> poolDictionary = new Dictionary<string, List<GameObject>>();
    //Another dictionary to store data for future instancing
    private Dictionary<string, PoolObject> pooledObjectData = new Dictionary<string, PoolObject>();
    //List of objects to instantiate
    [SerializeField]
    private List<PoolObject> poolList;

    //For a nice hierachy
    private GameObject poolParent;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        poolParent = new GameObject("Object Pools");
        //poolParent.transform.parent = Persistent.instance.GO_DYNAMIC.transform;
        poolParent.transform.parent = transform;
        foreach (PoolObject poolObject in poolList)
        {
            List<GameObject> resultPool = new List<GameObject>();
            //Make new object with name under this transform
            GameObject parent = new GameObject(poolObject.poolName + " Pool");
            parent.transform.parent = poolParent.transform;

            for (int i = 0; i < poolObject.amountToPool; ++i)
            {
                GameObject resultObject = Instantiate(poolObject.objToPool, parent.transform);
                resultObject.SetActive(false);
                resultPool.Add(resultObject);
            }

            //MAke sure the reference to the gameObject parent is saved
            poolObject.parentInHierachy = parent;

            poolDictionary.Add(poolObject.poolName, resultPool);
            pooledObjectData.Add(poolObject.poolName, poolObject);
        }
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("The dictionary does not contain key " + tag);
            return null;
        }

        //Only if object is active then we swap, don't want to kinda teleport stuff around
        for (int i = 0; i < poolDictionary[tag].Count; ++i)
        {
            GameObject resultObject = poolDictionary[tag][i];
            if (!resultObject.activeSelf)
            {
                resultObject.transform.position = position;
                resultObject.transform.rotation = rotation;

                resultObject.SetActive(true);

                IPooledObject pooledObject = resultObject.GetComponent<IPooledObject>();
                if (pooledObject != null)
                    pooledObject.OnObjectSpawn();


                return resultObject;
            }
        }

        //Else we instantiate more and add poggers

        //Get pool data
        PoolObject objectData = pooledObjectData[tag];

        int newCount = poolDictionary[tag].Count;

        //Gonna add more badboys in
        for (int i = 0; i < objectData.amountToExpand; ++i)
        {
            GameObject resultObject = Instantiate(objectData.objToPool, objectData.parentInHierachy.transform);

            resultObject.transform.position = position;
            resultObject.transform.rotation = rotation;

            resultObject.SetActive(false);
            poolDictionary[tag].Add(resultObject);
        }
        //Debug.Log("Expanded pool by " + objectData.amountToExpand);
        poolDictionary[tag][newCount].SetActive(true);
        return poolDictionary[tag][newCount];

    }



    /// <summary> NOT TESTED FUNCTION WILL PROBABLY BREAK <
    /// Adds another pool of objects during runtime, might cause like performance loss, instantiate as less as possible at any given time
    /// </summary>
    /// <param name="thatObject">poolObject that will be added</param>
    /// <param name="overWrite">force override any given pool</param>
    /// <returns>false if failed to add, true if it has added</returns>
    public bool AddAnotherPool(PoolObject thatObject, bool overWrite = false)
    {
        if (thatObject == null)
            return false;

        if (poolDictionary.ContainsKey(thatObject.poolName))
        {
            if (!overWrite)
                return false;

            List<GameObject> newList = new List<GameObject>();

            for (int i = 0; i < thatObject.amountToPool; ++i)
            {
                GameObject resultObject = Instantiate(thatObject.objToPool, thatObject.parentInHierachy.transform);
                resultObject.SetActive(false);
                newList.Add(resultObject);
            }

            //Idk maybe safety, not sure just in case?
            poolDictionary[thatObject.poolName].Clear();
            //Copy the list into the old list
            poolDictionary[thatObject.poolName] = new List<GameObject>(newList);

            return true;
        }
            
        List<GameObject> resultPool = new List<GameObject>();
        //Make new object with name under this transform
        GameObject parent = new GameObject(thatObject.poolName + " Pool");
        parent.transform.parent = poolParent.transform;

        for (int i = 0; i < thatObject.amountToPool; ++i)
        {
            GameObject resultObject = Instantiate(thatObject.objToPool, parent.transform);
            resultObject.SetActive(false);
            resultPool.Add(resultObject);
        }

        //MAke sure the reference to the gameObject parent is saved
        thatObject.parentInHierachy = parent;

        poolDictionary.Add(thatObject.poolName, resultPool);
        pooledObjectData.Add(thatObject.poolName, thatObject);


        return true;
    }

    public bool AddAnotherPool(string poolName, GameObject objectToPool, int amountToPool, int amountToExpand , bool overWrite = false)
    {
        PoolObject resultObject = new PoolObject(poolName, objectToPool, amountToPool, amountToPool);
        return AddAnotherPool(resultObject, overWrite); 
    }

    public string AmountActive(string tag)
    {
        if (poolDictionary == null || poolDictionary[tag] == null)
            return "";

        int totalCount = poolDictionary[tag].Count;

        int totalActive = 0;
        for(int i =0; i < poolDictionary[tag].Count; ++i)
        {
            if (poolDictionary[tag][i].activeSelf)
                totalActive++;
        }

        return tag + " Pool: " + totalActive + "/" + totalCount;

    }
}
