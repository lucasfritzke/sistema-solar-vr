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

    void Start()
    {
        // Inicializa o reticle
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
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Debug visual (linha vermelha)
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.red);

        bool hitSomething = Physics.Raycast(ray, out hit, raycastDistance, interactionLayers);

        if (hitSomething)
        {
            GameObject hitObject = hit.collider.gameObject;
            
            // Log mais detalhado
            Debug.Log($"[RAYCAST] HIT: {hitObject.name} | Tag: {hitObject.tag} | Layer: {LayerMask.LayerToName(hitObject.layer)} | Distance: {hit.distance:F2}m");

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
                    
                    Debug.Log($"[GAZE] Novo alvo: {hitObject.name}");
                }
                else
                {
                    // Continua olhando para o mesmo objeto
                    gazeTimer += Time.deltaTime;
                    
                    // Log a cada 0.5s
                    if (gazeTimer % 0.5f < Time.deltaTime)
                    {
                        Debug.Log($"[GAZE] Timer: {gazeTimer:F1}s / {gazeTime}s");
                    }
                    
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
                Debug.Log($"[RAYCAST] Hit objeto SEM tag 'Selectable': {hitObject.name}");
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
            Debug.Log("[INPUT] Toque/clique detectado!");
            if (currentGazedObject != null && currentButton != null)
            {
                ExecuteGazeAction(currentGazedObject);
                ResetGaze();
            }
        }
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