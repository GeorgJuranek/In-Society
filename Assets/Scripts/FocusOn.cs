using UnityEngine;

public class FocusOn : MonoBehaviour
{
    [SerializeField] Transform target;

    Light spotLight;

    private void Awake()
    {
        spotLight = GetComponent<Light>();
    }

    void FixedUpdate()
    {
        SetLightRangeTillGround();

        if (target != null)
        {
            transform.LookAt(target);
        }
    }

    private void SetLightRangeTillGround()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 100f, LayerMask.GetMask("Ground")))//transform.position, transform.forward, 100f,LayerMask.GetMask("Ground")))
        {
            spotLight.range = Vector3.Distance(transform.position, raycastHit.transform.position);
        }

        float maxSpotAngle;
        float maxIntensity;
        if (Physics.Raycast(ray, spotLight.range, LayerMask.GetMask("RatsIgnore")))
        {
            maxSpotAngle = 5f;
            maxIntensity = 3f;
        }
        else
        {
            maxSpotAngle = 20f;
            maxIntensity = 1f;
        }

        spotLight.spotAngle = Mathf.Lerp(spotLight.spotAngle, maxSpotAngle, 0.05f);
        spotLight.intensity = Mathf.Lerp(spotLight.intensity, maxIntensity, 0.05f);

    }
}
