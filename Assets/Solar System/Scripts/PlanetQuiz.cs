using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetQuiz : MonoBehaviour
{
    [Header("Perguntas e respostas")]
    public GameObject[] questionPanels;   // Cada pergunta é um painel com botões
    public TMP_Text feedbackText;         // Texto de feedback (Correto/Incorreto)
    public Button nextPlanetButton;       // Botão "Próximo Planeta"
    public Button replayAudioButton;      // Botão "Tocar Áudio Novamente"

    [Header("Cores de Feedback")]
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color retryColor = new Color(1f, 0.64f, 0f);

    [Header("Áudio")]
    public AudioSource audioSource;       // AudioSource para tocar o áudio do planeta

    public Canvas quizCanvas;             // Canvas do quiz
    private int currentQuestionIndex = 0;
    private bool quizActivated = false;
    private XRRigMover rigMover;
    private AudioClip planetAudioClip;
    private bool audioFinished = false;
    private bool audioStarted = false;
    private Coroutine audioCoroutine;

    // Mapeamento de índices de planetas para nomes de áudio
    private static readonly string[] planetAudioNames = {
        "sol", "mercurio", "venus", "terra", "marte", "jupter", "saturno","urano", "netuno"
    };

    void Start()
    {
        // Garante que o quiz comece desativado
        rigMover = FindObjectOfType<XRRigMover>();

        // Busca automaticamente os componentes se não foram atribuídos
        FindComponentsIfNeeded();

        if (quizCanvas != null)
            quizCanvas.gameObject.SetActive(false);

        // Esconde todas as perguntas e o feedback
        HideAllQuestions();

        if (feedbackText != null)
            feedbackText.text = "";

        // Configura AudioSource se não foi atribuídoI
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Garante que o botão "Próximo Planeta" comece oculto
        if (nextPlanetButton != null)
        {
            nextPlanetButton.gameObject.SetActive(false);
            nextPlanetButton.onClick.RemoveAllListeners();
            nextPlanetButton.onClick.AddListener(OnNextPlanet);
        }
        ConfigureReplayButton();
    }

    void ConfigureReplayButton()
    {
        replayAudioButton.onClick.RemoveAllListeners();
        replayAudioButton.onClick.AddListener(OnReplayAudioButtonClicked);

        // Garante que o botão está interativo
        replayAudioButton.interactable = true;
    }

    void OnReplayAudioButtonClicked()
    {

        if (planetAudioClip != null)
            PlayPlanetAudio();
    }

    void FindComponentsIfNeeded()
    {
        // Busca o Canvas se não foi atribuído
        if (quizCanvas == null)
        {
            quizCanvas = GetComponentInChildren<Canvas>(true);
            if (quizCanvas == null)
            {
                // Tenta buscar no objeto pai
                quizCanvas = GetComponentInParent<Canvas>(true);
            }
        }

        // Se encontrou o Canvas, busca os componentes filhos
        if (quizCanvas != null)
        {
            // Busca o feedbackText se não foi atribuído
            if (feedbackText == null)
            {
                // Procura por nome "FeedBackText" (case insensitive)
                Transform feedbackTransform = quizCanvas.transform.Find("FeedBackText");
                if (feedbackTransform == null)
                {
                    // Tenta buscar em todos os filhos (case insensitive)
                    foreach (Transform child in quizCanvas.transform.GetComponentsInChildren<Transform>(true))
                    {
                        if (child.name.Equals("FeedBackText", System.StringComparison.OrdinalIgnoreCase))
                        {
                            feedbackTransform = child;
                            break;
                        }
                    }
                }

                if (feedbackTransform != null)
                {
                    feedbackText = feedbackTransform.GetComponent<TMP_Text>();
                }

                if (feedbackText == null)
                {
                    // Última tentativa: busca qualquer TMP_Text que contenha "feedback" no nome
                    TMP_Text[] allTexts = quizCanvas.GetComponentsInChildren<TMP_Text>(true);
                    foreach (TMP_Text text in allTexts)
                    {
                        if (text.name.ToLower().Contains("feedback"))
                        {
                            feedbackText = text;
                            break;
                        }
                    }
                }
            }

            // Busca o nextPlanetButton se não foi atribuído
            if (nextPlanetButton == null)
            {
                // Procura por nome "NextPlanetButton"
                Transform buttonTransform = quizCanvas.transform.Find("NextPlanetButton");
                if (buttonTransform == null)
                {
                    // Tenta buscar em todos os filhos (case insensitive)
                    foreach (Transform child in quizCanvas.transform.GetComponentsInChildren<Transform>(true))
                    {
                        if (child.name.Equals("NextPlanetButton", System.StringComparison.OrdinalIgnoreCase))
                        {
                            buttonTransform = child;
                            break;
                        }
                    }
                }

                if (buttonTransform != null)
                {
                    nextPlanetButton = buttonTransform.GetComponent<Button>();
                }
            }

            // Busca o replayAudioButton se não foi atribuído
            if (replayAudioButton == null)
            {
                // Procura por nomes comuns: "ReplayAudioButton", "ReplayAudio", "Replay Button", etc.
                Button[] allButtons = quizCanvas.GetComponentsInChildren<Button>(true);
                foreach (Button btn in allButtons)
                {
                    string btnName = btn.name.ToLower();
                    // Ignora botões que não são de replay
                    if (btnName.Contains("next") || btnName.Contains("option") || btnName.Contains("feedback"))
                        continue;

                    // Procura por botões que contenham "replay" ou "audio" no nome
                    if (btnName.Contains("replay") || (btnName.Contains("audio") && !btnName.Contains("next")))
                    {
                        replayAudioButton = btn;
                        break;
                    }
                }

                // Se não encontrou por nome, tenta buscar por texto do botão
                if (replayAudioButton == null)
                {
                    foreach (Button btn in allButtons)
                    {
                        string btnName = btn.name.ToLower();
                        if (btnName.Contains("next") || btnName.Contains("option"))
                            continue;

                        // Verifica o texto do botão
                        TMP_Text buttonText = btn.GetComponentInChildren<TMP_Text>(true);
                        if (buttonText != null)
                        {
                            string textContent = buttonText.text.ToLower();
                            if (textContent.Contains("replay") || textContent.Contains("tocar") || textContent.Contains("audio"))
                            {
                                replayAudioButton = btn;
                                break;
                            }
                        }
                    }
                }
            }

            // Busca os questionPanels se não foram atribuídos
            if (questionPanels == null || questionPanels.Length == 0)
            {
                List<GameObject> panels = new List<GameObject>();

                // Busca botões de opção (Button_Option1, Button_Option2, etc.)
                Button[] allButtons = quizCanvas.GetComponentsInChildren<Button>(true);
                HashSet<GameObject> foundPanels = new HashSet<GameObject>();

                foreach (Button btn in allButtons)
                {
                    string btnName = btn.name.ToLower();
                    // Ignora botões que não são de opção
                    if (btnName.Contains("next") || btnName.Contains("replay"))
                        continue;

                    // Se o botão tem "option" no nome, encontra seu container pai
                    if (btnName.Contains("option"))
                    {
                        // Encontra o container pai (que deve ser o painel)
                        // O painel geralmente é um filho direto do Canvas ou um nível acima dos botões
                        Transform parent = btn.transform.parent;

                        // Procura o container que contém os botões de opção
                        // Geralmente é um GameObject que tem múltiplos botões como filhos
                        while (parent != null && parent != quizCanvas.transform)
                        {
                            // Verifica se este pai tem outros botões de opção como irmãos
                            int optionButtonCount = 0;
                            foreach (Transform sibling in parent.GetComponentsInChildren<Transform>(false))
                            {
                                if (sibling != parent && sibling.GetComponent<Button>() != null)
                                {
                                    string siblingName = sibling.name.ToLower();
                                    if (siblingName.Contains("option"))
                                    {
                                        optionButtonCount++;
                                    }
                                }
                            }

                            // Se encontrou um container com pelo menos 2 botões de opção, é um painel
                            if (optionButtonCount >= 2)
                            {
                                if (!foundPanels.Contains(parent.gameObject))
                                {
                                    foundPanels.Add(parent.gameObject);
                                    panels.Add(parent.gameObject);
                                }
                                break;
                            }

                            parent = parent.parent;
                        }
                    }
                }

                // Se não encontrou painéis pelos botões, procura por nome
                if (panels.Count == 0)
                {
                    // Procura por GameObjects com "panel" ou "question" no nome que sejam filhos do Canvas
                    // Ou filhos de filhos (para estruturas mais aninhadas)
                    foreach (Transform child in quizCanvas.transform.GetComponentsInChildren<Transform>(true))
                    {
                        string childName = child.name.ToLower();
                        // Procura por objetos que contenham "panel" e "question" no nome
                        // ou apenas "panel_question" ou padrões similares
                        if ((childName.Contains("question") && childName.Contains("panel"))
                            || (childName.Contains("panel") && (childName.Contains("question") || childName.Contains("quiz")))
                            || childName.StartsWith("panel_question")
                            || childName.StartsWith("questionpanel"))
                        {
                            // Verifica se não é um botão ou texto, mas sim um container
                            if (child.GetComponent<Button>() == null && child.GetComponent<TMP_Text>() == null)
                            {
                                if (!panels.Contains(child.gameObject))
                                {
                                    panels.Add(child.gameObject);
                                }
                            }
                        }
                    }
                }

                if (panels.Count > 0)
                {
                    questionPanels = panels.ToArray();
                }                  
            }
        }
    }

    void HideAllQuestions()
    {
        foreach (var panel in questionPanels)
            panel.SetActive(false);
    }

    void ShowCurrentQuestion()
    {
        for (int i = 0; i < questionPanels.Length; i++)
        {
            bool active = i == currentQuestionIndex;
            questionPanels[i].SetActive(active);

        }
    }


    public void OnAnswerSelected(bool isCorrect)
    {
        if (isCorrect)
        {
            feedbackText.text = "Parabéns você acertou!";
            feedbackText.color = correctColor;
            Invoke(nameof(NextQuestion), 1.2f);
        }
        else
        {
            feedbackText.text = "Tente novamente!";
            feedbackText.color = incorrectColor;
            Invoke(nameof(RepeatOrReset), 1.8f);
        }
    }

    void NextQuestion()
    {
        feedbackText.text = "";
        currentQuestionIndex++;

        if (currentQuestionIndex >= questionPanels.Length)
            ShowEndQuiz();
        else
            ShowCurrentQuestion();
    }

    void RepeatOrReset()
    {
        feedbackText.text = "";
        ShowCurrentQuestion();
    }

    void ShowEndQuiz()
    {
        HideAllQuestions();
        feedbackText.text = "Parabéns! Você concluiu as perguntas!";
        feedbackText.color = correctColor;
        nextPlanetButton.gameObject.SetActive(true);
    }

    public void OnNextPlanet()
    {
        // Oculta o quiz imediatamente antes de mover
        if (quizCanvas != null)
            quizCanvas.gameObject.SetActive(false);

        if (feedbackText != null)
            feedbackText.text = "";
        if (nextPlanetButton != null)
            nextPlanetButton.gameObject.SetActive(false);

        // Para o áudio se estiver tocando
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }

        // Reseta o estado para o próximo planeta
        ResetQuiz();

        // Move a câmera
        if (rigMover != null)
            rigMover.NextPlanet();
    }

    // Chamado quando a câmera chega perto do planeta
    public void OnCameraArrived()
    {
        // Se o áudio ainda não foi iniciado, inicia agora
        if (!audioStarted)
        {
            LoadAndPlayPlanetAudio();
        }
    }

    public void ActivateQuiz()
    {
        if (quizActivated) return; // evita múltiplas ativações
        quizActivated = true;

        // Se o áudio ainda não foi iniciado, inicia agora
        if (!audioStarted)
        {
            LoadAndPlayPlanetAudio();
        }

        // Se o áudio já terminou, mostra o quiz imediatamente
        if (audioFinished)
        {
            ShowQuiz();
        }
        // Caso contrário, o quiz será mostrado quando o áudio terminar (na corrotina)
    }

    void LoadAndPlayPlanetAudio()
    {
        Debug.Log("[PlanetQuiz] LoadAndPlayPlanetAudio chamado");

        // Marca como iniciado, mas permite recarregar se necessário (para replay)
        if (!audioStarted)
        {
            audioStarted = true;
        }

        // Tenta identificar o planeta através do PlanetIdentifier
        PlanetIdentifier planetId = GetComponentInParent<PlanetIdentifier>();
        int planetIndex = -1;

        if (planetId != null)
        {
            planetIndex = planetId.planetIndex;
        }
        else if (rigMover != null)
        {
            // Tenta identificar através do XRRigMover usando o Transform do planeta
            Transform planetTransform = transform.root;
            planetIndex = rigMover.GetIndexByReference(planetTransform);
        }

        // Se ainda não encontrou, tenta pelo nome do GameObject
        if (planetIndex == -1)
        {
            string planetName = gameObject.name.ToLower();
            for (int i = 0; i < planetAudioNames.Length; i++)
            {
                if (planetName.Contains(planetAudioNames[i]))
                {
                    planetIndex = i;
                    break;
                }
            }
        }

        // Carrega o áudio
        if (planetIndex >= 0 && planetIndex < planetAudioNames.Length)
        {
            string audioName = planetAudioNames[planetIndex];
            planetAudioClip = Resources.Load<AudioClip>(audioName);

            if (planetAudioClip != null)
            {
                Debug.Log($"[PlanetQuiz] Áudio carregado: {audioName}");
                PlayPlanetAudio();
            }
            else
            {
                Debug.LogWarning($"[PlanetQuiz] Áudio não encontrado: {audioName}");
                // Se não encontrou o áudio, marca como finalizado
                audioFinished = true;
            }
        }
        else
        {
            Debug.LogWarning($"[PlanetQuiz] Não foi possível identificar o planeta. Índice: {planetIndex}");
            // Se não identificou o planeta, marca como finalizado
            audioFinished = true;
        }
    }

    void PlayPlanetAudio()
    {
        Debug.Log("[PlanetQuiz] PlayPlanetAudio chamado");

        // Se o áudio ainda não foi carregado, tenta carregar primeiro
        if (planetAudioClip == null)
        {
            Debug.Log("[PlanetQuiz] Áudio não carregado ainda, tentando carregar...");
            // Se ainda não foi iniciado, carrega o áudio
            if (!audioStarted)
            {
                LoadAndPlayPlanetAudio();
                return;
            }
            else
            {
                // Já foi iniciado mas o clip foi perdido, recarrega
                LoadAndPlayPlanetAudio();
                return;
            }
        }

        if (planetAudioClip != null && audioSource != null)
        {
            Debug.Log($"[PlanetQuiz] Reproduzindo áudio: {planetAudioClip.name}");
            Debug.Log($"[PlanetQuiz] AudioSource está tocando antes: {audioSource.isPlaying}");

            // Para qualquer áudio que esteja tocando
            audioSource.Stop();

            // Configura o clip
            audioSource.clip = planetAudioClip;

            // Garante que o AudioSource está habilitado
            if (!audioSource.enabled)
            {
                audioSource.enabled = true;
                Debug.Log("[PlanetQuiz] AudioSource estava desabilitado, habilitando agora");
            }

            // Toca o áudio
            audioSource.Play();

            Debug.Log($"[PlanetQuiz] AudioSource está tocando depois: {audioSource.isPlaying}");
            Debug.Log($"[PlanetQuiz] Volume do AudioSource: {audioSource.volume}");
            Debug.Log($"[PlanetQuiz] Duração do áudio: {planetAudioClip.length} segundos");

            // Se o quiz já está ativo e visível, apenas toca o áudio
            // Não precisa esperar para mostrar o quiz (já está visível)
            if (quizActivated && quizCanvas != null && quizCanvas.gameObject.activeSelf)
            {
                Debug.Log("[PlanetQuiz] Quiz já está visível, apenas tocando áudio (não espera terminar)");
                // Para a corrotina anterior se existir
                if (audioCoroutine != null)
                    StopCoroutine(audioCoroutine);
                // Não precisa iniciar nova corrotina, apenas toca o áudio
                return;
            }

            // Se o quiz não está visível ainda, espera o áudio terminar
            if (audioCoroutine != null)
                StopCoroutine(audioCoroutine);
            audioCoroutine = StartCoroutine(WaitForAudioToFinish());
        }
        else
        {
            Debug.LogWarning("[PlanetQuiz] Não é possível tocar o áudio: planetAudioClip ou audioSource é null");
            // Se não há áudio, marca como finalizado
            audioFinished = true;
            // Se o quiz já foi ativado mas ainda não foi mostrado, mostra imediatamente
            if (quizActivated && (quizCanvas == null || !quizCanvas.gameObject.activeSelf))
            {
                ShowQuiz();
            }
        }
    }

    IEnumerator WaitForAudioToFinish()
    {
        // Espera o áudio terminar
        while (audioSource != null && audioSource.isPlaying)
        {
            yield return null;
        }

        // Quando o áudio terminar, marca como finalizado
        audioFinished = true;

        // Após o áudio terminar, o quiz deve aparecer quando o usuário olhar para o planeta
        // (quando ActivateQuiz for chamado via gaze)
        // Se o quiz já foi ativado (usuário já olhou), mostra imediatamente
        if (quizActivated && (quizCanvas == null || !quizCanvas.gameObject.activeSelf))
        {
            ShowQuiz();
        }
    }

    void ShowQuiz()
    {
        // Ativa o Canvas do quiz
        if (quizCanvas != null)
            quizCanvas.gameObject.SetActive(true);

        currentQuestionIndex = 0;
        if (feedbackText != null)
            feedbackText.text = "";
        HideAllQuestions();

        // Mostra a primeira pergunta
        ShowCurrentQuestion();
    }

    public void ResetQuiz()
    {
        quizActivated = false;
        audioFinished = false;
        audioStarted = false;
        currentQuestionIndex = 0;
        if (quizCanvas != null)
            quizCanvas.gameObject.SetActive(false);
        HideAllQuestions();
        if (feedbackText != null)
            feedbackText.text = "";
        if (nextPlanetButton != null)
            nextPlanetButton.gameObject.SetActive(false);
        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }
    }
}
