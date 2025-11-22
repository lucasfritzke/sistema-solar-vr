
using UnityEngine;
using UnityEngine.UI;

public class Iniciar: MonoBehaviour
{
    [SerializeField] GameObject sol;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject panel;
    [SerializeField] Button button;
    [SerializeField] GameObject painelRef;

    void Start()
    {
        button.onClick.AddListener(() => this.OnClick());
    }

    public void OnClick()
    {
        sol.SetActive(true);
        gameManager.SetActive(true);
        painelRef.SetActive(true);
        panel.SetActive(false);
    }
}