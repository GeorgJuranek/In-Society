using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RatBrain : MonoBehaviour
{
    [SerializeField]
    EAreas areaBelonging = EAreas.Error; //default
    public EAreas AreaBelonging { get => areaBelonging; } 

    Animator ratAnimator;
    NavMeshAgent ratAgent;

    bool isAlarmed;
    public bool IsAlarmed { get => isAlarmed; set { isAlarmed = value; } }

    bool isTrapped;
    public bool IsTrapped
    {
        get => isTrapped;
        set
        {
            if (isTrapped == value) return;
            isTrapped = value;
            ratAnimator.SetBool("isTrapped", value);
        }
    }

    [SerializeField] float runSpeed = 4.5f;
    [SerializeField] float walkSpeed = 2.5f;

    [SerializeField] LayerMask ground;

    [SerializeField] public Light ratTailLight;

    Transform ratKingTransform;

    [SerializeField] float alarmDistance = 3f;
    [SerializeField] float safetyDistance = 20f;
    [SerializeField] float highlightDistance = 10f;

    [SerializeField] float pauseDuration = 5f;
    [SerializeField] float escapeDistance = 5f;

    [SerializeField] float minDestinationDistance = 10f;
    [SerializeField] float maxDestinationDistance = 50f;

    NavMeshPath navMeshPath;

    void Awake()
    {
        navMeshPath = new NavMeshPath();
        ratKingTransform = GameObject.FindGameObjectWithTag("Player").transform;
        ratAnimator = GetComponent<Animator>();
        ratAgent = GetComponent<NavMeshAgent>();
        SetRandomDestination();
    }

    void Update()
    {
        if(!isTrapped)
        {
            ratAnimator.SetFloat("Blend", ratAgent.velocity.magnitude / (2 * runSpeed));
        }
    }

    private void FixedUpdate()
    {
        if (!isTrapped)
        {
            ControlSpeed();

            //Case: RatKing contact
            if (Vector3.Distance(transform.position, ratKingTransform.position) < alarmDistance && !isAlarmed)
            {
                isAlarmed = true;
            }
            //Case: Rat tries to escape
            else if (isAlarmed)
            {
                //Case: Rat is safe again
                if (Vector3.Distance(ratKingTransform.position, transform.position) > safetyDistance)
                {
                    isAlarmed = false;
                }
                else
                {
                    RunAway();
                }
            }
            //Case: normal rat behaviour
            else
            {
                ActNormal();
            }
        }

        if (!isTrapped)
        {
            if (Vector3.Distance(transform.position, ratKingTransform.position) < highlightDistance)
            {
                ratTailLight.enabled = true;
            }
            else
            {
                ratTailLight.enabled = false;
            }
        }
    }

    void ControlSpeed()
    {
        float targetSpeed = CalculateTargetSpeed();

        if (ratAgent.speed != targetSpeed)
            ratAgent.speed = Mathf.Lerp(ratAgent.speed, targetSpeed, 0.1f);
    }

    float CalculateTargetSpeed()
    {
        float result;

        if (isAlarmed)
        {
            result = runSpeed;
        }
        else if (ratAgent.isStopped)
        {
            result = 0f;
        }
        else
        {
            result = walkSpeed;
        }

        return result;
    }

    void SetRandomDestination()
    {
        Vector3 newDestination;

        float newXrange = Random.Range(minDestinationDistance, maxDestinationDistance);
        float newZrange = Random.Range(minDestinationDistance, maxDestinationDistance);

        float newX = Random.Range(-newXrange, newXrange);
        float newZ = Random.Range(-newZrange, newZrange);
        newDestination = new Vector3(newX, 0, newZ);
        newDestination = transform.position + newDestination;

        if (ratAgent.isOnNavMesh && ratAgent.CalculatePath(newDestination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            ratAgent.SetDestination(newDestination);
    }

    private void ActNormal()
    {
        if (Vector3.Distance(ratAgent.destination, transform.position) < 1f && ratAgent.enabled == true && !ratAgent.isStopped)
        {
            StartCoroutine(StopForRest());
        }
    }

    private void RunAway()
    {
        Vector3 retreatDestination = transform.position + ((transform.position - ratKingTransform.position).normalized * escapeDistance);

        if (ratAgent.CalculatePath(retreatDestination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            ratAgent.SetDestination(retreatDestination);
        }
        else
        {
            if (ratAgent.CalculatePath(Quaternion.Euler(0f, -45f, 0f) * retreatDestination, navMeshPath))
            {
                retreatDestination = Quaternion.Euler(0f, -45f, 0f) * retreatDestination;
            }
            else
            {
                retreatDestination = Quaternion.Euler(0f, 45f, 0f) * retreatDestination;
            }

            ratAgent.SetDestination(retreatDestination);
        }
    }

    IEnumerator StopForRest()
    {
        float pauseTime = 0f;

        while (pauseTime < pauseDuration)
        {
            if (isAlarmed || !ratAgent.isOnNavMesh)
            {
                yield break;
            }

            yield return null;
            pauseTime += 0.01f;
        }

        SetRandomDestination();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Servant"))
        {
            if (collision.gameObject.GetComponent<RatBrain>().IsAlarmed && !isAlarmed)
            {
                isAlarmed = true;
            }
        }
    }
}

