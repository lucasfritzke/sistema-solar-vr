using UnityEngine;
using UnityEngine.UI;

public class GazeSelectorUI : MonoBehaviour
{
    public float gazeTime = 3f;
    private float gazeTimer;
    private GameObject gazedObject;

    public Image reticleFill;  // círculo de carregamento
    public Image reticlePoint; // ponto fixo no centro

    void Start()
    {
        if (reticleFill != null)
            reticleFill.fillAmount = 0;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Só ativa o reticle de carregamento se o objeto for interativo
            if (hitObject.CompareTag("Selectable"))
            {
                if (gazedObject != hitObject)
                {
                    if (gazedObject != null)
                        gazedObject.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);

                    gazedObject = hitObject;
                    gazeTimer = 0;
                    gazedObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    gazeTimer += Time.deltaTime;
                    gazedObject.SendMessage("OnGazeStay", SendMessageOptions.DontRequireReceiver);

                    if (reticleFill != null)
                        reticleFill.fillAmount = gazeTimer / gazeTime;

                    if (gazeTimer >= gazeTime)
                    {
                        gazedObject.SendMessage("OnGazeSelect", SendMessageOptions.DontRequireReceiver);
                        gazeTimer = 0;
                        if (reticleFill != null)
                            reticleFill.fillAmount = 0;
                    }
                }

                // Mostra o círculo e o ponto
                if (reticleFill != null) reticleFill.enabled = true;
                if (reticlePoint != null) reticlePoint.enabled = true;
            }
            else
            {
                ResetReticle();
            }
        }
        else
        {
            ResetReticle();
        }
    }

    void ResetReticle()
    {
        if (gazedObject != null)
        {
            gazedObject.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
            gazedObject = null;
        }

        gazeTimer = 0;

        if (reticleFill != null)
        {
            reticleFill.fillAmount = 0;
            reticleFill.enabled = false; // esconde o círculo quando não está olhando para nada interativo
        }

        if (reticlePoint != null)
        {
            reticlePoint.enabled = true; // o ponto sempre visível
        }
    }
}
