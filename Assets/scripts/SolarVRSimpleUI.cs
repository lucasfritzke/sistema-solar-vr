using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class SolarVRSimpleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI planetNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private CanvasGroup infoPanel;

    [SerializeField] private TextMeshProUGUI questionTittle;
    [SerializeField] private TextMeshProUGUI questionDescription;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Button validateButton;
    [SerializeField] private CanvasGroup questionPanel;

    private SolarSystemManager solarSystemManager;

    private int currentQuestion = 0;

    void Start()
    {
        solarSystemManager = GetComponent<SolarSystemManager>();

        // Se nextButton não foi atribuído, tenta encontrar automaticamente
        if (nextButton == null)
        {
            nextButton = FindObjectOfType<Button>();
            Debug.Log(nextButton != null ? "✓ Botão encontrado automaticamente" : "✗ Botão não encontrado");
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() => GoToNextPlanet());
            Debug.Log("✓ Listener do botão adicionado");
        }

        if (infoPanel == null)
        {
            Debug.LogError("✗ InfoPanel não foi atribuído!");
            return;
        }

        if (validateButton != null)
        {
            validateButton.onClick.AddListener(() => ValidateAndGoToNextQuestion());
        }

        Invoke("ShowCurrentPlanetInfo", 3.5f);
    }

    private void ValidateAndGoToNextQuestion()
    {
        
        bool isCorrectToggle = IsToggleOption();
        if (isCorrectToggle)
        {
            //if (currentQuestion == 1)
            //{
            //    validateButton.interactable = false;
            //}
            //else
            //{
                StartCoroutine(FadeInQuestion());
                OnChangeQuestion();
                StartCoroutine(FadeOutQuestion());
            //}
        }
    }

    private bool IsToggleOption()
    {
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        if (activeToggle != null)
        {
            ToggleOption toggleOption = activeToggle.GetComponent<ToggleOption>();
            if (toggleOption != null)
            {
                SolarSystemManager.PlanetCutscene planet = CurrentPlanet();
                return planet.questions[currentQuestion].corretAnswer == toggleOption.indexValue;
            }
        }
        return false;
    }

    void GoToNextPlanet()
    {
        StartCoroutine(FadeOut());
    }

    void ShowCurrentPlanetInfo()
    {
        if (solarSystemManager.planets.Count == 0)
            return;

        // Se estiver no modo panorâmico, mostra mensagem especial
        if (solarSystemManager.IsInPanoramicMode())
        {
            planetNameText.text = "SISTEMA SOLAR";
            descriptionText.text = "Você completou a jornada pelos planetas!\nApreciando a vista panorâmica...";

            // Esconde o botão no modo panorâmico
            if (nextButton != null)
                nextButton.gameObject.SetActive(false);

            StartCoroutine(FadeIn());
            return;
        }

        SolarSystemManager.PlanetCutscene planet = CurrentPlanet();

        planetNameText.text = planet.planetName;
        descriptionText.text = planet.description;
        SetQuestion(planet);

        // Mostra o botão se não estiver no modo panorâmico
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);

        StartCoroutine(FadeIn());
    }

    private SolarSystemManager.PlanetCutscene CurrentPlanet()
    {
        int index = solarSystemManager.GetCurrentPlanetIndex();
        return solarSystemManager.planets[index];
    }

    private void OnChangeQuestion()
    {
        SolarSystemManager.PlanetCutscene planet = CurrentPlanet();
        currentQuestion = (currentQuestion + 1) % planet.questions.Count();
        SetQuestion(planet);
    }

    private void SetQuestion(SolarSystemManager.PlanetCutscene planet)
    {
        questionTittle.text = "Perguntas";
        questionDescription.text = planet.questions[currentQuestion].question;

        RadioGroupController radioGroupController = new RadioGroupController();
        radioGroupController.toggleGroup = toggleGroup;

        radioGroupController.SetQuestions(planet.questions[currentQuestion].alternatives);
    }

    System.Collections.IEnumerator FadeIn()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            infoPanel.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            questionPanel.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return null;
        }

        infoPanel.alpha = 1;
        questionPanel.alpha = 1;
    }

    System.Collections.IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            infoPanel.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            questionPanel.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            yield return null;
        }

        infoPanel.alpha = 0;
        questionPanel.alpha = 0;

        solarSystemManager.NextPlanet();
        currentQuestion = 0;

        Invoke("ShowCurrentPlanetInfo", 3.5f);
    }

    System.Collections.IEnumerator FadeInQuestion()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            questionPanel.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return null;
        }

        questionPanel.alpha = 1;
    }
    System.Collections.IEnumerator FadeOutQuestion()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            questionPanel.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            yield return null;
        }

        questionPanel.alpha = 0;
    }
}