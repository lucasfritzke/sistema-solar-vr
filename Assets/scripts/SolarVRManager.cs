using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gerenciador principal que integra todo o sistema VR do Sistema Solar
/// Controla: posicionamento da câmera, áudio, quiz e transição entre planetas
/// </summary>
public class SolarVRManager : MonoBehaviour
{
    [System.Serializable]
    public class PlanetData
    {
        [Header("Identificação")]
        public string planetName;
        public Transform planetTransform; // Transform do planeta na cena
        
        [Header("Câmera")]
        public Transform cameraPosition; // Posição da câmera para este planeta
        public float cameraTransitionSpeed = 2f; // Velocidade de transição da câmera
        
        [Header("Áudio")]
        public AudioClip planetAudio; // Áudio explicativo (máximo 1 minuto)
        public float audioTriggerDistance = 50f; // Distância para começar o áudio
        public Button replayAudioButton; // Botão para repetir áudio
        
        [Header("Quiz")]
        public Canvas quizCanvas; // Canvas do quiz para este planeta
        public TextMeshProUGUI questionText; // Texto da pergunta
        public Button[] optionButtons; // Botões das alternativas (2 alternativas)
        public Button nextButton; // Botão "Próximo" (habilitado quando acertar)
        public TextMeshProUGUI feedbackText; // Texto de feedback
        
        [Header("Pergunta do Quiz")]
        [TextArea(2, 4)]
        public string question;
        public string optionA;
        public string optionB;
        public int correctAnswer; // 0 = A, 1 = B
    }

    [Header("Configurações Gerais")]
    public Transform mainCamera; // Câmera principal (Cardboard)
    public Transform initialCameraPosition; // Posição inicial (Sol)
    public SolarVRGaze gazeController; // Controller de gaze/raycast
    public AudioSource audioSource; // AudioSource para tocar áudios dos planetas
    
    [Header("Reticle UI")]
    public Image reticleFill; // Imagem do reticle preenchido
    public Image reticlePoint; // Ponto do reticle
    
    [Header("Planetas (em ordem de visita)")]
    public List<PlanetData> planets = new List<PlanetData>();
    
    private int currentPlanetIndex = -1; // -1 = Sol (posição inicial)
    private bool audioPlaying = false;
    private bool quizActive = false;
    private Coroutine cameraTransitionCoroutine;

    void Start()
    {
        // Validações iniciais
        if (mainCamera == null)
            mainCamera = Camera.main.transform;
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        // Inicia na posição do Sol
        if (initialCameraPosition != null && mainCamera != null)
        {
            mainCamera.position = initialCameraPosition.position;
            mainCamera.rotation = initialCameraPosition.rotation;
        }
        
        // Configura todos os planetas
        InitializePlanets();
        
        Debug.Log("✅ SolarVRManager iniciado - Posicionado no Sol");
    }

    void InitializePlanets()
    {
        foreach (PlanetData planet in planets)
        {
            // Esconde quiz inicialmente
            if (planet.quizCanvas != null)
            {
                CanvasGroup cg = planet.quizCanvas.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = planet.quizCanvas.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
            
            // Desabilita botão próximo inicialmente
            if (planet.nextButton != null)
            {
                planet.nextButton.interactable = false;
                planet.nextButton.gameObject.SetActive(false);
                
                // Configura listener do botão próximo
                planet.nextButton.onClick.RemoveAllListeners();
                planet.nextButton.onClick.AddListener(() => OnNextPlanetClicked());
            }
            
            // Configura botões de opções
            if (planet.optionButtons != null && planet.optionButtons.Length >= 2)
            {
                for (int i = 0; i < planet.optionButtons.Length && i < 2; i++)
                {
                    int index = i; // Captura para closure
                    planet.optionButtons[i].onClick.RemoveAllListeners();
                    planet.optionButtons[i].onClick.AddListener(() => OnAnswerSelected(index, planet));
                }
            }
            
            // Configura botão de repetir áudio
            if (planet.replayAudioButton != null)
            {
                planet.replayAudioButton.onClick.RemoveAllListeners();
                planet.replayAudioButton.onClick.AddListener(() => PlayPlanetAudio(planet));
            }
            
            // Esconde feedback inicialmente
            if (planet.feedbackText != null)
                planet.feedbackText.gameObject.SetActive(false);
        }
    }

    public void StartJourney()
    {
        // Começa a jornada indo para o primeiro planeta
        if (planets.Count > 0)
        {
            GoToPlanet(0);
        }
    }

    void GoToPlanet(int planetIndex)
    {
        if (planetIndex < 0 || planetIndex >= planets.Count)
        {
            Debug.LogWarning("Índice de planeta inválido: " + planetIndex);
            return;
        }

        PlanetData planet = planets[planetIndex];
        currentPlanetIndex = planetIndex;

        Debug.Log($"🌍 Indo para: {planet.planetName}");

        // Move câmera para o planeta
        if (planet.cameraPosition != null && mainCamera != null)
        {
            if (cameraTransitionCoroutine != null)
                StopCoroutine(cameraTransitionCoroutine);
            
            cameraTransitionCoroutine = StartCoroutine(MoveCameraToPlanet(planet));
        }

        // Para áudio anterior se estiver tocando
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        // Inicia processo: áudio -> quiz
        StartCoroutine(PlanetSequence(planet));
    }

    IEnumerator MoveCameraToPlanet(PlanetData planet)
    {
        Vector3 startPos = mainCamera.position;
        Quaternion startRot = mainCamera.rotation;
        Vector3 targetPos = planet.cameraPosition.position;
        Quaternion targetRot = planet.cameraPosition.rotation;

        float elapsed = 0f;
        float duration = 1f / planet.cameraTransitionSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            mainCamera.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.rotation = Quaternion.Lerp(startRot, targetRot, t);
            
            yield return null;
        }

        mainCamera.position = targetPos;
        mainCamera.rotation = targetRot;
        
        Debug.Log($"✅ Câmera chegou em {planet.planetName}");
    }

