using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XRRigMover : MonoBehaviour
{
    [System.Serializable]
    public class CutTarget
    {
        [Tooltip("Transform usado como referência (pode estar dentro do prefab do planeta).")]
        public Transform targetReference;

        [Tooltip("Offset local aplicado ao targetReference para posicionar a câmera (em espaço local do targetReference).")]
        public Vector3 cameraLocalOffset = new Vector3(0f, 0.7f, -2f);

        [Tooltip("Tempo da transição")]
        public float moveDuration = 2.0f;
    }

    [Header("Targets (ordem dos planetas)")]
    public List<CutTarget> targets = new List<CutTarget>();

    [Header("Referências do Rig / Câmera")]
    [Tooltip("Arraste aqui o root do seu XR Origin (o objeto pai do transform da câmera).")]
    public Transform cameraRig;
    [Tooltip("Se vazio, será obtido por Camera.main.transform")]
    public Transform mainCameraTransform;

    [Header("Config")]
    public float arriveThreshold = 0.05f;

    private int currentIndex = -1;
    private bool isMoving = false;

    void Start()
    {
        if (mainCameraTransform == null)
        {
            if (Camera.main != null)
                mainCameraTransform = Camera.main.transform;
        }

        if (cameraRig == null)
        {
            Debug.LogWarning("[XRRigMover] cameraRig não atribuída. Arraste o XR Origin root no inspector.");
        }

        if (mainCameraTransform == null)
        {
            Debug.LogError("[XRRigMover] Não foi possível encontrar Camera.main. Configure mainCameraTransform manualmente.");
        }

        // opcional: começa no índice 0
        if (targets.Count > 0)
            StartCoroutine(MoveToIndexImmediate(0));
    }

    public void NextPlanet()
    {
        if (isMoving) return;
        int next = currentIndex + 1;
        if (next >= targets.Count)
        {
            Debug.Log("[XRRigMover] Último planeta alcançado.");
            return;
        }
        StartCoroutine(MoveToIndex(next));
    }

    public void MoveToPlanetByIndex(int index)
    {
        if (isMoving) return;
        if (index < 0 || index >= targets.Count) return;
        StartCoroutine(MoveToIndex(index));
    }

    IEnumerator MoveToIndexImmediate(int index)
    {
        yield return null; // frame
        currentIndex = index;
        CutTarget ct = targets[index];

        Vector3 desiredCamWorldPos;
        if (ct.targetReference != null)
        {
            desiredCamWorldPos = ct.targetReference.position
                + ct.targetReference.right * ct.cameraLocalOffset.x
                + ct.targetReference.up * ct.cameraLocalOffset.y
                + ct.targetReference.forward * ct.cameraLocalOffset.z;
        }
        else
        {
            desiredCamWorldPos = ct.cameraLocalOffset;
        }

        Vector3 camWorldOffset = mainCameraTransform.position - cameraRig.position;
        Vector3 desiredRigPos = desiredCamWorldPos - camWorldOffset;

        Vector3 planetWorldPos = ct.targetReference != null ? ct.targetReference.position : desiredCamWorldPos + cameraRig.forward * 5f;

        // 🔍 DEBUG
        Debug.Log($"[MoveToIndexImmediate] ---- PLANETA #{index} ----");
        Debug.Log($"Target Reference: {(ct.targetReference ? ct.targetReference.name : "NULO")}");
        Debug.Log($"PlanetWorldPos: {planetWorldPos}");
        Debug.Log($"DesiredCamWorldPos: {desiredCamWorldPos}");
        Debug.Log($"DesiredRigPos: {desiredRigPos}");
        Debug.Log($"CameraRig posição antes: {cameraRig.position}");

        cameraRig.position = desiredRigPos;

        mainCameraTransform.LookAt(planetWorldPos);
        cameraRig.rotation = Quaternion.LookRotation((planetWorldPos - mainCameraTransform.position).normalized, Vector3.up);

        Debug.Log($"CameraRig posição final: {cameraRig.position}");
        Debug.Log("--------------------------------------------");

        // Notifica que a câmera chegou (funciona com qualquer script)
        if (ct.targetReference != null)
        {
            // Tenta chamar OnCameraArrived em qualquer script do target
            ct.targetReference.SendMessage("OnCameraArrived", SendMessageOptions.DontRequireReceiver);

            // Também tenta nos filhos (para compatibilidade com PlanetQuiz)
            ct.targetReference.BroadcastMessage("OnCameraArrived", SendMessageOptions.DontRequireReceiver);
        }

        yield break;
    }

    IEnumerator MoveToIndex(int index)
    {
        isMoving = true;
        if (currentIndex >= 0 && currentIndex < targets.Count)
        {
            CutTarget previousTarget = targets[currentIndex];
            if (previousTarget.targetReference != null)
            {
                previousTarget.targetReference.SendMessage("HideLabel", SendMessageOptions.DontRequireReceiver);
            }
        }
        CutTarget ct = targets[index];

        // calcula posição desejada da câmera
        Vector3 desiredCamWorldPos;
        if (ct.targetReference != null)
        {
            desiredCamWorldPos = ct.targetReference.position
                + ct.targetReference.right * ct.cameraLocalOffset.x
                + ct.targetReference.up * ct.cameraLocalOffset.y
                + ct.targetReference.forward * ct.cameraLocalOffset.z;
        }
        else
        {
            desiredCamWorldPos = ct.cameraLocalOffset;
        }

        // offset do rig
        Vector3 camWorldOffset = mainCameraTransform.position - cameraRig.position;
        Vector3 desiredRigPos = desiredCamWorldPos - camWorldOffset;

        Vector3 planetWorldPos = ct.targetReference != null ? ct.targetReference.position : desiredCamWorldPos + cameraRig.forward * 5f;
        Quaternion targetRigRot = Quaternion.LookRotation((planetWorldPos - desiredCamWorldPos).normalized, Vector3.up);

        Vector3 startPos = cameraRig.position;
        Quaternion startRot = cameraRig.rotation;

        float duration = Mathf.Max(0.01f, ct.moveDuration);
        float elapsed = 0f;

        // 🔍 DEBUG INICIAL
        Debug.Log($"[MoveToIndex] ---- Iniciando transição para planeta #{index} ----");
        Debug.Log($"Target Reference: {(ct.targetReference ? ct.targetReference.name : "NULO")}");
        Debug.Log($"PlanetWorldPos: {planetWorldPos}");
        Debug.Log($"DesiredCamWorldPos: {desiredCamWorldPos}");
        Debug.Log($"DesiredRigPos: {desiredRigPos}");
        Debug.Log($"CameraRig posição inicial: {cameraRig.position}");
        Debug.Log("--------------------------------------------");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOutCubic(elapsed / duration);

            cameraRig.position = Vector3.Lerp(startPos, desiredRigPos, t);
            cameraRig.rotation = Quaternion.Slerp(startRot, targetRigRot, t);

            yield return null;
        }

        cameraRig.position = desiredRigPos;
        cameraRig.rotation = targetRigRot;
        mainCameraTransform.LookAt(planetWorldPos);

        currentIndex = index;
        isMoving = false;

        // 🔍 DEBUG FINAL
        Debug.Log($"[MoveToIndex] ---- Fim da transição planeta #{index} ----");
        Debug.Log($"CameraRig posição final: {cameraRig.position}");
        Debug.Log($"MainCamera posição final: {mainCameraTransform.position}");
        Debug.Log("--------------------------------------------");

        // Notifica que a câmera chegou perto do planeta para iniciar o áudio
        if (ct.targetReference != null)
        {
            // Tenta chamar OnCameraArrived em qualquer script do target
            ct.targetReference.SendMessage("OnCameraArrived", SendMessageOptions.DontRequireReceiver);

            // Também tenta nos filhos (para compatibilidade com PlanetQuiz)
            ct.targetReference.BroadcastMessage("OnCameraArrived", SendMessageOptions.DontRequireReceiver);
        }

        yield break;
    }

    float EaseInOutCubic(float t)
    {
        return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    public int GetIndexByReference(Transform reference)
    {
        if (reference == null) return -1;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].targetReference == reference || reference.IsChildOf(targets[i].targetReference))
                return i;
        }
        return -1;
    }
}
