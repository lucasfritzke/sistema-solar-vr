using UnityEngine;

/// <summary>
/// Controla a visibilidade do label do planeta.
/// Coloque este script em cada CubeSol, CubeMercurio, CubeVenus, etc.
/// </summary>
public class PlanetLabelController : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o GameObject do Label aqui (ex: PlanetLabel)")]
    public GameObject planetLabel;

    [Header("Configurações")]
    public bool showLabelOnArrival = true; // Mostra quando chega
    public bool hideLabelOnLeave = true; // Esconde quando sai

    void Start()
    {
        // Começa com o label escondido
        if (planetLabel != null)
        {
            planetLabel.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Planet Label não configurado em {gameObject.name}");
        }
    }

    // Chamado pelo XRRigMover quando a câmera chega
    public void OnCameraArrived()
    {
        if (showLabelOnArrival && planetLabel != null)
        {
            planetLabel.SetActive(true);
            Debug.Log($"Label mostrado: {planetLabel.name}");
        }
    }

    // Método público para esconder o label (será chamado quando sair)
    public void HideLabel()
    {
        if (hideLabelOnLeave && planetLabel != null)
        {
            planetLabel.SetActive(false);
            Debug.Log($"Label escondido: {planetLabel.name}");
        }
    }
}