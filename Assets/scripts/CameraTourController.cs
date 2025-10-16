using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraTourController : MonoBehaviour
{
    [Header("Configuração das Câmeras")]
    public CinemachineCamera initialCamera; // Câmera da posição inicial
    public CinemachineCamera[] tourCameras; // Câmeras do tour
    public float switchTime = 5f;
    public float pauseTime = 4f;
    public bool returnToInitialAfterTour = true; // Volta ao início após completar
    
    private int currentIndex = 0;
    private bool tourActive = false;
    private Coroutine tourCoroutine;

    void Start()
    {
        // IMPORTANTE: O tour NÃO começa automaticamente
        tourActive = false;
        
        // Força todas as câmeras do tour para Priority 0
        for (int i = 0; i < tourCameras.Length; i++)
        {
            if (tourCameras[i] != null)
                tourCameras[i].Priority = 0;
        }
        
        // Garante que começa SEMPRE na câmera inicial
        if (initialCamera != null)
        {
            initialCamera.Priority = 10;
            Debug.Log("Câmera inicial ativada - Priority 10");
        }
        
        Debug.Log("Tour Controller iniciado - Tour NÃO está ativo");
    }

    public void StartTour()
    {
        if (tourCameras.Length == 0)
        {
            Debug.LogWarning("Nenhuma câmera configurada no array de tour!");
            return;
        }

        if (tourActive)
        {
            // Se já está ativo, para o tour
            StopTour();
        }
        else
        {
            // Inicia o tour
            tourActive = true;
            currentIndex = 0;
            tourCoroutine = StartCoroutine(TourCoroutine());
            Debug.Log("Tour iniciado!");
        }
    }

    public void StopTour()
    {
        if (tourCoroutine != null)
        {
            StopCoroutine(tourCoroutine);
            tourCoroutine = null;
        }
        tourActive = false;
        
        // Volta para a câmera inicial
        SetInitialCamera();
            
        Debug.Log("Tour parado - voltou para posição inicial!");
    }

    IEnumerator TourCoroutine()
    {
        while (tourActive)
        {
            ActivateTourCamera(currentIndex);
            yield return new WaitForSeconds(switchTime);
            yield return new WaitForSeconds(pauseTime);
            
            currentIndex++;
            
            // Após completar um ciclo, volta ao início
            if (currentIndex >= tourCameras.Length)
            {
                if (returnToInitialAfterTour)
                {
                    tourActive = false;
                    SetInitialCamera();
                    Debug.Log("Tour completo - voltando à posição inicial!");
                }
                else
                {
                    currentIndex = 0; // Recomeça o tour
                }
            }
        }
    }

    void ActivateTourCamera(int index)
    {
        // Desativa a câmera inicial
        if (initialCamera != null)
            initialCamera.Priority = 0;
            
        // Desativa todas as câmeras do tour
        for (int i = 0; i < tourCameras.Length; i++)
        {
            tourCameras[i].Priority = 0;
        }
        
        // Ativa apenas a câmera atual
        tourCameras[index].Priority = 10;
        
        Debug.Log($"Câmera do tour {index} ativada");
    }
    
    void SetInitialCamera()
    {
        if (initialCamera == null) return;
        
        // Desativa todas as câmeras do tour
        for (int i = 0; i < tourCameras.Length; i++)
        {
            tourCameras[i].Priority = 0;
        }
        
        // Ativa apenas a câmera inicial
        initialCamera.Priority = 10;
        
        Debug.Log("Voltou para a câmera inicial");
    }
}