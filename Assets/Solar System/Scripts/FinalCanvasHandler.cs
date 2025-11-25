using UnityEngine;
using System.Collections.Generic;

public class FinalCanvasHandler : MonoBehaviour
{
    [Header("Opções")]
    public bool findOrbitsAutomatically = true; // Busca todos os Orbit na cena

    [Header("Planetas com Orbit (opcional)")]
    [Tooltip("Se 'findOrbitsAutomatically' estiver desmarcado, liste os planetas aqui")]
    public List<GameObject> planetsWithOrbit = new List<GameObject>();

    private List<MonoBehaviour> orbitScripts = new List<MonoBehaviour>();

    // Método público chamado pelo FinalScreenTrigger
    public void ActivateCanvasAndOrbits()
    {
        Debug.Log("🎬 Ativando canvas e orbits!");

        // Busca os orbits
        if (findOrbitsAutomatically)
        {
            FindAllOrbitScripts();
        }
        else if (planetsWithOrbit.Count > 0)
        {
            foreach (GameObject planet in planetsWithOrbit)
            {
                if (planet != null)
                {
                    MonoBehaviour orbit = planet.GetComponent("Orbit") as MonoBehaviour;
                    if (orbit != null)
                    {
                        orbitScripts.Add(orbit);
                    }
                }
            }
        }

        Debug.Log($"🪐 {orbitScripts.Count} scripts Orbit encontrados");

        // Ativa todos os orbits
        ActivateAllOrbits();
    }

    void FindAllOrbitScripts()
    {
        orbitScripts.Clear();

        // Busca todos os MonoBehaviour chamados "Orbit" na cena (incluindo desativados)
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>(true);

        foreach (MonoBehaviour script in allScripts)
        {
            if (script.GetType().Name == "Orbit")
            {
                orbitScripts.Add(script);
                Debug.Log($"   └─ Orbit encontrado em: {script.gameObject.name}");
            }
        }
    }

    void ActivateAllOrbits()
    {
        Debug.Log("🌍 Ativando rotação de todos os planetas!");

        foreach (MonoBehaviour orbit in orbitScripts)
        {
            if (orbit != null)
            {
                orbit.enabled = true;
                Debug.Log($"   ✅ Orbit ativado em: {orbit.gameObject.name}");
            }
        }
    }
}