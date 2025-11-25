using UnityEngine;

/// <summary>
/// Coloque este script em um GameObject ATIVO e vazio.
/// Ele será o targetReference no XRRigMover e ativará o Canvas Final quando a câmera chegar.
/// </summary>
public class FinalScreenTrigger : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o Canvas Final aqui (pode estar desativado)")]
    public GameObject finalCanvas;

    // Este método será chamado pelo XRRigMover quando chegar aqui
    public void OnCameraArrived()
    {
        Debug.Log("Câmera chegou no trigger da tela final!");

        if (finalCanvas != null)
        {
            // Ativa o canvas
            finalCanvas.SetActive(true);
            Debug.Log("Canvas Final ativado!");

            // Chama o método para ativar os orbits
            FinalCanvasHandler handler = finalCanvas.GetComponent<FinalCanvasHandler>();
            if (handler != null)
            {
                handler.ActivateCanvasAndOrbits();
            }
        }
        else
        {
            Debug.LogError(" Final Canvas não configurado no FinalScreenTrigger!");
        }
    }
}