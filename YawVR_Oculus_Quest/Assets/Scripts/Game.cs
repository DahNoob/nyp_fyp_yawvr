﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles all game logic, win conditions and the player's mech loadouts.
/// </summary>
[System.Serializable]
public class Game : MonoBehaviour
{
    /// <summary>
    /// Delegate event definition for when an objective has started.
    /// </summary>
    /// <param name="_objectiveInfo"></param>
    public delegate void ObjectiveStarted(ObjectiveInfo _objectiveInfo);

    /// <summary>
    /// Delegate event definition for when an object has ended, with a outcome.
    /// </summary>
    /// <param name="_objectiveInfo">The reference objectiveInfo</param>
    /// <param name="_succeeded">Was the objective completed?</param>
    /// 
    public delegate void ObjectiveFinished(ObjectiveInfo _objectiveInfo, bool _succeeded);

    /// <summary>
    /// Event that is called when an objective has started.
    /// </summary>
    public event ObjectiveStarted onObjectiveStarted;

    /// <summary>
    /// Event that is called when an object has ended.
    /// </summary>
    public event ObjectiveFinished onObjectiveFinished;

    /// <summary>
    /// Game instance
    /// </summary>
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
    public ObjectiveInfo[] m_objectives;

    [Header("Game Configuration")]
    [SerializeField]
    private int m_maxObjectives = 3;
    [SerializeField]
    [Range(1.0f, 100.0f)]
    private float m_objectiveActivationRadius = 50.0f;
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float m_enemySpawnProbability = 20;
    [SerializeField]
    [Range(0.0f, 500.0f)]
    private float m_enemySpawnDeadzone = 80.0f;
    [SerializeField]
    private Color m_bountyHuntEnemyColor;
    [SerializeField]
    private GameObject m_bountyHuntObjectivePrefab;
    [SerializeField]
    private GameObject m_environmentParticles;
    [SerializeField]
    private OVR.SoundFXRef m_backgroundMusic;
    [SerializeField]
    private OVR.SoundFXRef m_backgroundAmbience;
    [SerializeField]
    private OVR.SoundFXRef m_battleMusic;

    //Local variables
    public int currentObjectiveIndex { private set; get; } = -1;
    private List<int> allocatedPoints;

    void Awake()
    {
        if (instance == null)
            instance = this;
        print("Game awake!");

        //Subscribe to the events
        //Isntance is already set


    }

    void Start()
    {
        //moved it here, dk if it's a good idea or not
        onObjectiveStarted += UIObjectiveHandler.instance.Game_onObjectiveStarted;
        onObjectiveFinished += UIObjectiveHandler.instance.Game_onObjectiveFinished;

        Debug.Log("Game events -attached- successfully!");
        Random.InitState(System.DateTime.Now.Second);
        ApplyMechLoadouts();
        ApplyObjectives();
        StartCoroutine(checkObjectives());

        if (m_environmentParticles != null)
            StartCoroutine(setEnvironmentParticlePosition());

        m_backgroundMusic.PlaySound();
        m_backgroundMusic.AttachToParent(Camera.main.transform);
        m_backgroundAmbience.PlaySound(Random.Range(10, 20));
        m_backgroundAmbience.AttachToParent(Camera.main.transform);

        print("Game started!");
    }

