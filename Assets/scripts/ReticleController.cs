using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    public Image reticleFill;
    public Image reticlePoint;
    public float fillSpeed = 1f; // tempo em segundos para encher

    private float fillAmount = 0f;
    private GameObject currentTarget;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Detecta o que o jogador está olhando
        if (Physics.Raycast(ray, out hit))
        {
            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("Selectable"))
            {
                // Se começou a olhar para um novo alvo
                if (target != currentTarget)
                {
                    currentTarget = target;
                    fillAmount = 0f;
                    reticleFill.fillAmount = 0f;
                    reticleFill.enabled = true;
                    reticlePoint.enabled = true;
                }

                // Enquanto estiver olhando, enche o círculo
                if (fillAmount < 1f)
                {
                    fillAmount += Time.deltaTime / fillSpeed;
                    reticleFill.fillAmount = fillAmount;
                }
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
        currentTarget = null;
        fillAmount = 0f;
        reticleFill.fillAmount = 0f;
        reticleFill.enabled = false;
        reticlePoint.enabled = false;
    }
}
