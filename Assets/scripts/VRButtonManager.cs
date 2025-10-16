using UnityEngine;
using UnityEngine.UI;

public class VRButtonManager : MonoBehaviour
{
    [Header("Referências")]
    public Camera vrCamera; // câmera principal do VR
    public Transform heart; // objeto do coração
    public Transform targetPosition; // posição na frente da câmera
    public CameraTourController tourController; // script do tour
    
    [Header("Configurações")]
    public float speed = 2f;
    public float raycastDistance = 100f;
    public LayerMask buttonLayer; // layer dos botões
    
    [Header("UI Reticle (opcional)")]
    public Image reticleImage; // imagem do ponto no centro da tela
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    
    private bool moveToTarget = false;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private GameObject currentLookingAt;

    void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;
            
        if (heart != null)
        {
            originalPos = heart.position;
            originalRot = heart.rotation;
        }
    }

    void Update()
    {
        HandleVRRaycast();
        HandleMouseClickDirect(); // Nova função para clicar direto
        HandleKeyboardShortcuts(); // Atalhos de teclado para testar
        MoveHeart();
    }
    
    // Método 3: Atalhos de teclado (para testar facilmente)
    void HandleKeyboardShortcuts()
    {
        // Pressione C para centralizar o coração
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("=====> Tecla C pressionada!");
            CenterHeart();
        }
        
        // Pressione T para iniciar/parar o tour
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("=====> Tecla T pressionada!");
            StartTour();
        }
    }
    
    // Método 1: Raycast do centro da tela (para VR)
    void HandleVRRaycast()
    {
        // Raycast do centro da tela
        Ray ray = new Ray(vrCamera.transform.position, vrCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, buttonLayer))
        {
            // Debug para ver qual objeto está olhando
            Debug.Log($"Olhando para: {hit.collider.gameObject.name} | Tag: {hit.collider.tag}");
            
            // Mudando a cor do reticle quando olha para um botão
            if (reticleImage != null)
                reticleImage.color = hoverColor;

            currentLookingAt = hit.collider.gameObject;

            // Detecta toque na tela (celular) ou clique do mouse (PC)
            bool inputDetected = false;
            
            // Para celular
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                inputDetected = true;
            }
            
            // Para PC (testar no Unity)
            if (Input.GetMouseButtonDown(0))
            {
                inputDetected = true;
            }
            
            if (inputDetected)
            {
                // Identifica qual botão foi clicado
                if (hit.collider.CompareTag("CenterButton"))
                {
                    CenterHeart();
                }
                else if (hit.collider.CompareTag("TourButton"))
                {
                    StartTour();
                }
            }
        }
        else
        {
            // Restaura cor normal do reticle
            if (reticleImage != null)
                reticleImage.color = normalColor;
                
            currentLookingAt = null;
        }
    }
    
    // Método 2: Clique direto no objeto (para testar no PC)
    void HandleMouseClickDirect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = vrCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                Debug.Log($"Clicou em: {hit.collider.gameObject.name} | Tag: {hit.collider.tag}");
                
                // Identifica qual botão foi clicado
                if (hit.collider.CompareTag("CenterButton"))
                {
                    CenterHeart();
                }
                else if (hit.collider.CompareTag("TourButton"))
                {
                    StartTour();
                }
            }
        }
    }

    void CenterHeart()
    {
        moveToTarget = !moveToTarget;
        Debug.Log($"=====> Botão CENTRALIZAR pressionado! moveToTarget = {moveToTarget}");
    }

    void StartTour()
    {
        if (tourController != null)
        {
            tourController.StartTour();
            Debug.Log("=====> Botão TOUR pressionado!");
        }
        else
        {
            Debug.LogError("Tour Controller não está configurado no VRManager!");
        }
    }

    void MoveHeart()
    {
        if (heart == null || targetPosition == null) return;

        if (moveToTarget)
        {
            heart.position = Vector3.Lerp(heart.position, targetPosition.position, Time.deltaTime * speed);
            heart.rotation = Quaternion.Lerp(heart.rotation, targetPosition.rotation, Time.deltaTime * speed);
        }
        else
        {
            heart.position = Vector3.Lerp(heart.position, originalPos, Time.deltaTime * speed);
            heart.rotation = Quaternion.Lerp(heart.rotation, originalRot, Time.deltaTime * speed);
        }
    }

    // Método público para resetar a posição
    public void ResetHeartPosition()
    {
        moveToTarget = false;
    }
}