    void Update()
    {
        if (currentObjectiveIndex == -1) return;
        ObjectiveInfo currObj = m_objectives[currentObjectiveIndex];
        currObj.m_timeLeft -= Time.deltaTime;
        currObj.m_timer += Time.deltaTime;
        if (currObj.type == VariedObjectives.TYPE.BOUNTYHUNT)
        {
            if (currObj.m_timer > currObj.m_spawnTime)
            {
                currObj.m_timer -= currObj.m_spawnTime;
                print("Spawn Enemies!");
                int i = 0, max = Random.Range(1, m_enemies.Length);
                while (i < max)
                {
                    QuadRect newQuadRect = new QuadRect(currObj.m_highlight.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20)) * (i + 1), 75, 9999);
                    Vector3 newEnemyPos = MapPointsHandler.instance.GetClosestPoint(newQuadRect);
                    if (CustomUtility.IsHitRadius(newEnemyPos, currObj.m_mapPointPosition, 2.0f))
                        continue;
                    EnemyBase derp = ObjectPooler.instance.SpawnFromPool(m_enemies[i].poolType, newEnemyPos, Quaternion.identity).GetComponent<EnemyBase>();
                    derp.m_target = PlayerHandler.instance.transform;
                    ++i;
                }
            }
            if (!currObj.m_highlight.gameObject.activeInHierarchy)
            {
                //GUIManager.instance.SucceededObjectiveGUI(ref currObj);
                print("Current Objective ended with status <Succeeded objective>!");
                currObj.m_completed = true;
                currObj.panelInfo.panelText.color = Color.green;
                currentObjectiveIndex = -1;
                currObj.m_inProgress = false;
                if (currObj.m_highlight.Find("Crown"))
                    Destroy(currObj.m_highlight.Find("Crown").gameObject);
                if (currObj.m_highlight.Find("Beacon"))
                    Destroy(currObj.m_highlight.Find("Beacon").gameObject);
                Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE_DROP, currObj.m_highlight.position, Quaternion.Euler(0, Random.Range(0, 360), 0), Persistent.instance.GO_STATIC.transform);
                m_battleMusic.DetachFromParent();
                m_battleMusic.StopSound();
                onObjectiveFinished?.Invoke(currObj, true);
                return;
            }
            else if (currObj.m_timeLeft <= 0)
            {
                //GUIManager.instance.FailedObjectiveGUI(ref currObj);
                print("Current Objective ended with status <Failed objective>!");
                currObj.m_completed = true;
                currObj.panelInfo.panelText.color = Color.red;
                currentObjectiveIndex = -1;
                currObj.m_inProgress = false;
                if (currObj.m_highlight.Find("Crown"))
                    Destroy(currObj.m_highlight.Find("Crown").gameObject);
                if (currObj.m_highlight.Find("Beacon"))
                    Destroy(currObj.m_highlight.Find("Beacon").gameObject);
                m_battleMusic.DetachFromParent();
                m_battleMusic.StopSound();
                onObjectiveFinished?.Invoke(currObj, false);
                return;
            }
        }
        else if (currObj.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
        {
            if (currObj.m_timer > currObj.m_spawnTime)
            {
                currObj.m_timer -= currObj.m_spawnTime;
                print("Spawn Enemies!");
                int i = 0, max = Random.Range(1, m_enemies.Length);
                while (i < max)
                {
                    QuadRect newQuadRect = new QuadRect(currObj.m_highlight.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20)) * (i + 1), 75, 9999);
                    Vector3 newEnemyPos = MapPointsHandler.instance.GetClosestPoint(newQuadRect);
                    if (CustomUtility.IsHitRadius(newEnemyPos, currObj.m_mapPointPosition, 2.0f))
                        continue;
                    EnemyBase derp = ObjectPooler.instance.SpawnFromPool(m_enemies[i].poolType, newEnemyPos, Quaternion.identity).GetComponent<EnemyBase>();
                    derp.m_target = Random.Range(0, 100) > 50 ? currObj.m_highlight : PlayerHandler.instance.transform;
                    ++i;
                }
            }
            if (currObj.m_highlight == null)
            {
                UIObjectiveHandler.instance.FailedObjectiveGUI(ref currObj);
                print("Current Objective ended with status <Failed objective>!");
                currObj.m_completed = true;
                currObj.panelInfo.panelText.color = Color.red;
                currentObjectiveIndex = -1;
                currObj.m_inProgress = false;
                m_battleMusic.DetachFromParent();
                m_battleMusic.StopSound();
                onObjectiveFinished?.Invoke(currObj, false);
                //Update UI on fail


                return;

            }
            else if (currObj.m_timeLeft <= 0)
            {
                UIObjectiveHandler.instance.SucceededObjectiveGUI(ref currObj);
                print("Current Objective ended with status <Succeeded objective>!");
                currObj.m_completed = true;
                currObj.panelInfo.panelText.color = Color.green;
                currentObjectiveIndex = -1;
                currObj.m_inProgress = false;
                if (currObj.m_highlight.Find("Beacon"))
                    Destroy(currObj.m_highlight.Find("Beacon").gameObject);
                currObj.m_highlight.GetComponent<ObjectiveStructure>().SetCurrentObjective(false);
                Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE_DROP, currObj.m_highlight.position, Quaternion.Euler(0, Random.Range(0, 360), 0), Persistent.instance.GO_STATIC.transform);
                m_battleMusic.DetachFromParent();
                m_battleMusic.StopSound();
                onObjectiveFinished?.Invoke(currObj, true);
                return;
            }
            UIObjectiveHandler.instance.UpdateObjectiveProgress(ref currObj);
        }
    }

    /// <summary>
    /// Assigns the player it's weapons on it's left and right arms
    /// </summary>
    private void ApplyMechLoadouts()
    {
        PlayerHandler.instance.GetLeftPilotController().AttachArmModules(m_leftArmModules);
        PlayerHandler.instance.GetRightPilotController().AttachArmModules(m_rightArmModules);
    }

    /// <summary>
    /// Handles the logic for objectives, and starts the game with objectives.
    /// </summary>
    private void ApplyObjectives()
    {
        MapPointsHandler mph = MapPointsHandler.instance;
        //SortedDictionary<int, float> distances = new SortedDictionary<int, float>();
        allocatedPoints = new List<int>();
        //System.Array.Resize(ref m_objectives, mph.m_variedObjectives.possibleObjectivePoints.Length);
        System.Array.Resize(ref m_objectives, m_maxObjectives);
        int currObjectivesCount = 0;
        while (currObjectivesCount < m_maxObjectives)
        {
            int randomisedPoint = Random.Range(0, mph.m_variedObjectives.possibleObjectivePoints.Length);
            var objIndex = mph.m_variedObjectives.possibleObjectivePoints[randomisedPoint];
            if (allocatedPoints.Contains(objIndex))
                continue;
            Vector3 objectivePos = mph.m_mapPoints[objIndex];
            m_objectives[currObjectivesCount] = new ObjectiveInfo();
            m_objectives[currObjectivesCount].m_mapPointPosition = objectivePos;
            m_objectives[currObjectivesCount].type = Random.Range(0, 1000) > 500 ? VariedObjectives.TYPE.BOUNTYHUNT : VariedObjectives.TYPE.DEFEND_STRUCTURE;
            if (m_objectives[currObjectivesCount].type == VariedObjectives.TYPE.BOUNTYHUNT)
            {
                //Debug.Log(m_enemies[2].nameInPool);
                // EnemyBase enemy = Instantiate(m_enemies[2].enemy, objectivePos, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<EnemyBase>();
                GameObject marker = Instantiate(m_bountyHuntObjectivePrefab, objectivePos, Quaternion.identity, Persistent.instance.GO_STATIC.transform);
                AttachBeacon(marker.transform, Color.red);
                m_objectives[currObjectivesCount].m_highlight = marker.transform;
                print("Objective deployed : Bounty Hunt @ " + objIndex);
            }
            else if (m_objectives[currObjectivesCount].type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
            {
                RaycastHit hit;
                Physics.Raycast(objectivePos, -Vector3.up, out hit);
                BaseStructure structure = Instantiate(m_structures[0], hit.point, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseStructure>();
                AttachBeacon(structure.transform, Color.blue);
                structure.GetComponent<ObjectiveStructure>().SetRingRadius(m_objectiveActivationRadius);
                structure.onEntityDie += Structure_onEntityDie;
                m_objectives[currObjectivesCount].m_highlight = structure.transform;
                print("Objective deployed : Defend Structure @ " + objIndex);
            }
            UIObjectiveHandler.instance.AddObjectiveToPanel(ref m_objectives[currObjectivesCount], currObjectivesCount);
            //distances.Add(currObjectivesCount, (m_objectives[currObjectivesCount].m_highlight.position - PlayerHandler.instance.transform.position).sqrMagnitude);
            allocatedPoints.Add(objIndex);
            currObjectivesCount++;
        }
        //for (int i = 0; i < m_maxObjectives; ++i)
        //{
        //    if(tempderp[i] == distances[i])
        //    {
        //        if (i == 0)
        //            m_objectives[i].m_highlight.Find("Beacon").GetComponent<LineRenderer>().endColor = Color.green;
        //        else if (i == 1)
        //            m_objectives[i].m_highlight.Find("Beacon").GetComponent<LineRenderer>().endColor = new Color(1, 0.6f, 0);
        //        else
        //            m_objectives[i].m_highlight.Find("Beacon").GetComponent<LineRenderer>().endColor = Color.red;
        //    }
        //}
        UIObjectiveHandler.instance.SetActiveObjective(m_objectives[0]);
        for (int i = 0; i < MapPointsHandler.instance.m_mapPoints.Count; ++i)
        {
            bool loadloadaald = !allocatedPoints.Contains(i);
            if (!allocatedPoints.Contains(i) && Random.Range(0.0f, 100.0f) < m_enemySpawnProbability && !CustomUtility.IsHitRadius(MapPointsHandler.instance.m_mapPoints[i], PlayerHandler.instance.transform.position, m_enemySpawnDeadzone))
            {
                EnemyBase enemy = ObjectPooler.instance.SpawnFromPool(m_enemies[Random.Range(0, m_enemies.Length)].poolType, MapPointsHandler.instance.m_mapPoints[i], Quaternion.identity).GetComponent<EnemyBase>();
                //print("Enemy deployed at point " + i);
            }
        }
    }

    private void Enemy_onEntityDie(BaseEntity _entity)
    {
        _entity.onEntityDie -= Enemy_onEntityDie;
    }

    private void Structure_onEntityDie(BaseEntity _entity)
    {
        _entity.onEntityDie -= Structure_onEntityDie;
    }

    /// <summary>
    /// Coroutine that checks the completion status of all objectives.
    /// </summary>
    /// <returns></returns>
    IEnumerator checkObjectives()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(1);
            if (currentObjectiveIndex != -1)
                continue;
            for (int i = 0; i < m_objectives.Length; ++i)
            {
                if (!m_objectives[i].m_completed && CustomUtility.IsHitRadius(m_objectives[i].m_highlight.position, PlayerHandler.instance.transform.position, 50.0f))
                {
                    currentObjectiveIndex = i;
                    if (m_objectives[i].type == VariedObjectives.TYPE.BOUNTYHUNT)
                    {
                        //should make it pick a random enemy but even more buffed up, and randomise wher it spawns???????? idk
                        EnemyBase enemy = ObjectPooler.instance.SpawnFromPool(m_enemies[2].poolType, m_objectives[i].m_highlight.position, Quaternion.identity).GetComponent<EnemyBase>();
                        AttachBeacon(enemy.transform, m_bountyHuntEnemyColor);
                        AttachCrown(enemy.transform);
                        enemy.SetMaxHealthMultiplier(5);
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
                    else if (m_objectives[i].type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
                    {
                        //AttachCrown(m_objectives[i].m_highlight.transform);
                        m_objectives[i].m_highlight.GetComponent<ObjectiveStructure>().SetCurrentObjective(true);
                    }
                    UIObjectiveHandler.instance.SetActiveObjective(m_objectives[i]);
                    PlayerUIManager.instance.ObjectiveTriggered(i);
                    m_objectives[i].m_inProgress = true;
                    m_objectives[i].panelInfo.panelText.color = Color.yellow;
                    m_battleMusic.PlaySound();
                    m_battleMusic.AttachToParent(Camera.main.transform);
                    onObjectiveStarted?.Invoke(m_objectives[i]);
                    print("Current Objective started! : " + i);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Sets a particle system to be the player's position
    /// </summary>
    /// <returns></returns>
    IEnumerator setEnvironmentParticlePosition()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(3);
            m_environmentParticles.transform.position = PlayerHandler.instance.transform.position;
        }
    }

    /// <summary>
    /// Assigns the player a random objective.
    /// </summary>
    /// <returns>True if theres an uncompleted objective, false if not.</returns>
    public bool SetRandomObjective()
    {
        for (int i = 0; i < m_objectives.Length; ++i)
        {
            if (!m_objectives[i].m_completed)
            {
                UIObjectiveHandler.instance.SetActiveObjective(m_objectives[i]);
                return true;
            }
        }
        UIObjectiveHandler.instance.SetActiveObjective();
        return false;
    }

    private GameObject AttachBeacon(Transform _transform, Color _color)
    {
        GameObject beacon = Instantiate(Persistent.instance.PREFAB_BEACON, _transform);
        beacon.GetComponent<LineRenderer>().endColor = _color;
        beacon.name = "Beacon";
        return beacon;
    }
    private GameObject AttachCrown(Transform _transform)
    {
        GameObject crown = Instantiate(Persistent.instance.PREFAB_CROWN, _transform);
        crown.name = "Crown";
        return crown;
    }

    void OnDrawGizmos()
    {
        GameObject p = GameObject.Find("Player");
        if (p)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(p.transform.position, m_enemySpawnDeadzone);
        }
    }

    /// <summary>
    /// Gets the current objective, and it's info.
    /// </summary>
    /// <returns>Current Objective Data</returns>
    public ObjectiveInfo GetCurrentObjectiveInfo()
    {
        if (currentObjectiveIndex == -1) return null;
        ObjectiveInfo currObj = m_objectives[currentObjectiveIndex];
        return currObj;
    }

    /// <summary>
    /// Gets the nearest objective to the player.
    /// </summary>
    /// <returns>Nearest Objective Data</returns>
    public ObjectiveInfo ReturnNearestObjectiveToPlayer()
    {
        float maxDistance = float.MaxValue;
        ObjectiveInfo nearest = new ObjectiveInfo();
        for (int i = 0; i < m_objectives.Length; ++i)
        {
            if (m_objectives[i] == null
                || m_objectives[i].m_highlight == null
                || m_objectives[i].m_completed)
                continue;

            Vector3 offset = m_objectives[i].m_highlight.position - PlayerHandler.instance.transform.position;
            float sqrLen = offset.sqrMagnitude;

            if (sqrLen < maxDistance)
            {
                maxDistance = sqrLen;
                nearest = m_objectives[i];
            }
        }
        return nearest;
    }

    /// <summary>
    /// Checks if all objectives are cleared
    /// </summary>
    /// <returns>True if all are completed, false if not.</returns>
    public bool IsObjectivesCleared()
    {
        if (m_objectives.Length == 0) return true;
        for (int i = 0; i < m_objectives.Length; ++i)
        {
            if (!m_objectives[i].m_completed)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Stops in game sound.
    /// </summary>
    public void StopAllBGM()
    {
        m_battleMusic.StopSound();
        m_backgroundMusic.StopSound();
    }
}