using UnityEngine;

public class SolarSystemRealistic : MonoBehaviour
{
    [System.Serializable]
    public class CelestialBody
    {
        public string name;
        public Transform bodyTransform;
        public float rotationSpeed;           // Graus por segundo
        public Vector3 rotationAxis = Vector3.up;
        public bool enableRotation = true;
    }

    [System.Serializable]
    public class Moon
    {
        public string name;
        public Transform moonTransform;
        public Transform planetTransform;     // Planeta ao redor do qual orbita
        public float orbitSpeed;              // Graus por segundo
        public float orbitRadius;             // Distância do planeta
        public float rotationSpeed;           // Rotação própria
        [HideInInspector] public float currentAngle;
    }

    [Header("Sol")]
    public CelestialBody sun;

    [Header("Planetas Rochosos (Apenas Rotação)")]
    public CelestialBody mercury;
    public CelestialBody venus;
    public CelestialBody earth;
    public CelestialBody mars;

    [Header("Gigantes Gasosos (Apenas Rotação)")]
    public CelestialBody jupiter;
    public CelestialBody saturn;
    public CelestialBody uranus;
    public CelestialBody neptune;

    [Header("Planetas Anões (Apenas Rotação)")]
    public CelestialBody pluto;

    [Header("Luas (Orbitam os Planetas)")]
    public Moon[] moons;

    [Header("Configurações Globais")]
    public float timeScale = 50f;
    public bool showOrbits = true;

    [Header("IMPORTANTE")]
    [Tooltip("Planetas NÃO orbitam o Sol - ficam fixos. Apenas rotacionam.")]
    public string note = "Planetas fixos, apenas luas orbitam";

    void Start()
    {
        // Inicializa as luas com ângulos aleatórios
        foreach (Moon moon in moons)
        {
            moon.currentAngle = Random.Range(0f, 360f);
            UpdateMoonPosition(moon);
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime * timeScale;

        // Rotação de todos os corpos celestes
        RotateCelestialBody(sun, deltaTime);
        RotateCelestialBody(mercury, deltaTime);
        RotateCelestialBody(venus, deltaTime);
        RotateCelestialBody(earth, deltaTime);
        RotateCelestialBody(mars, deltaTime);
        RotateCelestialBody(jupiter, deltaTime);
        RotateCelestialBody(saturn, deltaTime);
        RotateCelestialBody(uranus, deltaTime);
        RotateCelestialBody(neptune, deltaTime);
        RotateCelestialBody(pluto, deltaTime);

        // Atualiza todas as luas
        foreach (Moon moon in moons)
        {
            if (moon.moonTransform != null && moon.planetTransform != null)
            {
                // Translação: Orbita o planeta
                moon.currentAngle += moon.orbitSpeed * deltaTime;
                UpdateMoonPosition(moon);

                // Rotação própria
                moon.moonTransform.Rotate(Vector3.up, moon.rotationSpeed * deltaTime, Space.Self);
            }
        }
    }

    void RotateCelestialBody(CelestialBody body, float deltaTime)
    {
        if (body.bodyTransform != null && body.enableRotation)
        {
            body.bodyTransform.Rotate(body.rotationAxis, body.rotationSpeed * deltaTime, Space.Self);
        }
    }

    void UpdateMoonPosition(Moon moon)
    {
        float angleRad = moon.currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(angleRad) * moon.orbitRadius,
            0,
            Mathf.Sin(angleRad) * moon.orbitRadius
        );

        moon.moonTransform.position = moon.planetTransform.position + offset;
    }

    // Desenha órbitas das luas no editor
    void OnDrawGizmosSelected()
    {
        if (!showOrbits)
            return;

        Gizmos.color = Color.cyan;

        foreach (Moon moon in moons)
        {
            if (moon.planetTransform == null)
                continue;

            // Desenha órbita da lua
            int segments = 50;
            Vector3 previousPoint = moon.planetTransform.position + new Vector3(moon.orbitRadius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = (360f / segments) * i * Mathf.Deg2Rad;
                Vector3 newPoint = moon.planetTransform.position + new Vector3(
                    Mathf.Cos(angle) * moon.orbitRadius,
                    0,
                    Mathf.Sin(angle) * moon.orbitRadius
                );

                Gizmos.DrawLine(previousPoint, newPoint);
                previousPoint = newPoint;
            }
        }
    }
}