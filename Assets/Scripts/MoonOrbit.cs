using UnityEngine;

public class MoonOrbit : MonoBehaviour
{
    [Header("Configurações da Órbita")]
    public Transform planetCenter;           // O planeta ao redor do qual a lua orbita
    public float orbitSpeed = 50f;           // Velocidade da órbita (graus por segundo)
    public float orbitRadius = 5f;           // Distância do planeta
    public Vector3 orbitAxis = Vector3.up;   // Eixo de rotação (padrão: Y = vertical)
    
    [Header("Rotação Própria")]
    public bool rotateOnSelf = true;         // A lua gira em torno de si mesma?
    public Vector3 selfRotationSpeed = new Vector3(0, 30f, 0);

    private Vector3 currentAngle;

    void Start()
    {
        // Se não foi atribuído um planeta, tenta pegar o pai
        if (planetCenter == null)
        {
            if (transform.parent != null)
                planetCenter = transform.parent;
            else
                Debug.LogError($"MoonOrbit em {gameObject.name}: Nenhum planeta atribuído!");
        }

        // Posiciona a lua na órbita inicial
        if (planetCenter != null)
        {
            currentAngle = Random.Range(0f, 360f) * Vector3.up; // Ângulo inicial aleatório
            UpdatePosition();
        }
    }

    void Update()
    {
        if (planetCenter == null)
            return;

        // Orbita ao redor do planeta
        currentAngle += orbitAxis.normalized * orbitSpeed * Time.deltaTime;
        UpdatePosition();

        // Rotação própria da lua
        if (rotateOnSelf)
        {
            transform.Rotate(selfRotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    void UpdatePosition()
    {
        // Calcula a posição da lua na órbita
        Quaternion rotation = Quaternion.AngleAxis(currentAngle.y, orbitAxis);
        Vector3 offset = rotation * (Vector3.forward * orbitRadius);
        transform.position = planetCenter.position + offset;
    }

    // Desenha a órbita no editor
    void OnDrawGizmosSelected()
    {
        if (planetCenter == null)
            return;

        Gizmos.color = Color.cyan;
        
        // Desenha um círculo representando a órbita
        int segments = 50;
        Vector3 previousPoint = planetCenter.position + (Vector3.forward * orbitRadius);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = (360f / segments) * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, orbitAxis);
            Vector3 newPoint = planetCenter.position + (rotation * (Vector3.forward * orbitRadius));
            
            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
}