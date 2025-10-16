using UnityEngine;

public class GazeTarget : MonoBehaviour
{
    public void OnGazeSelect()
    {
        Debug.Log("Selecionou: " + gameObject.name);
        GetComponent<Renderer>().material.color = Color.green; // exemplo visual
    }
}
