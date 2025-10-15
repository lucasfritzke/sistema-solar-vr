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
        public Vector3 lookAtOffset = Vector3.zero;
        public float moveDuration = 3f;
        public string description;
    }

    public List<PlanetCutscene> planets = new List<PlanetCutscene>();
    private int currentPlanetIndex = 0;
    private bool isMoving = false;
    
    private Transform mainCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;
        
        if (planets.Count > 0)
            StartCoroutine(PlayCutscene(0));
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
            Debug.Log("Fim da jornada pelo sistema solar!");
            currentPlanetIndex = 0;
            StartCoroutine(PlayCutscene(0));
        }
    }

    IEnumerator PlayCutscene(int planetIndex)
    {
        isMoving = true;
        PlanetCutscene planet = planets[planetIndex];

        // Reativa a rotação do planeta anterior (se houver)
        if (currentPlanetIndex > 0)
        {
            PlanetCutscene previousPlanet = planets[currentPlanetIndex - 1];
            RestartPlanetRotation(previousPlanet.targetPosition);
        }

        if (planet.targetPosition != null)
        {
            yield return StartCoroutine(MoveCameraToTarget(planet.targetPosition, planet.lookAtOffset, planet.moveDuration));
        }

        isMoving = false;
        DisplayPlanetInfo(planetIndex);
    }

    IEnumerator MoveCameraToTarget(Transform target, Vector3 lookAtOffset, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = mainCamera.position;
        Vector3 targetPos = target.position + lookAtOffset;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            mainCamera.position = Vector3.Lerp(startPos, targetPos, EaseInOutCubic(t));
            mainCamera.LookAt(target.position);

            yield return null;
        }

        mainCamera.position = targetPos;
        mainCamera.LookAt(target.position);
    }

    void DisplayPlanetInfo(int planetIndex)
    {
        PlanetCutscene planet = planets[planetIndex];
        
        // Para a rotação do planeta
        StopPlanetRotation(planet.targetPosition);
        
        Debug.Log($"Planeta: {planet.planetName}\n{planet.description}");
    }

    void StopPlanetRotation(Transform planetTransform)
    {
        if (planetTransform == null)
            return;

        // Desativa o script Rotate do planeta
        Rotate rotateScript = planetTransform.GetComponent<Rotate>();
        if (rotateScript != null)
        {
            rotateScript.enabled = false;
        }

        // Também desativa scripts de órbita dos filhos
        Orbit orbitScript = planetTransform.GetComponent<Orbit>();
        if (orbitScript != null)
        {
            orbitScript.enabled = false;
        }

        // Desativa rotação de filhos também
        foreach (Transform child in planetTransform)
        {
            Rotate childRotate = child.GetComponent<Rotate>();
            if (childRotate != null)
                childRotate.enabled = false;

            Orbit childOrbit = child.GetComponent<Orbit>();
            if (childOrbit != null)
                childOrbit.enabled = false;
        }
    }

    void RestartPlanetRotation(Transform planetTransform)
    {
        if (planetTransform == null)
            return;

        // Reativa o script Rotate do planeta
        Rotate rotateScript = planetTransform.GetComponent<Rotate>();
        if (rotateScript != null)
        {
            rotateScript.enabled = true;
        }

        // Também reativa scripts de órbita dos filhos
        Orbit orbitScript = planetTransform.GetComponent<Orbit>();
        if (orbitScript != null)
        {
            orbitScript.enabled = true;
        }

        // Reativa rotação de filhos também
        foreach (Transform child in planetTransform)
        {
            Rotate childRotate = child.GetComponent<Rotate>();
            if (childRotate != null)
                childRotate.enabled = true;

            Orbit childOrbit = child.GetComponent<Orbit>();
            if (childOrbit != null)
                childOrbit.enabled = true;
        }
    }

    public int GetCurrentPlanetIndex()
    {
        return currentPlanetIndex;
    }

    float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}