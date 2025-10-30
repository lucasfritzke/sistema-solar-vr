using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioGroupController : MonoBehaviour
{
    public ToggleGroup toggleGroup;

    //public void Submit()
    //{
    //    Toggle toggle = toggleGroup.gameObject.GetComponent<Toggle>();
    //    Debug.Log("Clicked");
    //}

    public void SetQuestions(List<string> options)
    {
        Toggle[] toggles = toggleGroup.gameObject.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].GetComponentInChildren<Text>().text = options[i];
        }
    }
}
