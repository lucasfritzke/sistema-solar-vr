using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SolarSystemManager : MonoBehaviour
{
    [System.Serializable]
    public class PlanetCutscene
    {
        public string planetName;
        public Transform targetPosition;
        public Vector3 cameraOffset = new Vector3(0, 5, -10);
        public float moveDuration = 3f;
        public string description;
    }

    [Header("Configurações")]
    public List<PlanetCutscene> planets = new List<PlanetCutscene>();
    public Vector3 panoramicViewPosition = new Vector3(0, 100, -200);
    public Vector3 panoramicLookAtPosition = Vector3.zero;
    public float panoramicRotationSpeed = 10f;
    
    private int currentPlanetIndex = 0;
    private bool isMoving = false;
    private bool isPanoramicMode = false;
    private Transform cameraRig;

    private Transform mainCamera;

    void Start()
{
    // Tenta encontrar a câmera principal mesmo em XR
    Camera cam = Camera.main;
    if (cam == null)
    {
        cam = FindObjectOfType<Camera>();
        Debug.LogWarning("⚠️ Nenhuma Camera.main encontrada — usando primeira câmera disponível.");
    }

    mainCamera = cam != null ? cam.transform : null;

    if (mainCamera == null)
    {
        Debug.LogError("❌ Nenhuma câmera foi encontrada! A movimentação não funcionará.");
        return;
    }

    DisableAllPlanetMovements();

    if (planets.Count > 0)
        StartCoroutine(PlayCutscene(0));
}


    void Update()
    {
        // Rotaciona a câmera no modo panorâmico
        if (isPanoramicMode)
        {
            mainCamera.RotateAround(panoramicLookAtPosition, Vector3.up, panoramicRotationSpeed * Time.deltaTime);
            mainCamera.LookAt(panoramicLookAtPosition);
        }
    }

    public void NextPlanet()
    {
        if (isMoving) return;

        currentPlanetIndex++;

        if (currentPlanetIndex < planets.Count)
        {
            StartCoroutine(PlayCutscene(currentPlanetIndex));
        }
        else
        {
            // Chegou ao final - vai para vista panorâmica
            StartCoroutine(GoToPanoramicView());
        }
    }

    IEnumerator PlayCutscene(int planetIndex)
    {
        isMoving = true;
        isPanoramicMode = false;
        
        PlanetCutscene planet = planets[planetIndex];

        if (planet.targetPosition != null)
        {
            Vector3 targetPos = planet.targetPosition.position + planet.cameraOffset;
            yield return StartCoroutine(MoveCameraToPosition(targetPos, planet.targetPosition.position, planet.moveDuration));
        }

        isMoving = false;
        DisplayPlanetInfo(planetIndex);
    }

    IEnumerator GoToPanoramicView()
    {
        isMoving = true;
        isPanoramicMode = false;

        Debug.Log("Vista Panorâmica! Apreciando todo o sistema solar...");
        
        yield return StartCoroutine(MoveCameraToPosition(panoramicViewPosition, panoramicLookAtPosition, 4f));

        isMoving = false;
        isPanoramicMode = true; // Ativa a rotação automática
    }

    IEnumerator MoveCameraToPosition(Vector3 targetPos, Vector3 lookAtPos, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = mainCamera.position;
        Quaternion startRot = mainCamera.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = EaseInOutCubic(elapsedTime / duration);

            mainCamera.position = Vector3.Lerp(startPos, targetPos, t);
            
            // Faz a câmera olhar suavemente para o alvo
            Quaternion targetRot = Quaternion.LookRotation(lookAtPos - mainCamera.position);
            mainCamera.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        mainCamera.position = targetPos;
        mainCamera.LookAt(lookAtPos);
    }

    void DisplayPlanetInfo(int planetIndex)
    {
        PlanetCutscene planet = planets[planetIndex];
        Debug.Log($"Planeta {planetIndex + 1}/{planets.Count}: {planet.planetName}\n{planet.description}");
    }

    void DisableAllPlanetMovements()
    {
        // Desativa TODOS os scripts Rotate e Orbit na cena
        Rotate[] allRotateScripts = FindObjectsOfType<Rotate>();
        foreach (Rotate rotate in allRotateScripts)
        {
            rotate.enabled = false;
        }

        Orbit[] allOrbitScripts = FindObjectsOfType<Orbit>();
        foreach (Orbit orbit in allOrbitScripts)
        {
            orbit.enabled = false;
        }

        Debug.Log($"Desativados {allRotateScripts.Length} scripts Rotate e {allOrbitScripts.Length} scripts Orbit");
    }

    public int GetCurrentPlanetIndex()
    {
        return currentPlanetIndex;
    }

    public bool IsInPanoramicMode()
    {
        return isPanoramicMode;
    }

    float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}