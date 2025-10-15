using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    [SerializeField] private Button nextButton;

    void Start()
    {
        if (nextButton == null)
        {
            Debug.LogError("✗ NextButton não foi atribuído no Inspector!");
            return;
        }

        if (!nextButton.TryGetComponent<Button>(out Button btn))
        {
            Debug.LogError("✗ NextButton não tem componente Button!");
            return;
        }

        nextButton.onClick.AddListener(() => OnButtonClicked());
        Debug.Log("✓ Botão configurado com sucesso!");
    }

    void OnButtonClicked()
    {
        Debug.Log("✓✓✓ BOTÃO FOI CLICADO! ✓✓✓");
    }
}