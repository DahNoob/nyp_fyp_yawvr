using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryController : MonoBehaviour
{
    [Header("Sentry Configuration")]
    [SerializeField]
    private Transform m_projectileOrigin;
    [SerializeField]
    private GameObject m_projectilePrefab;
    [SerializeField]
    private GameObject bulletSorter;
    [SerializeField]
    QuadRect queryBounds;

    [Header("Shooting Configurations")]
    [SerializeField]
    private float m_shootTime;

    [Header("Ammo Configuration")]
    [SerializeField]
    private AmmoModule m_ammoModule;

    //Local variables
    private float m_shootTick;

    private bool canShoot = true;

    public List<GameObject> obstacles;
    public List<GameObject> enemies;

    public float projectileVelocity;

    public GameObject targetCenterObject;

    // Start is called before the first frame update
    void Start()
    {
        SentryProjectile projectile = m_projectilePrefab.GetComponent<SentryProjectile>();
        enemies = new List<GameObject>();
        obstacles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //Update query bounds position
        queryBounds.position = transform.position;
        //Query and sort
        QueryStuffAndSort();

        Vector3 targetCenter = FirstOrderIntercept(projectileVelocity, enemies[0]);

        //Debug.Log(targetCenter);
        targetCenterObject.transform.position = targetCenter;
        transform.parent.LookAt(targetCenter);

        if (m_ammoModule.m_isReloading == false)
        {
            canShoot = m_shootTick > m_shootTime;
            if (!canShoot)
                m_shootTick += Time.deltaTime;
            else
            {
                m_shootTick -= m_shootTime;
                Debug.Log("shoot");
                BaseProjectile baseProjectile = Instantiate(m_projectilePrefab, m_projectileOrigin.position, m_projectileOrigin.rotation, bulletSorter.transform).GetComponent<BaseProjectile>();
                baseProjectile.Init(m_projectileOrigin);
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(queryBounds.position,
            queryBounds.GetWidth() * 2);

    }

    public void QueryStuffAndSort()
    {
        obstacles = QuadTreeManager.instance.Query(queryBounds, true);
        enemies = QuadTreeManager.instance.Query(queryBounds, false);
    }

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




}
