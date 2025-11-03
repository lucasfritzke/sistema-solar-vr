using UnityEngine;

public class GazeTarget : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && rend.material.HasProperty("_Color"))
            originalColor = rend.material.color;
    }

    public void OnGazeEnter()
    {
        if (rend != null && rend.material.HasProperty("_Color"))
            rend.material.color = Color.yellow;
    }


    public void OnGazeStay()
    {
        // Aqui você pode colocar animações ou feedback contínuo
    }

    public void OnGazeExit()
    {
        if (rend != null)
            rend.material.color = originalColor; // volta à cor normal
        Debug.Log($" Saiu do gaze: {name}");
    }

    public void OnGazeSelect()
    {
        if (rend != null)
            rend.material.color = Color.green; // confirma seleção
        Debug.Log($" Selecionou: {name}");
    }
}
