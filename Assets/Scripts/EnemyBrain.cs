using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

public class EnemyBrain : MonoBehaviour
{
    Transform target = null;

    [SerializeField]
    Transform origin;
    public Transform Origin { get => origin; set { origin = value; } }

    [SerializeField]
    Transform playerTransform;

    [SerializeField]
    float speed = 0.1f;

    [SerializeField]
    float activationDistance = 10f;
    [SerializeField]
    float huntingDistance = 30f;

    [SerializeField]
    float stunningTimeout = 1f;

    float maxSpeed;

    float force = 25f;

    bool hasPrey;
    public bool HasPrey { get => hasPrey; set { hasPrey = value; } }

    bool isSwarm;
    public bool IsSwarm { get => isSwarm; set { isSwarm = value; } }

    bool isActivated;

    [SerializeField] Light lightFigure;
    [SerializeField] TextMeshPro[] textMeshes;

    NavMeshAgent agent;

    List<GameObject> enemyFriends = new List<GameObject>();

    [SerializeField]
    AudioSource mothersAudioSource;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnDisable()
    {
        transform.position = origin.transform.position;
    
        lightFigure.enabled = false;

        GoodbyeSwarm();

        if(mothersAudioSource.isPlaying)
        {
            mothersAudioSource.Stop();
        }
    }

    void OnDestroy() //just to be sure
    {
        GoodbyeSwarm();

        if (mothersAudioSource.isPlaying)
        {
            mothersAudioSource.Stop();
        }

    }

    void FixedUpdate()
    {
        if (!isSwarm) //Swarm is using navMesh with OnCollision Information
        {
            if (Vector3.Distance(transform.position, playerTransform.position)< activationDistance && !isActivated)
            {
                foreach (TextMeshPro textMesh in textMeshes) //permanent change
                {
                    textMesh.enabled = true;
                }

                StartCoroutine(LightUp(lightFigure));

                isActivated = true;
                target = playerTransform;

                if (mothersAudioSource.enabled)
                    mothersAudioSource.Play();
            }

            if (!hasPrey)
            {
                if (isActivated && target != null)
                {
                    if (Vector3.Distance(transform.position, target.transform.position)< huntingDistance)
                    {
                        mothersAudioSource.pitch = 1 + Vector3.Distance(transform.position, target.transform.position) / huntingDistance;
                        maxSpeed = Mathf.Max(speed, 1/Vector3.Distance(transform.position, origin.position));
                        transform.position = Vector3.MoveTowards(transform.position, hasPrey ? origin.position : target.position, hasPrey ? maxSpeed : speed);
                    }
                    else
                    {
                        target = null;
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, origin.position) > 0.1f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, origin.position, speed/2);
                    }
                    else
                    {
                        isActivated = false;
                    }
                }
            }
            else
            {
                DragPreyBackToOriginOnPath();
            }
        }
    }

    public void ResetTarget()
    {
        target = null;
    }

    void DragPreyBackToOriginOnPath()
    {
        if (agent.enabled)
        {
            NavMeshPath navMeshPath = new NavMeshPath();

            if (agent.CalculatePath(origin.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                agent.SetDestination(origin.transform.position);
        }
    }

    IEnumerator LightUp(Light light, float stopDuration=2f)
    {
        light.enabled = true;

        float duration = 0;
        float startIntensity = 0;
        float endIntensity = light.intensity;

        light.intensity = startIntensity;

        while (duration < stopDuration)
        {
            duration += Time.deltaTime;
            light.intensity = Mathf.Lerp(startIntensity, endIntensity, duration/stopDuration);
            yield return null;
        }

        light.intensity = endIntensity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Dummy"))
        {
            target = other.gameObject.transform;
        }

        if (other.gameObject.CompareTag("ExDummy"))
        {
            StartCoroutine("StunnedBeforeCanAct");
        }

        if (other.CompareTag("Save"))
        {
            GoodbyeSwarm();
            target = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.transform == target)
        {
            Vector3 DirectionToThis = (transform.position - other.transform.position).normalized;
            DirectionToThis.y = 0;
            other.gameObject.GetComponent<Rigidbody>().AddForce(DirectionToThis * force, ForceMode.Force);


            if (Vector3.Distance(transform.position, other.gameObject.transform.position) < (isSwarm ? 0.1f : 1f) && !hasPrey)
            {
                if(isSwarm)
                isSwarm = false;

                agent.avoidancePriority = 10; //high value

                hasPrey = true;
                agent.enabled = true;
            }
        }

        if (other.CompareTag("Eye"))
        {
            InfectWithSwarm(other.gameObject);
        }
    }

    void InfectWithSwarm(GameObject other)
    {
        if (hasPrey && !other.GetComponent<EnemyBrain>().HasPrey)
        {
            NavMeshAgent swarmAgent = other.GetComponent<NavMeshAgent>();

            other.GetComponent<EnemyBrain>().IsSwarm = true;
            swarmAgent.enabled = true;
            swarmAgent.avoidancePriority = 50; //normal value
            swarmAgent.SetDestination(this.origin.transform.position);
            enemyFriends.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == target)
        {
            StartCoroutine("StunnedBeforeCanAct");
        }
    }

    IEnumerator StunnedBeforeCanAct()
    {
        float currentStunned = stunningTimeout;

        while(currentStunned > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            currentStunned -= 0.1f;
        }

        hasPrey = false;
        agent.enabled = false;
        // still uses target to move

        GoodbyeSwarm();
    }

    void GoodbyeSwarm()
    {
        if (enemyFriends.Count == 0) return;
        
        foreach (GameObject friend in enemyFriends)
        {
            ResettingFriend(friend);
        }

        enemyFriends.Clear();
    }

    void ResettingFriend(GameObject friend)
    {
        if (friend == null) return;

        EnemyBrain friendsBrain = friend.GetComponent<EnemyBrain>();
        NavMeshAgent friendsAgent = friend.GetComponent<NavMeshAgent>();

        friendsBrain.IsSwarm = false;
        friendsAgent.enabled = false;
        friendsBrain.ResetTarget();
        friend.transform.position = friendsBrain.Origin.position;
    }


}
