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

    public Canvas quizCanvas;             // Canvas do quiz
    private int currentQuestionIndex = 0;
    private bool quizActivated = false;
    private XRRigMover rigMover;

    void Start()
    {
        // Garante que o quiz comece desativado
        rigMover = FindObjectOfType<XRRigMover>();
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
            nextPlanetButton.onClick.RemoveAllListeners();
            nextPlanetButton.onClick.AddListener(OnNextPlanet);
        }
        else
        {
            Debug.LogWarning("⚠️ O botão 'Next Planet' não foi atribuído no PlanetQuiz.");
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

        feedbackText.text = "";
        nextPlanetButton.gameObject.SetActive(false);

        // Move a câmera
        if (rigMover != null)
            rigMover.NextPlanet(); ;
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
