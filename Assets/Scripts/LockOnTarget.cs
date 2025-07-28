using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    public Transform Target { get => target; set { target = value; } }

    void FixedUpdate()
    {
        if (target != null)
            transform.position = new Vector3(target.position.x, this.transform.position.y, target.position.z);
    }

}
