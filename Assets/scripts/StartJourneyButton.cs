using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script simples para iniciar a jornada do Sistema Solar
/// Pode ser usado em um botão ou para iniciar automaticamente
/// </summary>
public class StartJourneyButton : MonoBehaviour
{
    [Header("Referências")]
    public SolarVRManager solarVRManager;
    public Button startButton; // Opcional: botão para iniciar

    [Header("Iniciar Automaticamente")]
    public bool startAutomatically = false;
    public float autoStartDelay = 3f; // Segundos até iniciar automaticamente

    void Start()
    {
        // Encontra o manager se não foi atribuído
        if (solarVRManager == null)
            solarVRManager = FindObjectOfType<SolarVRManager>();

        // Configura botão se existir
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClicked);
        }

        // Inicia automaticamente se configurado
        if (startAutomatically && solarVRManager != null)
        {
            Invoke(nameof(StartJourney), autoStartDelay);
        }
    }

    public void OnStartClicked()
    {
        StartJourney();
        
        // Esconde botão após clicar
        if (startButton != null)
            startButton.gameObject.SetActive(false);
    }

    void StartJourney()
    {
        if (solarVRManager != null)
        {
            Debug.Log("🚀 Iniciando jornada pelo Sistema Solar!");
            solarVRManager.BeginJourney();
        }
        else
        {
            Debug.LogError("❌ SolarVRManager não encontrado! Verifique se está na cena.");
        }
    }
}

