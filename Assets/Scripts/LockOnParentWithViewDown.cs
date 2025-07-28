using UnityEngine;

public class LockOnParentWithViewDown : MonoBehaviour
{
    Quaternion lockedRotation = Quaternion.Euler(90, 0, 0);

    void FixedUpdate()
    {
        if (Quaternion.Angle(transform.rotation, lockedRotation) > 0f)
            transform.rotation = lockedRotation;

        transform.position = transform.parent.transform.position;
    }
}
