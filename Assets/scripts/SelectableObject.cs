using UnityEngine;
using UnityEngine.UI;

public class SelectableObject : MonoBehaviour
{
    private Renderer objectRenderer;
    private Image uiImage;
    private Button uiButton;
    private Color originalColor;

    void Start()
    {
        // Detecta se é objeto 3D ou UI
        objectRenderer = GetComponent<Renderer>();
        uiImage = GetComponent<Image>();
        uiButton = GetComponent<Button>();

        // Salva cor original
        if (objectRenderer != null)
            originalColor = objectRenderer.material.color;
        else if (uiImage != null)
            originalColor = uiImage.color;
        
        Debug.Log($"SelectableObject em {gameObject.name} inicializado");
    }

    public void OnGazeEnter()
    {
        Debug.Log($"→ Gaze ENTER: {gameObject.name}");

        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.yellow;
        }
        else if (uiImage != null)
        {
            uiImage.color = new Color(1f, 1f, 0f, uiImage.color.a); // Amarelo mantendo alpha
        }
    }

    public void OnGazeStay()
    {
        // Chamado enquanto olha (opcional usar)
    }

    public void OnGazeExit()
    {
        Debug.Log($"← Gaze EXIT: {gameObject.name}");

        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
        else if (uiImage != null)
        {
            uiImage.color = originalColor;
        }
    }

    public void OnGazeSelect()
    {
        Debug.Log($"✓ Gaze SELECT: {gameObject.name}");

        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.green;
        }
        else if (uiImage != null)
        {
            uiImage.color = new Color(0f, 1f, 0f, uiImage.color.a); // Verde mantendo alpha
        }

        // Se for botão UI, clica
        if (uiButton != null)
        {
            uiButton.onClick.Invoke();
        }
    }
}