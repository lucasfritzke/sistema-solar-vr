using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;

public class SolarVRSimpleUI : MonoBehaviour
{
    [Header("Painel de informações do planeta")]
    [SerializeField] private CanvasGroup infoPanel;
    [SerializeField] private TextMeshProUGUI planetNameText;

    [Header("Painel de perguntas")]
    [SerializeField] private CanvasGroup questionPanel;
    [SerializeField] private TextMeshProUGUI questionTitleText;
    [SerializeField] private TextMeshProUGUI questionDescriptionText;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Button validateButton;

    private SolarSystemManager solarSystemManager;
    private SolarSystemManager.PlanetCutscene currentPlanet;
    private int currentQuestionIndex = 0;

    void Start()
    {
        solarSystemManager = FindObjectOfType<SolarSystemManager>();

        // Inicialmente esconde os painéis
        SetCanvasGroupVisible(infoPanel, false);
        SetCanvasGroupVisible(questionPanel, false);

        if (validateButton != null)
            validateButton.onClick.AddListener(ValidateAnswer);
    }

    // =====================================================================
    // Exibe apenas o nome do planeta (enquanto toca o áudio)
    // =====================================================================
    public void ShowPlanetUI(SolarSystemManager.PlanetCutscene planet)
    {
        currentPlanet = planet;
        currentQuestionIndex = 0;

        if (planetNameText != null)
            planetNameText.text = planet.planetName;

        StartCoroutine(FadeIn(infoPanel));
        SetCanvasGroupVisible(questionPanel, false);
    }

    // =====================================================================
    // Chamado pelo manager após o áudio terminar
    // =====================================================================
    public void ShowPlanetQuiz(SolarSystemManager.PlanetCutscene planet)
    {
        currentPlanet = planet;
        currentQuestionIndex = 0;

        if (currentPlanet.questions == null || currentPlanet.questions.Count == 0)
        {
            Debug.LogWarning($"[SolarVRSimpleUI] Nenhuma pergunta configurada para {currentPlanet.planetName}!");
            return;
        }

        ShowQuestion(currentQuestionIndex);
    }

    // =====================================================================
    // Mostra a pergunta atual
    // =====================================================================
    private void ShowQuestion(int index)
    {
        if (currentPlanet == null || currentPlanet.questions.Count <= index)
            return;

        var q = currentPlanet.questions[index];
        questionTitleText.text = $"Pergunta {index + 1}";
        questionDescriptionText.text = q.question;

        // Limpa os toggles antigos
        foreach (Transform child in toggleGroup.transform)
            Destroy(child.gameObject);

        // Cria novos toggles para cada alternativa
        foreach (var alt in q.alternatives)
        {
            GameObject toggleObj = new GameObject("OptionToggle", typeof(RectTransform), typeof(Toggle), typeof(TextMeshProUGUI));
            toggleObj.transform.SetParent(toggleGroup.transform, false);

            Toggle toggle = toggleObj.GetComponent<Toggle>();
            toggle.group = toggleGroup;
            toggle.isOn = false;

            TextMeshProUGUI label = toggleObj.GetComponent<TextMeshProUGUI>();
            label.text = alt;
            label.fontSize = 24;
            label.color = Color.white;

            // Ajusta layout
            RectTransform rt = toggleObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(800, 40);
        }

        // Mostra o painel de perguntas
        StartCoroutine(FadeIn(questionPanel));
    }

    // =====================================================================
    // Quando o jogador clica em "Validar"
    // =====================================================================
    private void ValidateAnswer()
    {
        Toggle selected = toggleGroup.ActiveToggles().FirstOrDefault();
        if (selected == null)
        {
            Debug.Log("Nenhuma opção selecionada.");
            return;
        }

        int selectedIndex = selected.transform.GetSiblingIndex();
        bool correct = (selectedIndex == currentPlanet.questions[currentQuestionIndex].corretAnswer);

        if (correct)
        {
            Debug.Log($"[Quiz] ✅ Resposta correta para {currentPlanet.planetName} / Pergunta {currentQuestionIndex + 1}");

            StartCoroutine(HandleCorrectAnswer());
        }
        else
        {
            Debug.Log($"[Quiz] ❌ Resposta incorreta para {currentPlanet.planetName}");
        }
    }

    private IEnumerator HandleCorrectAnswer()
    {
        yield return new WaitForSeconds(1f);

        currentQuestionIndex++;

        if (currentQuestionIndex < currentPlanet.questions.Count)
        {
            // Próxima pergunta
            StartCoroutine(FadeOut(questionPanel, () =>
            {
                ShowQuestion(currentQuestionIndex);
            }));
        }
        else
        {
            // Todas as perguntas concluídas -> próximo planeta
            Debug.Log($"[Quiz] ✅ Todas perguntas concluídas para {currentPlanet.planetName}");
            StartCoroutine(FadeOut(questionPanel, () =>
            {
                solarSystemManager.NextPlanet();
            }));
        }
    }

    // =====================================================================
    // Utilitários de fade e visibilidade
    // =====================================================================
    private void SetCanvasGroupVisible(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }

    private IEnumerator FadeIn(CanvasGroup cg, float duration = 1f)
    {
        if (cg == null) yield break;
        cg.gameObject.SetActive(true);
        cg.blocksRaycasts = true;
        float t = 0f;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(0, 1, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1;
    }

    private IEnumerator FadeOut(CanvasGroup cg, System.Action onComplete = null, float duration = 0.5f)
    {
        if (cg == null) yield break;
        float t = 0f;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(1, 0, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        cg.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
