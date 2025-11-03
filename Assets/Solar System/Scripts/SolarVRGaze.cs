using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SolarVRGaze : MonoBehaviour
{
    [Header("Configurações de Gaze")]
    public float gazeTime = 2f;
    public float raycastDistance = 100f;
    public LayerMask interactableLayers = -1; // Layer para planetas e objetos interativos

    [Header("Referências UI")]
    public Image reticlePoint;
    public Image reticleFill;
    public GraphicRaycaster graphicRaycaster; // Para UI (Canvas)
    public EventSystem eventSystem;

    private float gazeTimer = 0f;
    private GameObject currentTarget;
    private Button currentButton;
    private Camera mainCamera;
    private bool gazeCompleted = false;
    private PointerEventData pointerEventData;

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

        if (graphicRaycaster == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
                graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        }
    }

    void Update()
    {
        // Primeiro tenta UI
        if (!CheckUIGaze())
        {
            // Se não acertou UI, tenta 3D
            Check3DGaze();
        }
    }

    // Retorna true se acertou algum UI interactable
    bool CheckUIGaze()
    {
        if (graphicRaycaster == null || eventSystem == null)
            return false;

        pointerEventData = new PointerEventData(eventSystem)
        {
            position = new Vector2(Screen.width / 2f, Screen.height / 2f)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
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

                HandleGazeProgress(currentTarget);
                return true; // UI detectado
            }
        }

        return false; // nada detectado
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
                currentButton = hitObject.GetComponent<Button>();
            }

            HandleGazeProgress(currentTarget);
        }
        else
        {
            ResetGaze();
        }
    }

    void HandleGazeProgress(GameObject target)
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
                    ExecuteGazeAction(target);
                }
            }
        }
    }

    void ExecuteGazeAction(GameObject target)
    {
        Debug.Log("ExecuteGazeAction chamado para: " + target.name);
        // Botão do UI
        if (currentButton != null)
        {
            currentButton.onClick.Invoke();
            return;
        }

        // Busca PlanetQuiz em qualquer filho do prefab do planeta
        PlanetQuiz planetQuiz = target.GetComponentInChildren<PlanetQuiz>(true);
        if (planetQuiz != null)
        {
            Debug.Log("PlanetQuiz encontrado no filho de: " + target.name);
            planetQuiz.ActivateQuiz();
            return;
        }
        else
        {
            Debug.LogWarning("PlanetQuiz NÃO encontrado no alvo: " + target.name);
        }
            // Mantém caso tenha outros scripts escutando gaze
            target.SendMessage("OnGazeSelect", SendMessageOptions.DontRequireReceiver);
    }

    void ResetGaze()
    {
        currentTarget = null;
        currentButton = null;
        gazeTimer = 0f;
        gazeCompleted = false;

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
