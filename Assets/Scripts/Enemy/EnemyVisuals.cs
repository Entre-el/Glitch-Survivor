using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    private EnemyCore core;
    private Animator animator;

    private void Awake()
    {
        TryGetComponent<EnemyCore>(out EnemyCore enemyCore);
        core = enemyCore;

        TryGetComponent<Animator>(out Animator anim);
        animator = anim;
    }

    public void PlayHitReaction()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
            Debug.Log($"Enemy {gameObject.name} plays hit reaction.");
        }
    }
}
