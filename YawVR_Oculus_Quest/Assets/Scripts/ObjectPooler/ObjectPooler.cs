using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolObject
{
    //For visualization.
    public string poolName;
    //Tag reference
    public OBJECTTYPES poolType;
    //Prefab for the pool
    public GameObject objToPool;
    //Amount to pool
    public int amountToPool;
    //How much to increase it by each time
    [Range(1, 15)]
    public int amountToExpand = 1;

    //Hierachy stuff
    public GameObject parentInHierachy;

    public int amountActiveInPool;

    //Public enums
    public enum OBJECTTYPES
    {
        PLAYER_PROJECTILE,
        ENEMY_PROJECTILE,
        PLAYER_PROJECTILE_IMPACT,
        ENEMY_DEATH_EFFECT,
        LIGHT_MECH2,
        HEAVY_MECH2,
        LIGHT_MECH1,
        MINIMAP_ICONS,
        BULLET_IMPACT_B_EFFECT,
        PLAYER_PROJECTILE_BIG,
        PLAYER_PROJECTILE_BIGGER,
        TOTAL_TYPES
    }

    public PoolObject(OBJECTTYPES types, GameObject _objToPool, int _amountToPool, int _amountToExpand)
    {
        poolType = types;
        objToPool = _objToPool;
        amountToPool = _amountToPool;
        amountToExpand = _amountToExpand;
        amountActiveInPool = 0;
    }
}

public class ObjectPooler : MonoBehaviour
{
    //Instance object
    public static ObjectPooler instance;
  
    //Dictionary to store the queues
    private  Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    //Another dictionary to store data for future instancing
    private Dictionary<int, PoolObject> pooledObjectData = new Dictionary<int, PoolObject>();
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
        //int totalCount = 0;
        foreach (PoolObject poolObject in poolList)
        {
            Queue<GameObject> resultPool = new Queue<GameObject>();
            //Make new object with name under this transform
            GameObject parent = new GameObject(poolObject.poolName + " Pool");
            parent.transform.parent = poolParent.transform;

            for (int i = 0; i < poolObject.amountToPool; ++i)
            {
                GameObject resultObject = Instantiate(poolObject.objToPool, parent.transform);
                resultObject.SetActive(false);
                resultPool.Enqueue(resultObject);
            }

            //MAke sure the reference to the gameObject parent is saved
            poolObject.parentInHierachy = parent;

            poolDictionary.Add((int)poolObject.poolType, resultPool);
            pooledObjectData.Add((int)poolObject.poolType, poolObject);

            //totalCount++;
        }
    }

    public GameObject SpawnFromPool(PoolObject.OBJECTTYPES objectType, Vector3 position, Quaternion rotation)
    {
        int tag = (int)(objectType);

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("The dictionary does not contain key " + tag);
            return null;
        }

        //The current queue
        Queue<GameObject> currentQueue = poolDictionary[tag];
        PoolObject currentData = pooledObjectData[tag];
        if (currentData.amountActiveInPool < currentQueue.Count)
        {
            GameObject resultObject = currentQueue.Dequeue();

            resultObject.transform.position = position;
            resultObject.transform.rotation = rotation;

            resultObject.SetActive(true);

            IPooledObject pooledObject = resultObject.GetComponent<IPooledObject>();
            if (pooledObject != null)
                pooledObject.OnObjectSpawn();

            currentQueue.Enqueue(resultObject);

            currentData.amountActiveInPool++;
            return resultObject;
        }


        //Else we instantiate more and add poggers
        //Gonna add more badboys in
        Queue<GameObject> currentQueueCopy = new Queue<GameObject>(currentQueue);

        //Clear current Queue
        currentQueue.Clear();

        for (int i = 0; i < currentData.amountToExpand; i++)
        {
            GameObject newObject = Instantiate(currentData.objToPool, currentData.parentInHierachy.transform);
            newObject.SetActive(false);
            currentQueue.Enqueue(newObject);
        }

        while (currentQueueCopy.Count > 0)
        {
            currentQueue.Enqueue(currentQueueCopy.Dequeue());
        }

        GameObject newestObject = currentQueue.Dequeue();

        newestObject.transform.position = position;
        newestObject.transform.rotation = rotation;

        //Debug.Log("Expanded pool by " + objectData.amountToExpand);
        newestObject.SetActive(true);

        currentData.amountActiveInPool++;

        currentQueue.Enqueue(newestObject);

        return newestObject;

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

        int tag = (int)(thatObject.poolType);

        if (poolDictionary.ContainsKey(tag))
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
            poolDictionary[tag].Clear();
            //Copy the list into the old list
            poolDictionary[tag] = new Queue<GameObject>();

            return true;
        }

        Queue<GameObject> resultPool = new Queue<GameObject>();
        //Make new object with name under this transform
        GameObject parent = new GameObject(thatObject.poolName + " Pool");
        parent.transform.parent = poolParent.transform;

        for (int i = 0; i < thatObject.amountToPool; ++i)
        {
            GameObject resultObject = Instantiate(thatObject.objToPool, parent.transform);
            resultObject.SetActive(false);
            resultPool.Enqueue(resultObject);
        }

        //MAke sure the reference to the gameObject parent is saved
        thatObject.parentInHierachy = parent;

        poolDictionary.Add(tag, resultPool);
        pooledObjectData.Add(tag, thatObject);


        return true;
    }

    public bool AddAnotherPool(PoolObject.OBJECTTYPES type, GameObject objectToPool, int amountToPool, int amountToExpand, bool overWrite = false)
    {
        PoolObject resultObject = new PoolObject(type, objectToPool, amountToPool, amountToPool);
        return AddAnotherPool(resultObject, overWrite);
    }

    public string AmountActive(PoolObject.OBJECTTYPES type)
    {
        int tag = (int)(type);

        if (poolDictionary == null || poolDictionary[tag] == null)
            return "";

        Queue<GameObject> tempQueue = new Queue<GameObject>(poolDictionary[tag]);
        int totalCount = tempQueue.Count;

        int totalActive = 0;
        int count = 0;

        do
        {
            if (tempQueue.Dequeue().activeSelf)
                totalActive++;

            count++;

        } while (count < totalCount);



        return pooledObjectData[tag].poolName + " Pool: " + totalActive + "/" + totalCount;

    }

    public int AmountActiveInt(PoolObject.OBJECTTYPES type)
    {
        int tag = (int)(type);

        if (poolDictionary == null || poolDictionary[tag] == null)
            return -1;

        return poolDictionary[tag].Count;
    }

    public void DisableInPool(PoolObject.OBJECTTYPES type)
    {
        //Guaranteed type, dont need to check, i hope.
        int tag = (int)(type);
        pooledObjectData[tag].amountActiveInPool -= 1;
    }
}
