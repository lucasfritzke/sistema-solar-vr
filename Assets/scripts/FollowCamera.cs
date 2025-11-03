using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cameraTransform; // Referência à câmera
    public Vector3 offset; // O deslocamento que você quer manter entre o elemento e a câmera

    void Update()
    {
        // Atualiza a posição do elemento com base na posição da câmera + o offset
        transform.position = cameraTransform.position + offset;
    }
}
