using UnityEngine;

public class CanvasFollower : MonoBehaviour
{
    public Transform planet;
    public Transform playerCamera;
    public Vector3 offset = new Vector3(0, 2, 0);

    void Update()
    {
        if (planet == null || playerCamera == null) return;

        // fica sempre perto do planeta
        transform.position = planet.position + offset;

        // olha sempre para o jogador
        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0); // inverte se estiver de costas
    }
}
