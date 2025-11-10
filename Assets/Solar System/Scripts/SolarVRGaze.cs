using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SolarVRGaze : MonoBehaviour
{
    [Header("Configurações de Gaze")]
    public float gazeTime = 2f;
    public float raycastDistance = 100f;
    public LayerMask interactableLayers = -1;

    [Header("Referências UI")]
    public Image reticlePoint;
    public Image reticleFill;
    public EventSystem eventSystem;

    private float gazeTimer = 0f;
    private bool gazeCompleted = false;
    private GameObject currentTarget;
    private Camera mainCamera;
    private Button currentButton;
    private GraphicRaycaster[] allRaycasters;

    void Start()
    {
        mainCamera = Camera.main;

        if (reticleFill != null)
        {
            reticleFill.fillAmount = 0f;
            reticleFill.enabled = false;
        }

        if (reticlePoint != null)
            reticlePoint.enabled = true;

        if (eventSystem == null)
            eventSystem = FindObjectOfType<EventSystem>();

        // Busca todos os canvases da cena (um por planeta, no seu caso)
        allRaycasters = FindObjectsOfType<GraphicRaycaster>(true);
    }

    void Update()
    {
        // 1️⃣ Tenta interagir com UI (botões 2D)
        if (!CheckUIGaze())
        {
            // 2️⃣ Se não encontrou UI, tenta 3D
            Check3DGaze();
        }
    }

    bool CheckUIGaze()
    {
        if (allRaycasters == null || eventSystem == null)
            return false;

        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = new Vector2(Screen.width / 2f, Screen.height / 2f)
        };

        foreach (var raycaster in allRaycasters)
        {
            if (!raycaster.isActiveAndEnabled)
                continue;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                Button btn = result.gameObject.GetComponent<Button>();
                if (btn != null && btn.interactable)
                {
                    if (result.gameObject != currentTarget)
                    {
                        ResetGaze();
                        currentTarget = result.gameObject;
                        currentButton = btn;
                    }

                    HandleGazeProgress();
                    return true;
                }
            }
        }

        return false;
    }

    void Check3DGaze()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayers))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != currentTarget)
            {
                ResetGaze();
                currentTarget = hitObject;
            }

            HandleGazeProgress();
        }
        else
        {
            ResetGaze();
        }
    }

    void HandleGazeProgress()
    {
        if (reticlePoint != null)
            reticlePoint.enabled = false;

        if (reticleFill != null)
        {
            reticleFill.enabled = true;

            if (!gazeCompleted)
            {
                gazeTimer += Time.deltaTime;
                reticleFill.fillAmount = gazeTimer / gazeTime;

                if (reticleFill.fillAmount >= 1f)
                {
                    gazeCompleted = true;
                    ExecuteGazeAction();
                }
            }
        }
    }

    void ExecuteGazeAction()
    {
        if (currentTarget == null) return;

         if (currentButton != null)
        {
            currentButton.onClick.Invoke();
            return;
        }

        // Caso seja um planeta
        PlanetQuiz planetQuiz = currentTarget.GetComponentInChildren<PlanetQuiz>(true);
        if (planetQuiz != null)
        {
            Debug.Log("PlanetQuiz encontrado no filho de: " + currentTarget.name);
            planetQuiz.ActivateQuiz();
            return;
        }
        else
        {
            Debug.LogWarning("PlanetQuiz NÃO encontrado no alvo: " + currentTarget.name);
        }
        currentTarget.SendMessage("OnGazeSelect", SendMessageOptions.DontRequireReceiver);
    }

    void ResetGaze()
    {
        gazeTimer = 0f;
        gazeCompleted = false;
        currentTarget = null;
        currentButton = null;

        if (reticlePoint != null)
            reticlePoint.enabled = true;

        if (reticleFill != null)
        {
            reticleFill.enabled = false;
            reticleFill.fillAmount = 0f;
        }
    }

    void OnDrawGizmos()
    {
        if (Camera.main != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * raycastDistance);
        }
    }
}
