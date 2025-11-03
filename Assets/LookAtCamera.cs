using UnityEngine;
public class LookAtCamera : MonoBehaviour
{
    public Transform cameraTransform; // Referência à câmera

    void Update()
    {
        // Faz o objeto sempre olhar para a câmera
        transform.LookAt(cameraTransform.position);
    }
}
