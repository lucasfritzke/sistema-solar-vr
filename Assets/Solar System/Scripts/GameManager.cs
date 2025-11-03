using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class PlanetCutscene
    {
        public Transform targetPosition;                 // Posição do planeta
        public Vector3 cameraOffset = new Vector3(0, 3, -8); // Distância da câmera
        public float moveDuration = 2.5f;                // Tempo de transição
    }

    [Header("Configuração dos Planetas")]
    public List<PlanetCutscene> planets = new List<PlanetCutscene>();

    [Header("Referência da Câmera")]
    public Transform cameraRig; // <-- arrasta aqui o objeto que contém a MainCamera (ou a própria câmera)

    private int currentPlanetIndex = 0;
    private bool isMoving = false;

    void Start()
    {
        if (planets.Count == 0)
        {
            Debug.LogWarning("⚠️ Nenhum planeta configurado no GameManager!");
            return;
        }

        if (cameraRig == null)
        {
            // Se não foi arrastado manualmente, tenta encontrar automaticamente
            if (Camera.main != null)
                cameraRig = Camera.main.transform;
            else
            {
                Debug.LogError("❌ Nenhuma câmera encontrada na cena!");
                return;
            }
        }

        // Começa mostrando o primeiro planeta
        StartCoroutine(MoveToPlanet(0));
    }

    public void NextPlanet()
    {
        if (isMoving) return;

        currentPlanetIndex++;

        if (currentPlanetIndex < planets.Count)
        {
            Debug.Log("➡️ Indo para o planeta: " + currentPlanetIndex);
            StartCoroutine(MoveToPlanet(currentPlanetIndex));
        }
        else
        {
            Debug.Log("🌟 Todos os planetas foram visitados!");
        }
    }

    IEnumerator MoveToPlanet(int index)
    {
        isMoving = true;

        PlanetCutscene planet = planets[index];
        Vector3 startPos = cameraRig.position;
        Quaternion startRot = cameraRig.rotation;

        Vector3 targetPos = planet.targetPosition.position + planet.cameraOffset;
        Quaternion targetRot = Quaternion.LookRotation(planet.targetPosition.position - targetPos);

        float elapsed = 0f;
        while (elapsed < planet.moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOutCubic(elapsed / planet.moveDuration);

            cameraRig.position = Vector3.Lerp(startPos, targetPos, t);
            cameraRig.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        cameraRig.position = targetPos;
        cameraRig.rotation = targetRot;

        isMoving = false;
    }

    float EaseInOutCubic(float t)
    {
        return t < 0.5f
            ? 4f * t * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
