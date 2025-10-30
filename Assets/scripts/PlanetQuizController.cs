using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;

public class PlanetQuizController : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup quizCanvas;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI feedbackText;

    [Header("Perguntas do planeta")]
    [TextArea] public string[] questions;
    public string[][] options;
    public int[] correctAnswers;

    private int currentQuestionIndex = 0;
    private bool questionActive = false;

    private SolarSystemManager solarSystemManager;

    void Start()
    {
        solarSystemManager = FindObjectOfType<SolarSystemManager>();
        quizCanvas.alpha = 0;
        feedbackText.gameObject.SetActive(false);
    }

    public void StartQuizAfterAudio()
    {
        currentQuestionIndex = 0;
        ShowQuestion(currentQuestionIndex);
    }

    private void ShowQuestion(int index)
    {
        if (index >= questions.Length)
        {
            // Se respondeu todas, vai para o próximo planeta
            StartCoroutine(FinishAndNextPlanet());
            return;
        }

        questionText.text = questions[index];

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int optIndex = i;
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[index][i];
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => SelectAnswer(optIndex));
        }

        StartCoroutine(FadeIn(quizCanvas));
        questionActive = true;
    }

    private void SelectAnswer(int index)
    {
        if (!questionActive) return;
        questionActive = false;

        bool correct = index == correctAnswers[currentQuestionIndex];
        feedbackText.text = correct ? "✔️ Correto!" : "❌ Errado!";
        feedbackText.color = correct ? Color.green : Color.red;
        feedbackText.gameObject.SetActive(true);

        StartCoroutine(NextQuestionDelay(correct));
    }

    private IEnumerator NextQuestionDelay(bool correct)
    {
        yield return new WaitForSeconds(2f);
        feedbackText.gameObject.SetActive(false);

        StartCoroutine(FadeOut(quizCanvas));
        yield return new WaitForSeconds(1f);

        if (correct)
            currentQuestionIndex++;

        ShowQuestion(currentQuestionIndex);
    }

    private IEnumerator FinishAndNextPlanet()
    {
        feedbackText.text = "🌎 Excelente! Vamos para o próximo planeta!";
        feedbackText.color = Color.cyan;
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        feedbackText.gameObject.SetActive(false);
        StartCoroutine(FadeOut(quizCanvas));
        yield return new WaitForSeconds(0.8f);

        solarSystemManager.NextPlanet();
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        float d = 0.8f;
        for (float t = 0; t < d; t += Time.deltaTime)
        {
            cg.alpha = Mathf.Lerp(0, 1, t / d);
            yield return null;
        }
        cg.alpha = 1;
    }

    private IEnumerator FadeOut(CanvasGroup cg)
    {
        float d = 0.8f;
        for (float t = 0; t < d; t += Time.deltaTime)
        {
            cg.alpha = Mathf.Lerp(1, 0, t / d);
            yield return null;
        }
        cg.alpha = 0;
    }
}
