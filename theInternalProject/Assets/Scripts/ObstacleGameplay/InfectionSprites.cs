using UnityEngine;

public class InfectionSprites : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer.GetComponent<SpriteRenderer>();
        }

        var patient = GameManager.instance?.currentPatient;
        if (patient == null) return;

        bool hasAnimator = patient.infectionAnimator != null;
        bool hasAnimState = !string.IsNullOrEmpty(patient.infectionAnimationStringName);

        if (hasAnimator && hasAnimState)
        {
            animator.runtimeAnimatorController = patient.infectionAnimator;
            animator.enabled = true;
            animator.Play(patient.infectionAnimationStringName, 0, 0f);
        }
        else
        {
            if (animator != null) animator.enabled = true;
            if (patient.infectionSprite)
                spriteRenderer.sprite = patient.infectionSprite;
        }

    }   
}

