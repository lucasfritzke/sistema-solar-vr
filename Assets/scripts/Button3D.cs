using UnityEngine;

public class Button3D : MonoBehaviour
{
    public Transform heart;          // arraste o coração aqui no Inspector
    public Transform targetPosition; // posição final (um Empty colocado na frente da câmera)
    public float speed = 2f;

    private bool moveToTarget = false;
    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        if (heart != null)
        {
            originalPos = heart.position;
            originalRot = heart.rotation;
        }
    }

    void OnMouseDown() // dispara quando clica no objeto com collider
    {
        moveToTarget = !moveToTarget; // alterna entre ir e voltar
    }

    void Update()
    {
        if (heart == null || targetPosition == null) return;

        if (moveToTarget)
        {
            // Move para o target
            heart.position = Vector3.Lerp(
                heart.position,
                targetPosition.position,
                Time.deltaTime * speed
            );

            heart.rotation = Quaternion.Lerp(
                heart.rotation,
                targetPosition.rotation,
                Time.deltaTime * speed
            );
        }
        else
        {
            // Volta para o original
            heart.position = Vector3.Lerp(
                heart.position,
                originalPos,
                Time.deltaTime * speed
            );

            heart.rotation = Quaternion.Lerp(
                heart.rotation,
                originalRot,
                Time.deltaTime * speed
            );
        }
    }
}
