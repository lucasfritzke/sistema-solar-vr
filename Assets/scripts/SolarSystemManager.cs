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

    private Transform cameraRig;     // XR Origin
    private Transform mainCamera;    // Câmera real dentro do XR Origin

    void Start()
    {
        // Tenta encontrar a câmera principal
        mainCamera = Camera.main?.transform;
        
        // Se não encontrar, procura por XR Camera
        if (mainCamera == null)
        {
            GameObject xrOrigin = GameObject.Find("XR Origin");
            if (xrOrigin != null)
            {
                cameraRig = xrOrigin.transform;
                Transform xrCamera = xrOrigin.transform.Find("Main Camera");
                if (xrCamera != null)
                    mainCamera = xrCamera;
            }
        }
        else
        {
            // Se encontrou a Main Camera, pega o pai (XR Origin)
            if (mainCamera.parent != null)
                cameraRig = mainCamera.parent;
            else
                cameraRig = mainCamera;
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("❌ Câmera não encontrada!");
            return;
        }
        
        if (cameraRig == null)
        {
            Debug.LogWarning("⚠️ XR Origin não encontrado! Usando câmera.");
            cameraRig = mainCamera;
        }
        
        Debug.Log($"✓ Câmera: {mainCamera.name}");
        Debug.Log($"✓ Rig: {cameraRig.name}");
        
        DisableAllPlanetMovements();
        
        if (planets.Count > 0)
            StartCoroutine(PlayCutscene(0));
        else
            Debug.LogError("❌ Lista de planetas vazia!");
    }

    void Update()
    {
        if (isPanoramicMode && cameraRig != null)
        {
            cameraRig.RotateAround(panoramicLookAtPosition, Vector3.up, panoramicRotationSpeed * Time.deltaTime);
            cameraRig.LookAt(panoramicLookAtPosition);
        }
    }

    public void NextPlanet()
    {
        if (isMoving) return;

        currentPlanetIndex++;

        if (currentPlanetIndex < planets.Count)
            StartCoroutine(PlayCutscene(currentPlanetIndex));
        else
            StartCoroutine(GoToPanoramicView());
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

        Debug.Log("🌌 Vista panorâmica...");

        yield return StartCoroutine(MoveCameraToPosition(panoramicViewPosition, panoramicLookAtPosition, 4f));

        isMoving = false;
        isPanoramicMode = true;
    }

    IEnumerator MoveCameraToPosition(Vector3 targetPos, Vector3 lookAtPos, float duration)
    {
        if (cameraRig == null)
        {
            Debug.LogError("❌ Camera rig é null!");
            yield break;
        }

        float elapsedTime = 0f;
        Vector3 startPos = cameraRig.position;
        Quaternion startRot = cameraRig.rotation;

        Debug.Log($"📹 Movendo de {startPos} para {targetPos}");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = EaseInOutCubic(elapsedTime / duration);

            cameraRig.position = Vector3.Lerp(startPos, targetPos, t);

            Vector3 direction = lookAtPos - cameraRig.position;
            if (direction != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                cameraRig.rotation = Quaternion.Slerp(startRot, targetRot, t);
            }

            yield return null;
        }

        cameraRig.position = targetPos;
        Vector3 finalDirection = lookAtPos - cameraRig.position;
        if (finalDirection != Vector3.zero)
            cameraRig.rotation = Quaternion.LookRotation(finalDirection);

        Debug.Log($"✓ Câmera chegou em {targetPos}");
    }

    void DisplayPlanetInfo(int planetIndex)
    {
        PlanetCutscene planet = planets[planetIndex];
        Debug.Log($"🪐 {planet.planetName}: {planet.description}");
    }

    void DisableAllPlanetMovements()
    {
        Rotate[] allRotateScripts = FindObjectsOfType<Rotate>();
        foreach (Rotate rotate in allRotateScripts)
            rotate.enabled = false;

        Orbit[] allOrbitScripts = FindObjectsOfType<Orbit>();
        foreach (Orbit orbit in allOrbitScripts)
            orbit.enabled = false;

        Debug.Log($"🚫 Desativados {allRotateScripts.Length} Rotate e {allOrbitScripts.Length} Orbit");
    }

    public int GetCurrentPlanetIndex() => currentPlanetIndex;
    public bool IsInPanoramicMode() => isPanoramicMode;

    float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
