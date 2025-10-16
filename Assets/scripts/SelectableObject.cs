using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public void OnGazeEnter()
    {
        // Chamado quando o olhar começa a focar neste objeto
        Debug.Log($"Olhar entrou em {gameObject.name}");
        GetComponent<Renderer>().material.color = Color.yellow; // exemplo visual
    }

    public void OnGazeStay()
    {
        // Chamado enquanto o olhar continua
    }

    public void OnGazeExit()
    {
        // Chamado quando o olhar sai do objeto
        Debug.Log($"Olhar saiu de {gameObject.name}");
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void OnGazeSelect()
    {
        // Chamado quando o tempo de foco for atingido (ex: 3 segundos)
        Debug.Log($"Selecionado: {gameObject.name}");
        GetComponent<Renderer>().material.color = Color.green;
    }
}
