using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateMovement(float moveAmount)
    {
        bool isWalking = moveAmount > 0.1f; // se mueve si el input es mayor a 0
        animator.SetBool("isWalking", isWalking);
    }
}