    IEnumerator PlanetSequence(PlanetData planet)
    {
        // Espera câmera chegar (movimento é assíncrono)
        while (cameraTransitionCoroutine != null)
            yield return new WaitForSeconds(0.1f);
        
        // Aguarda um momento adicional para estabilizar
        yield return new WaitForSeconds(0.5f);

        // 1. Toca áudio se existir
        if (planet.planetAudio != null && audioSource != null)
        {
            PlayPlanetAudio(planet);
            
            // Espera áudio terminar
            yield return new WaitWhile(() => audioSource.isPlaying);
            
            // Pequeno delay após áudio
            yield return new WaitForSeconds(0.5f);
        }

        // 2. Mostra quiz após áudio terminar (ou imediatamente se não houver áudio)
        ShowQuiz(planet);
    }

    void PlayPlanetAudio(PlanetData planet)
    {
        if (planet.planetAudio == null || audioSource == null)
        {
            Debug.LogWarning($"Áudio não configurado para {planet.planetName}");
            return;
        }

        audioSource.Stop();
        audioSource.clip = planet.planetAudio;
        audioSource.Play();
        audioPlaying = true;

        Debug.Log($"🔊 Tocando áudio de {planet.planetName} (duração: {planet.planetAudio.length}s)");
    }

    void ShowQuiz(PlanetData planet)
    {
        if (planet.quizCanvas == null)
        {
            Debug.LogWarning($"Quiz canvas não configurado para {planet.planetName}");
            return;
        }

        quizActive = true;

        // Atualiza pergunta e opções
        if (planet.questionText != null)
            planet.questionText.text = planet.question;

        if (planet.optionButtons != null && planet.optionButtons.Length >= 2)
        {
            if (planet.optionButtons[0] != null)
            {
                TextMeshProUGUI textA = planet.optionButtons[0].GetComponentInChildren<TextMeshProUGUI>();
                if (textA != null) textA.text = planet.optionA;
            }

            if (planet.optionButtons[1] != null)
            {
                TextMeshProUGUI textB = planet.optionButtons[1].GetComponentInChildren<TextMeshProUGUI>();
                if (textB != null) textB.text = planet.optionB;
            }
        }

        // Mostra quiz com fade in
        CanvasGroup cg = planet.quizCanvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = planet.quizCanvas.gameObject.AddComponent<CanvasGroup>();

        StartCoroutine(FadeInCanvas(cg));

        Debug.Log($"📋 Quiz de {planet.planetName} mostrado");
    }

    void OnAnswerSelected(int answerIndex, PlanetData planet)
    {
        if (!quizActive) return;

        bool isCorrect = (answerIndex == planet.correctAnswer);

        if (planet.feedbackText != null)
        {
            planet.feedbackText.gameObject.SetActive(true);
            planet.feedbackText.text = isCorrect ? "✅ Correto!" : "❌ Errado! Tente novamente.";
            planet.feedbackText.color = isCorrect ? Color.green : Color.red;
        }

        if (isCorrect)
        {
            // Acertou: habilita botão Próximo
            quizActive = false;
            
            if (planet.nextButton != null)
            {
                planet.nextButton.gameObject.SetActive(true);
                planet.nextButton.interactable = true;
                
                // Garante que o botão tenha a tag Selectable para o raycast funcionar
                if (!planet.nextButton.gameObject.CompareTag("Selectable"))
                    planet.nextButton.gameObject.tag = "Selectable";
            }

            Debug.Log($"✅ Resposta correta! Botão Próximo habilitado.");
        }
        else
        {
            // Errou: permite nova tentativa após feedback
            StartCoroutine(ResetQuizAfterFeedback(planet));
        }
    }

    IEnumerator ResetQuizAfterFeedback(PlanetData planet)
    {
        yield return new WaitForSeconds(2f);
        
        if (planet.feedbackText != null)
            planet.feedbackText.gameObject.SetActive(false);
    }

    void OnNextPlanetClicked()
    {
        if (currentPlanetIndex < 0 || currentPlanetIndex >= planets.Count)
            return;

        PlanetData currentPlanet = planets[currentPlanetIndex];

        // Esconde quiz atual
        if (currentPlanet.quizCanvas != null)
        {
            CanvasGroup cg = currentPlanet.quizCanvas.GetComponent<CanvasGroup>();
            if (cg != null)
                StartCoroutine(FadeOutCanvas(cg));
        }

        // Reseta feedback
        if (currentPlanet.feedbackText != null)
            currentPlanet.feedbackText.gameObject.SetActive(false);

        // Vai para próximo planeta
        int nextIndex = currentPlanetIndex + 1;
        if (nextIndex < planets.Count)
        {
            GoToPlanet(nextIndex);
        }
        else
        {
            Debug.Log("🌟 Jornada completa! Todos os planetas visitados!");
            // Opcional: voltar ao Sol ou mostrar mensagem final
        }
    }

    IEnumerator FadeInCanvas(CanvasGroup cg)
    {
        if (cg == null) yield break;

        cg.gameObject.SetActive(true);
        cg.blocksRaycasts = true;
        cg.interactable = true;

        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    IEnumerator FadeOutCanvas(CanvasGroup cg)
    {
        if (cg == null) yield break;

        float duration = 0.5f;
        float elapsed = 0f;
        float startAlpha = cg.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    // Método público para iniciar a jornada a partir de qualquer lugar
    public void BeginJourney()
    {
        StartJourney();
    }
}

