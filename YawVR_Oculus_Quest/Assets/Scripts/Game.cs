using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game : MonoBehaviour
{
    public static Game instance { get; private set; }

    [System.Serializable]
    protected class PoolObjectsInfo
    {
        public GameObject enemy;
        public PoolObject.OBJECTTYPES poolType;

        public PoolObjectsInfo(GameObject _enemy, PoolObject.OBJECTTYPES typeInPool)
        {
            enemy = _enemy;
            poolType = typeInPool;
        }
    }

    [System.Serializable]
    public class ObjectiveInfo
    {
        public VariedObjectives.TYPE type;//dis is cancerous but wutever
        public Transform m_highlight;
        public bool m_completed = false;
        public float m_timeLeft = 30;
        public float m_timer = 0;
        public float m_spawnTime = 7;
    }

    [Header("Default Mech Loadouts")]
    [SerializeField]
    private GameObject[] m_rightArmModules;
    [SerializeField]
    private GameObject[] m_leftArmModules;
    [Header("Prefabs")]
    [SerializeField]
    private PoolObjectsInfo[] m_enemies;
    [SerializeField]
    private GameObject[] m_structures;
  

    [Header("Game Variables")]
    [SerializeField]
    public int m_objectivesLeft = 0;
    [SerializeField]
    public ObjectiveInfo[] m_objectives;

    //Local variables
    private int currentObjectiveIndex = -1;

    void Awake()
    {
        if (instance == null)
            instance = this;
        print("Game awake!");
    }

    void Start()
    {
        Random.InitState(System.DateTime.Now.Second);
        ApplyMechLoadouts();
        ApplyObjectives();
        StartCoroutine(checkObjectives());
        print("Game started!");
    }

    void Update()
    {
        if (currentObjectiveIndex == -1) return;
        ObjectiveInfo currObj = m_objectives[currentObjectiveIndex];
        currObj.m_timeLeft -= Time.deltaTime;
        currObj.m_timer += Time.deltaTime;
        if (currObj.m_timeLeft <= 0)
        {
            print("Current Objective ended!");
            currObj.m_completed = true;
            currentObjectiveIndex = -1;
            return;
        }
        if (currObj.type == VariedObjectives.TYPE.DEFEND_STRUCTURE && currObj.m_timer > currObj.m_spawnTime)
        {
            if(currObj.m_highlight == null)
            {
                print("Current Objective ended!");
                currObj.m_completed = true;
                currentObjectiveIndex = -1;
                return;
            }
            currObj.m_timer -= currObj.m_spawnTime;
            print("Spawn Enemies!");
            for (int i = 0; i < 3; ++i)
            {
                //EnemyBase derp = Instantiate(m_enemies[i].enemy, currObj.m_highlight.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20)) * (i + 1), Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<EnemyBase>();
                EnemyBase derp = ObjectPooler.instance.SpawnFromPool(m_enemies[i].poolType, currObj.m_highlight.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20)) * (i + 1), Quaternion.identity).GetComponent<EnemyBase>();
                derp.m_target = currObj.m_highlight;
            }
        }
    }

    private void ApplyMechLoadouts()
    {
        PlayerHandler.instance.GetLeftPilotController().AttachArmModules(m_leftArmModules);
        PlayerHandler.instance.GetRightPilotController().AttachArmModules(m_rightArmModules);
    }

    private void ApplyObjectives()
    {
        MapPointsHandler mph = MapPointsHandler.instance;
        System.Array.Resize(ref m_objectives, mph.m_variedObjectives.possibleObjectivePoints.Length);
        for (int i = 0; i < m_objectives.Length; ++i)
        {
            var objIndex = mph.m_variedObjectives.possibleObjectivePoints[i];
            Vector3 objectivePos = mph.m_mapPoints[objIndex];
            m_objectives[i] = new ObjectiveInfo();
            m_objectives[i].type = Random.Range(0, 1000) > 500 ? VariedObjectives.TYPE.BOUNTYHUNT : VariedObjectives.TYPE.DEFEND_STRUCTURE;
            if (m_objectives[i].type == VariedObjectives.TYPE.BOUNTYHUNT)
            {
                //Debug.Log(m_enemies[2].nameInPool);
               // EnemyBase enemy = Instantiate(m_enemies[2].enemy, objectivePos, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<EnemyBase>();
                EnemyBase enemy = ObjectPooler.instance.SpawnFromPool(m_enemies[2].poolType, objectivePos, Quaternion.identity).GetComponent<EnemyBase>();
                enemy.onEntityDie += Enemy_onEntityDie;
                m_objectivesLeft++;
                m_objectives[i].m_highlight = enemy.transform;
                print("Objective deployed : Bounty Hunt @ " + objIndex);
            }
            else if(m_objectives[i].type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
            {
                RaycastHit hit;
                Physics.Raycast(objectivePos, -Vector3.up, out hit);
                BaseStructure structure = Instantiate(m_structures[0], hit.point, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseStructure>();
                structure.onEntityDie += Structure_onEntityDie;
                m_objectivesLeft++;
                m_objectives[i].m_highlight = structure.transform;
                print("Objective deployed : Defend Structure @ " + objIndex);
            }
        }
    }

    private void Enemy_onEntityDie(BaseEntity _entity)
    {
        _entity.onEntityDie -= Enemy_onEntityDie;
        m_objectivesLeft--;
    }

    private void Structure_onEntityDie(BaseEntity _entity)
    {
        _entity.onEntityDie -= Structure_onEntityDie;
        m_objectivesLeft--;
    }

    IEnumerator checkObjectives()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(1);
            if (currentObjectiveIndex != -1)
                continue;
            for (int i = 0; i < m_objectives.Length; ++i)
            {
                if(CustomUtility.IsHitRadius(m_objectives[i].m_highlight.position, PlayerHandler.instance.transform.position, 50.0f))
                {
                    currentObjectiveIndex = i;
                    print("Current Objective started! : " + i);
                    break;
                }
            }
        }
    }
}