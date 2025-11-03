using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlanetQuiz : MonoBehaviour
{
    [Header("Perguntas e respostas")]
    public GameObject[] questionPanels;   // Cada pergunta é um painel com botões
    public TMP_Text feedbackText;         // Texto de feedback (Correto/Incorreto)
    public Button nextPlanetButton;       // Botão "Próximo Planeta"

    [Header("Cores de Feedback")]
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color retryColor = new Color(1f, 0.64f, 0f);

    private GameManager gameManager;
    public Canvas quizCanvas;             // Canvas do quiz
    private int currentQuestionIndex = 0;
    private bool quizActivated = false;

    void Start()
    {
        // Garante que o quiz comece desativado
        if (quizCanvas != null)
            quizCanvas.gameObject.SetActive(false);

        // Esconde todas as perguntas e o feedback
        HideAllQuestions();

        if (feedbackText != null)
            feedbackText.text = "";

        // Garante que o botão "Próximo Planeta" comece oculto
        if (nextPlanetButton != null)
        {
            nextPlanetButton.gameObject.SetActive(false);

            // Remove listeners antigos e adiciona o correto
            nextPlanetButton.onClick.RemoveAllListeners();
            nextPlanetButton.onClick.AddListener(OnNextPlanet);
        }
        else
        {
            Debug.LogWarning("⚠️ O botão 'Next Planet' não foi atribuído no PlanetQuiz.");
        }

      
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
            Debug.LogWarning("⚠️ Nenhum GameManager encontrado na cena!");
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
            feedbackText.text = "Correto!";
            feedbackText.color = correctColor;
            Invoke(nameof(NextQuestion), 1.2f);
        }
        else
        {
            feedbackText.text = "Resposta incorreta! Tente novamente.";
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
        feedbackText.text = "";
        nextPlanetButton.gameObject.SetActive(false);
        HideAllQuestions();
        quizCanvas.gameObject.SetActive(false); // opcional: fecha quiz ao mudar de planeta
        if (gameManager != null)
            Debug.LogWarning("Indo para próximo planeta");
            gameManager.NextPlanet();
    }

    public void ActivateQuiz()
    {
        if (quizActivated) return; // evita múltiplas ativações
        quizActivated = true;

        if (quizCanvas != null)
            quizCanvas.gameObject.SetActive(true); // ativa o Canvas imediatamente

        currentQuestionIndex = 0;
        feedbackText.text = "";
        HideAllQuestions();

        // Mostra a primeira pergunta direto, sem delay
        ShowCurrentQuestion();
    }
}
