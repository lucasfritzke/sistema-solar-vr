using UnityEngine;
using UnityEngine.UI;

public class SolarVRGaze : MonoBehaviour
{
    [Header("Configurações do Gaze")]
    public float gazeTime = 2f;
    public LayerMask interactionLayers = -1; // Todas as layers por padrão
    public float raycastDistance = 100f;

    [Header("Reticle UI")]
    public Image reticleFill;  // Círculo que preenche
    public Image reticlePoint; // Ponto central
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private float gazeTimer = 0f;
    private GameObject currentGazedObject;
    private Button currentButton;

    private Camera mainCam;

    void Start()
{
    mainCam = Camera.main;
    if (mainCam == null)
    {
        mainCam = FindObjectOfType<Camera>();
        Debug.LogWarning("⚠️ Nenhuma Camera.main encontrada — usando primeira câmera disponível.");
    }

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
        // Raycast do centro da câmera
        if (mainCam == null) return;
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);

        RaycastHit hit;

        // Debug visual (linha vermelha)
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);

        if (Physics.Raycast(ray, out hit, raycastDistance, interactionLayers))
        {
            GameObject hitObject = hit.collider.gameObject;
            
            Debug.Log($"Raycast HIT: {hitObject.name} | Tag: {hitObject.tag}");

            // Verifica se é um objeto selecionável
            if (hitObject.CompareTag("Selectable"))
            {
                // Novo objeto detectado
                if (currentGazedObject != hitObject)
                {
                    ResetGaze();
                    currentGazedObject = hitObject;
                    currentButton = hitObject.GetComponent<Button>();
                    
                    // Notifica o objeto (se tiver SelectableObject)
                    hitObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
                    
                    Debug.Log($"→ Olhando para: {hitObject.name}");
                }
                else
                {
                    // Continua olhando para o mesmo objeto
                    gazeTimer += Time.deltaTime;
                    
                    // Atualiza o círculo de progresso
                    if (reticleFill != null)
                    {
                        reticleFill.fillAmount = gazeTimer / gazeTime;
                    }

                    // Notifica o objeto
                    hitObject.SendMessage("OnGazeStay", SendMessageOptions.DontRequireReceiver);

                    // Completa o gaze
                    if (gazeTimer >= gazeTime)
                    {
                        ExecuteGazeAction(hitObject);
                        ResetGaze();
                    }
                }

                // Muda cor do reticle para hover
                if (reticlePoint != null)
                    reticlePoint.color = hoverColor;

                if (reticleFill != null && !reticleFill.enabled)
                    reticleFill.enabled = true;
            }
            else
            {
                // Hit em algo não selecionável
                ResetGaze();
            }
        }
        else
        {
            // Não acertou nada
            ResetGaze();
        }

        // Também detecta toque/clique direto (backup)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (currentGazedObject != null && currentButton != null)
            {
                ExecuteGazeAction(currentGazedObject);
                ResetGaze();
            }
        }

        if (mainCam != null)
        Debug.DrawRay(mainCam.transform.position, mainCam.transform.forward * 5f, Color.red);
    }

    void ExecuteGazeAction(GameObject target)
    {
        Debug.Log($"✓✓✓ GAZE COMPLETE: {target.name}");

        // Se for um Button UI, clica nele
        if (currentButton != null)
        {
            currentButton.onClick.Invoke();
            Debug.Log($"→ Botão clicado: {target.name}");
        }

        // Notifica o objeto (SelectableObject se tiver)
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

        if (reticleFill != null)
        {
            reticleFill.fillAmount = 0f;
            reticleFill.enabled = false;
        }

        if (reticlePoint != null)
        {
            reticlePoint.color = normalColor;
        }
    }

    void OnDrawGizmos()
    {
        // Desenha o raycast no editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * raycastDistance);
    }
}