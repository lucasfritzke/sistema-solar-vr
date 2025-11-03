using UnityEngine;
using System.Collections;

/// <summary>
/// Script para movimento suave da câmera entre posições
/// Usado para transição entre planetas
/// </summary>
public class CameraSmoothMovement : MonoBehaviour
{
    public Transform targetPosition;
    public float transitionSpeed = 2f;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isMoving = false;

    public void MoveTo(Transform target, float speed = 2f)
    {
        if (isMoving) return;

        targetPosition = target;
        transitionSpeed = speed;
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 endPos = targetPosition.position;
        Quaternion endRot = targetPosition.rotation;

        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / transitionSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = movementCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, endPos, curveValue);
            transform.rotation = Quaternion.Lerp(startRot, endRot, curveValue);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;
        isMoving = false;
    }

    public bool IsMoving => isMoving;
}

