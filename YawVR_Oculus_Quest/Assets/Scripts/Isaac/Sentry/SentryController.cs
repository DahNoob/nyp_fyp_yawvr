using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SentryController : MonoBehaviour
{
    //Sentry modes to determine the sentry's behaviour
    public enum SENTRY_MODES
    {
        CLOSEST_DISTANCE,      //fires at the closest enemy at all times
        FIXED_TARGET,               // gets a target and continues firing until either : target is dead, or out of range, or cant be seen
        DISABLED,                       //doesn't do anything
        TOTAL_MODES               //are you serious right now?
    }

    [Header("Sentry Configuration")]
    [SerializeField]
    [Tooltip("Where the projectile fires from")]
    private Transform m_projectileOrigin;
    [SerializeField]
    [Tooltip("The projectile prefab, as much as possible edit Sentry Projectile because it is edited")]
    private GameObject m_projectilePrefab;
    [SerializeField]
    [Tooltip("Temporary solution to put the bullets")]
    private GameObject bulletSorter;
    [SerializeField]
    [Tooltip("The visibility or check range of this turret, the higher the more things it can see")]
    private QuadRect m_queryBounds;
    [SerializeField]
    [Tooltip("How fast the projectile travels")]
    private float projectileVelocity;

    [Header("Shooting Configurations")]
    [SerializeField]
    [Tooltip("How long between each shot")]
    private float m_shootTime;
    [SerializeField]
    [Tooltip("How much ammo each shot takes")]
    private int m_shootCost;

    [Header("Ammo Configuration")]
    [SerializeField]
    [Tooltip("Configurations for the ammo module")]
    private AmmoModule m_ammoModule;

    [Header("Target Finding Configuration")]
    [SerializeField]
    [Tooltip("Time in between querying for enemies")]
    private float m_enemyQueryTime = 0.5f;

    [Header("Behavioural Configuration")]
    [SerializeField]
    [Tooltip("Does the sentry react to attacks")]
    private bool m_reactToAttacks = false;
    [SerializeField]
    [Tooltip("Sentry Mode")]
    private SENTRY_MODES m_sentryMode = SENTRY_MODES.CLOSEST_DISTANCE;

    //Local variables
    private float m_shootTick;
    //Can the turret shoot
    private bool canShoot = true;
    [SerializeField] //debuggin
    //The list of enemies nearby, don't really need to store, but we'll see..
    private List<GameObject> m_enemyList;
    //Current target, will change to enemy base soon hopefully?
    private GameObject m_currentTarget;
    //Is the turret searching for a target
    private bool m_isSearching = false;
    //Target predicted position
    private Vector3 m_predictedPosition;

    // Start is called before the first frame update
    void Start()
    {
        //Initialise enemy List
        m_enemyList = new List<GameObject>();

        //For now it's this  animationless coroutine, once it is in, then we do some cool stuff
        if (m_sentryMode != SENTRY_MODES.DISABLED)            //Start the querying coroutine
            SearchForTarget();

    }

    // Update is called once per frame
    void Update()
    {
        if (m_sentryMode == SENTRY_MODES.DISABLED)
        {
            //Do something like some disabled update.
            return;
        }

        //Update query bounds position
        m_queryBounds.position = transform.position;

        switch (m_sentryMode)
        {
            case SENTRY_MODES.CLOSEST_DISTANCE:
                {
                    SearchForTarget();
                    //Search for closest object always.
                    break;
                }
            case SENTRY_MODES.FIXED_TARGET:
                {
                    if (!IsTargetValid())
                        SearchForTarget();
                    break;
                }
        }

        //We check if our target is null, cause why bother to do stuff if we don't have a target
        if (m_currentTarget != null)
        {
            //Just in case theres no enemies in the querying area, then... if target is still valid, shoot.
            if (IsTargetValid())
            {
                ShootTowardsTarget();
            }
        }
        else
        {
            SearchForTarget();
        }

    }

    public void SearchForTarget()
    {
        //if not already searching, start the coroutine again.
        if (!m_isSearching)
        {
            StartCoroutine(QueryEnemies());
        }
    }

    bool IsTargetValid()
    {
        //If the target is already null. return false
        if (m_currentTarget == null)
            return false;

        //If the bounds does not contain the enemy's point anymore, then return false;
        if (!m_queryBounds.Contains(m_currentTarget.transform.position))
            return false;


        Vector3 directionVector = (m_currentTarget.transform.position - m_projectileOrigin.position).normalized;

        Ray ray = new Ray(m_projectileOrigin.transform.position, directionVector);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.black);
        RaycastHit hit;
        LayerMask projectileMask = ~LayerMask.GetMask("PlayerProjectile");
        if (Physics.Raycast(ray.origin, ray.direction, out hit, float.MaxValue, projectileMask))
        {
            if (hit.collider.gameObject != m_currentTarget)
            {
                //Enemy being blocked by something?
                return false;
            }
        }

        //Then target still valid
        return true;
    }


    void ShootTowardsTarget()
    {

        //Look towards the enemy
        //Calculate the target center
        m_predictedPosition = FirstOrderIntercept(projectileVelocity, m_currentTarget);
        transform.parent.LookAt(m_predictedPosition);

        // transform.parent.LookAt()
        //Then we shoot using the forward vector
        Shoot();
    }

    void Shoot()
    {
        //if the ammo module is not reloading...
        if (m_ammoModule.m_isReloading == false)
        {
            //For some animation update later on.
            canShoot = m_shootTick > m_shootTime;
            if (!canShoot)
                m_shootTick += Time.deltaTime;
            else
            {
                //Shoot happens
                if (m_ammoModule.DecreaseAmmo(m_shootCost))
                {
                    m_shootTick -= m_shootTime;
                    BaseProjectile baseProjectile = Instantiate(m_projectilePrefab, m_projectileOrigin.position, m_projectileOrigin.rotation, bulletSorter.transform).GetComponent<BaseProjectile>();
                    baseProjectile.Init(m_projectileOrigin);
                }
                else
                {
                    //Else we do the reload
                    StartCoroutine(m_ammoModule.Reload());
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(m_queryBounds.position,
            m_queryBounds.GetWidth() * 2);
    }
    //Get all dynamic objects
    //  m_enemyList = QuadTreeManager.instance.Query(queryBounds, false);

    IEnumerator QueryEnemies()
    {
        //Ignore the null if mode is on closest
        if (m_currentTarget == null || m_sentryMode == SENTRY_MODES.CLOSEST_DISTANCE)
        {
            m_isSearching = true;
            while (true)
            {
                yield return new WaitForSeconds(m_enemyQueryTime);

                Debug.Log("Searching always");
                //Query the list and update the enemy list
                m_enemyList = QuadTreeManager.instance.Query(m_queryBounds, false);

                //List to be populated after the checks
                List<GameObject> filteredList = new List<GameObject>();

                //Potentially skip the more demanding code later on
                if (m_enemyList.Count == 0)
                {
                    m_isSearching = false;
                    yield break;
                }

                //Loop through the list and do some checks on whether it can be detected.
                //Raycast from the projectile pos to the enemy to see if it can even be hit
                foreach (GameObject enemies in m_enemyList)
                {
                    //Get normalized vector
                    Vector3 normalizedVector = (enemies.transform.position - m_projectileOrigin.position).normalized;
                    LayerMask projectileMask = ~LayerMask.GetMask("PlayerProjectile");
                    Ray ray = new Ray(m_projectileOrigin.position, normalizedVector);
                    RaycastHit hit;
                    if (Physics.Raycast(ray.origin, ray.direction, out hit, float.MaxValue, projectileMask))
                    {
                        if (hit.collider.gameObject == enemies)
                            filteredList.Add(enemies);
                    }
                }

                //No enemies
                if (filteredList.Count == 0)
                {
                    m_isSearching = false;
                    yield break;
                }

                float maxDistance = float.MaxValue;
                //Now we have the results... we can filter
                //Simple find the closest and shoot
                foreach (GameObject filteredEnemies in filteredList)
                {
                    Vector3 direction = filteredEnemies.transform.position - m_projectileOrigin.position;
                    //Calculate the distance between the enemies and the projectile origin or something
                    float distanceSquared = direction.sqrMagnitude;
                    if (distanceSquared < maxDistance)
                    {
                        //It's closer to the origin, so update.
                        maxDistance = distanceSquared;
                        m_currentTarget = filteredEnemies;
                    }
                }

                m_isSearching = false;
                //End function
                yield break;
            }
        }
    }




    #region Intersect Functions
    //Some intersect functions
    public Vector3 FirstOrderIntercept(float shotSpeed, GameObject target)
    {
        //Calculate target relativePosition
        Vector3 targetRelativePosition = target.transform.position - transform.position;
        NavMeshAgent navMeshAgent = target.GetComponent<NavMeshAgent>();

        Vector3 targetRelativeVelocity = navMeshAgent.velocity - GetComponent<Rigidbody>().velocity;

        float time = FirstOrderInterceptTime(shotSpeed, targetRelativePosition, targetRelativeVelocity);

        return target.transform.position + time * (targetRelativeVelocity);
    }

    //Get time to position
    public float FirstOrderInterceptTime(float shotSpeed, Vector3 relativePosition, Vector3 relativeVelocity)
    {
        float cutoff = 0.001f;
        float velSquared = relativeVelocity.sqrMagnitude;
        if (velSquared < cutoff)
            return 0f;

        float a = velSquared - shotSpeed * shotSpeed;

        if (Mathf.Abs(a) < cutoff)
        {
            float t = -relativePosition.sqrMagnitude / (2f * Vector3.Dot(relativeVelocity, relativePosition));

            return Mathf.Max(t, 0f);
        }

        //Quadratic Formula
        float b = 2f * Vector3.Dot(relativeVelocity, relativePosition);
        float c = relativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        {
            //Determinant > 0 // two intercept paths?
            //Get the two times and reject the other timing
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
            float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);

            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both positive, take the min
                else
                    return t1; //only t1 positive
            }
            else
                return Mathf.Max(t2, 0f);
        }
        else if (determinant < 0f)
            return 0f;
        else //determinant ==0, one intercept path?
            return Mathf.Max(-b / (2f * a), 0f);
    }
    #endregion



}
