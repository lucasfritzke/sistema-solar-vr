
using TMPro;
using UnityEngine;

public class Question : MonoBehaviour
{
    TextMeshPro textMeshPro;

    public void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public void setQuestion(string text)
    {
        textMeshPro.text = text;
    }
}
