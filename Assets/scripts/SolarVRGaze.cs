using UnityEngine;
using UnityEngine.UI;

public class SolarVRGaze : MonoBehaviour
{
    [Header("Configurações do Gaze")]
    public float gazeTime = 2f;
    public LayerMask interactionLayers = -1;
    public float raycastDistance = 100f;

    [Header("Reticle UI")]
    public Image reticleFill;  // Círculo que preenche
    public Image reticlePoint; // Ponto central
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private float gazeTimer = 0f;
    private GameObject currentGazedObject;
    private Button currentButton;
    private bool gazeCompleted = false;

    void Start()
    {
        if (reticleFill != null)
        {
            reticleFill.fillAmount = 0f;
            reticleFill.enabled = false;
        }

        if (reticlePoint != null)
        {
            reticlePoint.enabled = true;
            reticlePoint.color = normalColor;
        }

        Debug.Log("✓ SolarVRGaze iniciado!");
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red);

        bool hitSomething = Physics.Raycast(ray, out hit, raycastDistance, interactionLayers);

        if (hitSomething)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Selectable"))
            {
                if (currentGazedObject != hitObject)
                {
                    ResetGaze();
                    currentGazedObject = hitObject;
                    currentButton = hitObject.GetComponent<Button>();
                    hitObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
                }

                // 🔹 Some o ponto quando está olhando para algo interativo
                if (reticlePoint != null)
                    reticlePoint.enabled = false;

                // 🔹 Atualiza o círculo de carregamento
                if (!gazeCompleted)
                {
                    gazeTimer += Time.deltaTime;
                    float progress = Mathf.Clamp01(gazeTimer / gazeTime);

                    if (reticleFill != null)
                    {
                        reticleFill.fillAmount = progress;
                        reticleFill.enabled = true;
                    }

                    if (progress >= 1f)
                    {
                        gazeCompleted = true;
                        ExecuteGazeAction(hitObject);
                        if (reticleFill != null)
                            reticleFill.fillAmount = 1f;
                    }
                }
            }
            else
            {
                ResetGaze();
            }
        }
        else
        {
            ResetGaze();
        }

        // Clique manual (backup)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (currentGazedObject != null)
            {
                ExecuteGazeAction(currentGazedObject);
                ResetGaze();
            }
        }
    }

    void ExecuteGazeAction(GameObject target)
    {
        Debug.Log($"✓✓✓ GAZE COMPLETE: {target.name}");

        if (currentButton != null)
            currentButton.onClick.Invoke();

        target.SendMessage("OnGazeSelect", SendMessageOptions.DontRequireReceiver);
    }

    void ResetGaze()
    {
        if (currentGazedObject != null)
        {
            currentGazedObject.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
            currentGazedObject = null;
            currentButton = null;
        }

        gazeTimer = 0f;
        gazeCompleted = false;

        // 🔹 Some o círculo
        if (reticleFill != null)
        {
            reticleFill.fillAmount = 0f;
            reticleFill.enabled = false;
        }

        // 🔹 Volta a mostrar o ponto
        if (reticlePoint != null)
        {
            reticlePoint.enabled = true;
            reticlePoint.color = normalColor;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * raycastDistance);
    }
}
