using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class HasPathToPlayer : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] float activationDistance;

    //[SerializeField] Component ;

    NavMeshAgent navAgent;

    //NavMeshPath pathToPlayer;
    bool hasPath;
    //public bool HasPath { get => hasPath; }

    float pathLength;
    //public float PathLength { get => pathLength; }

    //bool isInRangeOfTarget;
    //public bool IsInRangeOfTarget { get => hasPath; }

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    public bool Check()
    {
        hasPath = IsPathToTargetAvailable();

        if (hasPath)
        {
            pathLength = GetPathLengthToTarget();

            if (pathLength <= activationDistance || activationDistance == 0f)
            {
                return true;
            }
        }

        return false;
    }

    bool IsPathToTargetAvailable()
    {
        NavMeshPath path = new NavMeshPath();
        navAgent.CalculatePath(targetTransform.position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    float GetPathLengthToTarget()
    {
        NavMeshPath path = new NavMeshPath();
        navAgent.CalculatePath(targetTransform.position, path);

        float pathLength = 0f;
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            for (int i = 1; i < path.corners.Length; i++)
            {
                pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return pathLength;
    }
}
