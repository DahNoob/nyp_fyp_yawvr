using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game : MonoBehaviour
{
    public delegate void ObjectiveStarted(ObjectiveInfo _objectiveInfo);
    public delegate void ObjectiveFinished(ObjectiveInfo _objectiveInfo, bool _succeeded);
    public event ObjectiveStarted onObjectiveStarted;
    public event ObjectiveFinished onObjectiveFinished;

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

    //[Header("Game Variables")]
    [HideInInspector]
    public ObjectiveInfo[] m_objectives;

    [Header("Game Configuration")]
    [SerializeField]
    private int m_maxObjectives = 3;
    [SerializeField]
    private Color m_bountyHuntEnemyColor;
    [SerializeField]
    private GameObject m_bountyHuntObjectivePrefab;
    [SerializeField]
    private GameObject m_environmentParticles;

    //Local variables
    public int currentObjectiveIndex { private set; get; } = -1;

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
        StartCoroutine(setEnvironmentParticlePosition());
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
                derp.m_target = Random.Range(0,100) > 50 ? currObj.m_highlight : PlayerHandler.instance.transform;
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
        List<int> allocatedPoints = new List<int>();
        //System.Array.Resize(ref m_objectives, mph.m_variedObjectives.possibleObjectivePoints.Length);
        System.Array.Resize(ref m_objectives, m_maxObjectives);
        int currObjectivesCount = 0;
        while (currObjectivesCount < m_maxObjectives)
        {
            int randomisedPoint = Random.Range(0, mph.m_variedObjectives.possibleObjectivePoints.Length);
            if (allocatedPoints.Contains(randomisedPoint))
                continue;
            var objIndex = mph.m_variedObjectives.possibleObjectivePoints[randomisedPoint];
            Vector3 objectivePos = mph.m_mapPoints[objIndex];
            m_objectives[currObjectivesCount] = new ObjectiveInfo();
            m_objectives[currObjectivesCount].type = Random.Range(0, 1000) > 500 ? VariedObjectives.TYPE.BOUNTYHUNT : VariedObjectives.TYPE.DEFEND_STRUCTURE;
            if (m_objectives[currObjectivesCount].type == VariedObjectives.TYPE.BOUNTYHUNT)
            {
                //Debug.Log(m_enemies[2].nameInPool);
                // EnemyBase enemy = Instantiate(m_enemies[2].enemy, objectivePos, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<EnemyBase>();
                GameObject marker = Instantiate(m_bountyHuntObjectivePrefab, objectivePos, Quaternion.identity, Persistent.instance.GO_STATIC.transform);
                m_objectives[currObjectivesCount].m_highlight = marker.transform;
                print("Objective deployed : Bounty Hunt @ " + objIndex);
            }
            else if (m_objectives[currObjectivesCount].type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
            {
                RaycastHit hit;
                Physics.Raycast(objectivePos, -Vector3.up, out hit);
                BaseStructure structure = Instantiate(m_structures[0], hit.point, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseStructure>();
                structure.onEntityDie += Structure_onEntityDie;
                m_objectives[currObjectivesCount].m_highlight = structure.transform;
                print("Objective deployed : Defend Structure @ " + objIndex);
            }
            GUIManager.instance.AddObjectiveToPanel(ref m_objectives[currObjectivesCount]);
            allocatedPoints.Add(randomisedPoint);
            currObjectivesCount++;
        }
        //for (int i = 0; i < m_objectives.Length; ++i)
        //{
        //    var objIndex = mph.m_variedObjectives.possibleObjectivePoints[i];
        //    Vector3 objectivePos = mph.m_mapPoints[objIndex];
        //    m_objectives[i] = new ObjectiveInfo();
        //    m_objectives[i].type = Random.Range(0, 1000) > 500 ? VariedObjectives.TYPE.BOUNTYHUNT : VariedObjectives.TYPE.DEFEND_STRUCTURE;
        //    if (m_objectives[i].type == VariedObjectives.TYPE.BOUNTYHUNT)
        //    {
        //        //Debug.Log(m_enemies[2].nameInPool);
        //        // EnemyBase enemy = Instantiate(m_enemies[2].enemy, objectivePos, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<EnemyBase>();
        //        GameObject marker = Instantiate(m_bountyHuntObjectivePrefab, objectivePos, Quaternion.identity, Persistent.instance.GO_STATIC.transform);
        //        m_objectives[i].m_highlight = marker.transform;
        //        print("Objective deployed : Bounty Hunt @ " + objIndex);
        //    }
        //    else if(m_objectives[i].type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
        //    {
        //        RaycastHit hit;
        //        Physics.Raycast(objectivePos, -Vector3.up, out hit);
        //        BaseStructure structure = Instantiate(m_structures[0], hit.point, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseStructure>();
        //        structure.onEntityDie += Structure_onEntityDie;
        //        m_objectives[i].m_highlight = structure.transform;
        //        print("Objective deployed : Defend Structure @ " + objIndex);
        //    }
        //}
    }

    private void Enemy_onEntityDie(BaseEntity _entity)
    {
        _entity.onEntityDie -= Enemy_onEntityDie;
    }

    private void Structure_onEntityDie(BaseEntity _entity)
    {
        _entity.onEntityDie -= Structure_onEntityDie;
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
                    if(m_objectives[i].type == VariedObjectives.TYPE.BOUNTYHUNT)
                    {
                        //should make it pick a random enemy but even more buffed up, and randomise wher it spawns???????? idk
                        EnemyBase enemy = ObjectPooler.instance.SpawnFromPool(m_enemies[2].poolType, m_objectives[i].m_highlight.position, Quaternion.identity).GetComponent<EnemyBase>();
                        Destroy(m_objectives[i].m_highlight.gameObject);
                        m_objectives[i].m_highlight = enemy.transform;
                        //SpriteRenderer enemyMarker = enemy.GetComponentInChildren<SpriteRenderer>();
                        //if (enemyMarker)
                        //{   
                        //    enemyMarker.sprite = Persistent.instance.MINIMAP_ICON_OBJECTIVE;
                        //    enemyMarker.color = m_bountyHuntEnemyColor;
                        //}
                        enemy.SetIconColor(m_bountyHuntEnemyColor);
                        enemy.SetIconSprite(Persistent.instance.MINIMAP_ICON_OBJECTIVE);
                    }
                    print("Current Objective started! : " + i);
                    break;
                }
            }
        }
    }

    IEnumerator setEnvironmentParticlePosition()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(3);
            m_environmentParticles.transform.position = PlayerHandler.instance.transform.position;
        }
    }
}