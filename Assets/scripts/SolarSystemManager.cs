using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        [TextArea(1, 3)]
        public string question;
        public string[] alternatives;   // alternativas (ex: 4 alternativas)
        public int corretAnswer;        // índice da alternativa correta (0-based)
    }

    [System.Serializable]
    public class PlanetCutscene
    {
        public string planetName;
        [Tooltip("Referência ao GameObject do planeta na cena (opcional)")]
        public GameObject planetObject;
        [Tooltip("Audio clip explicativo (opcional). Se vazio, o quiz aparece imediatamente.")]
        public AudioClip planetAudio;
        public List<QuestionData> questions = new List<QuestionData>();
    }

    [Header("Planetas")]
    public List<PlanetCutscene> planets = new List<PlanetCutscene>();

    [Header("Referências")]
    [Tooltip("AudioSource usado para tocar as descrições (opcional). Se vazio, não toca áudio.")]
    public AudioSource audioSource;

    [Tooltip("Script UI que mostrará nome/quiz (deve ter método ShowPlanetUI(PlanetCutscene) e ShowPlanetQuiz(PlanetCutscene)")]
    public SolarVRSimpleUI ui;

    private int currentPlanetIndex = 0;

    void Start()
    {
        if (ui == null)
            ui = FindObjectOfType<SolarVRSimpleUI>();

        if (planets == null || planets.Count == 0)
        {
            Debug.LogWarning("SolarSystemManager: nenhum planeta configurado na lista 'planets'.");
            return;
        }

        // Inicia exibindo o primeiro planeta (ajuste se quiser outro comportamento)
        ShowCurrentPlanet();
    }

    // Exibe o planeta atual: ativa objeto, atualiza UI e dispara áudio/quiz
    public void ShowCurrentPlanet()
    {
        if (currentPlanetIndex < 0 || currentPlanetIndex >= planets.Count)
        {
            Debug.Log("SolarSystemManager: índice de planeta fora do intervalo.");
            return;
        }

        PlanetCutscene p = planets[currentPlanetIndex];
        Debug.Log($"[SolarSystemManager] Show planet #{currentPlanetIndex}: {p.planetName}");

        // Ativa somente o planeta atual (se você usa essa abordagem)
        for (int i = 0; i < planets.Count; i++)
        {
            if (planets[i].planetObject != null)
                planets[i].planetObject.SetActive(i == currentPlanetIndex);
        }

        // Atualiza o UI com nome/descrição (método da sua UI)
        if (ui != null)
            ui.ShowPlanetUI(p);

        // Se há áudio e há um AudioSource configurado, toca e quando terminar chama ShowPlanetQuiz
        if (p.planetAudio != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = p.planetAudio;
            audioSource.Play();
            StartCoroutine(WaitForAudioThenShowQuiz(p, p.planetAudio.length));
        }
        else
        {
            // Se não há áudio, mostra o quiz imediatamente (útil para testes)
            Debug.Log("[SolarSystemManager] Sem áudio: liberando quiz imediatamente para teste.");
            if (ui != null)
                ui.ShowPlanetQuiz(p);
        }
    }

    private IEnumerator WaitForAudioThenShowQuiz(PlanetCutscene p, float length)
    {
        // espera o tempo do áudio (caso audioSource.isPlaying seja falso, ainda assim aguardamos o length)
        yield return new WaitForSeconds(length);
        if (ui != null)
        {
            ui.ShowPlanetQuiz(p);
        }
    }

    // Chamado pela UI quando terminar o fluxo do planeta e for para o próximo
    public void NextPlanet()
    {
        currentPlanetIndex++;
        if (currentPlanetIndex >= planets.Count)
        {
            Debug.Log("[SolarSystemManager] Todos os planetas concluídos. Você pode implementar o modo panorâmico aqui.");
            // Aqui você pode implementar comportamento de final (panoramic mode etc.)
            return;
        }

        ShowCurrentPlanet();
    }

    // UTILIDADES
    public int GetCurrentPlanetIndex() => currentPlanetIndex;
    public PlanetCutscene GetCurrentPlanet() => (currentPlanetIndex >= 0 && currentPlanetIndex < planets.Count) ? planets[currentPlanetIndex] : null;
}
