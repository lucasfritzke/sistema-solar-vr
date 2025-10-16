using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SolarVRSimpleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI planetNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private CanvasGroup infoPanel;

    private SolarSystemManager solarSystemManager;

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

        Invoke("ShowCurrentPlanetInfo", 3.5f);
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

        int index = solarSystemManager.GetCurrentPlanetIndex();
        SolarSystemManager.PlanetCutscene planet = solarSystemManager.planets[index];

        planetNameText.text = planet.planetName;
        descriptionText.text = planet.description;

        // Mostra o botão se não estiver no modo panorâmico
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);

        StartCoroutine(FadeIn());
    }

    System.Collections.IEnumerator FadeIn()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            infoPanel.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return null;
        }

        infoPanel.alpha = 1;
    }

    System.Collections.IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            infoPanel.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            yield return null;
        }

        infoPanel.alpha = 0;

        solarSystemManager.NextPlanet();

        Invoke("ShowCurrentPlanetInfo", 3.5f);
    }
}