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

        if (nextButton != null)
            nextButton.onClick.AddListener(() => GoToNextPlanet());

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

        int index = solarSystemManager.GetCurrentPlanetIndex();
        SolarSystemManager.PlanetCutscene planet = solarSystemManager.planets[index];

        planetNameText.text = planet.planetName;
        descriptionText.text = planet.description;

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