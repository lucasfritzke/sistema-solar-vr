using UnityEngine;
using UnityEngine.UI;

public class InitialCanvasHandler : MonoBehaviour
{
    [Header("Referências")]
    public XRRigMover xrRigMover; // Arraste o objeto com o script XRRigMover aqui
    public Button startButton; // Botão "Iniciar Jogo"

    [Header("Configurações")]
    public bool hideCanvasOnStart = true; // Esconde o canvas quando iniciar
    public float delayBeforeMove = 0.5f; // Delay antes de mover (opcional)

    void Start()
    {
        // Se não definiu o botão manualmente, tenta encontrar
        if (startButton == null)
            startButton = GetComponentInChildren<Button>();

        // Adiciona o listener ao botão
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
            Debug.Log("✅ Botão 'Iniciar' configurado!");
        }
        else
        {
            Debug.LogError("❌ Nenhum botão encontrado no canvas inicial!");
        }

        // Verifica se o XRRigMover está configurado
        if (xrRigMover == null)
        {
            xrRigMover = FindObjectOfType<XRRigMover>();
            if (xrRigMover != null)
                Debug.Log("✅ XRRigMover encontrado automaticamente!");
            else
                Debug.LogError("❌ XRRigMover não encontrado na cena!");
        }
    }

    public void OnStartButtonClick()
    {
        Debug.Log("🎮 Botão Iniciar pressionado!");

        if (xrRigMover != null)
        {
            // Esconde o canvas inicial
            if (hideCanvasOnStart)
            {
                gameObject.SetActive(false);
                Debug.Log("📺 Canvas inicial desativado");
            }

            // Move para o próximo planeta (do índice 0 para 1)
            if (delayBeforeMove > 0)
            {
                Invoke(nameof(MoveToFirstPlanet), delayBeforeMove);
            }
            else
            {
                MoveToFirstPlanet();
            }
        }
        else
        {
            Debug.LogError("❌ XRRigMover não está configurado!");
        }
    }

    void MoveToFirstPlanet()
    {
        xrRigMover.NextPlanet();
        Debug.Log("🚀 Movendo para o primeiro planeta!");
    }
}