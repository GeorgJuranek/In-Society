using System;
using System.Collections.Generic;
using UnityEngine;

public class MakeInvisibleForCam : MonoBehaviour
{
    Ray ray;
    List<GameObject> lastHits = new List<GameObject>();

    [SerializeField]
    GameObject player;

    bool isBlockedByCutscene;

    private void OnEnable()
    {
        DoorOpener.OnDoorSceneIsHappening += ChangeActive;
    }

    private void OnDisable()
    {
        DoorOpener.OnDoorSceneIsHappening -= ChangeActive;
    }

    void ChangeActive(bool isInCutscene)
    {
        isBlockedByCutscene = isInCutscene;
    }


    private void FixedUpdate()
    {
        HandleRenderersForCamera();
    }

    private void HandleRenderersForCamera()
    {
        if (isBlockedByCutscene) return;

        Vector3 direction = player.transform.position - transform.position;
        ray = new Ray(transform.position, direction);
        Debug.DrawRay(transform.position, direction, Color.green);

        float distance = Vector3.Distance(transform.position, player.transform.position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, distance, LayerMask.GetMask("Walls"));

        if (hit.transform != null)
        {
            if (hit.transform.gameObject.GetComponent<Renderer>() != null)
            {
                if (!lastHits.Contains(hit.transform.gameObject))
                {
                    lastHits.Add(hit.transform.gameObject);
                }

                SetMaterialTransparency(hit.transform.gameObject.GetComponent<Renderer>(), false);// targetTransparency);// 0.5f); // Setze die gewünschte Transparenz
            }
        }
        else
        {
            if (lastHits.Count > 0)
            {
                foreach (GameObject last in lastHits)
                {
                    SetMaterialTransparency(last.GetComponent<Renderer>(), true); // 1f); // Vollständig undurchsichtig
                }

                lastHits.Clear();
            }
        }
    }

    private void SetMaterialTransparency(Renderer renderer, bool value)
    {
        if (renderer == null)
            return;

        if (!value)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;// = value;
        else
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;// = value;
    }


        #region tests
    private void SetMaterialTransparency(Renderer renderer, float alpha)
    {
        // Durchlaufe alle Materialien des Renderers
        foreach (Material mat in renderer.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                // Setze die Transparenz (Alpha-Wert) des Materials
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;

                // Setze den Blend-Mode für korrektes Alpha-Blending
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

                // Schreibe in den Z-Buffer, um Tiefeninformationen korrekt zu handhaben
                mat.SetInt("_ZWrite", 1);  // Immer aktiv, um Durchscheinen zu verhindern

                // Shader-Keywords für Transparenz aktivieren
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                // Transparente Materialien sollten später in der Render-Reihenfolge sein
                mat.renderQueue = 3000;
            }
        }
    }
        public void SelectedUnrender(GameObject currentHit)
    {
        foreach (GameObject alreadyHitted in lastHits)
        {
            if (currentHit.transform == alreadyHitted.transform)
            {
                SetMaterialTransparency(alreadyHitted.GetComponent<Renderer>(), 0f); // Vollständig transparent
            }
            else
            {
                SetMaterialTransparency(alreadyHitted.GetComponent<Renderer>(), 1f); // Vollständig undurchsichtig
            }
        }
    }
    #endregion
}
