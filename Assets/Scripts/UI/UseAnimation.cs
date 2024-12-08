using UnityEngine;

public class UseAnimation : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    private void OnEnable()
    {
        Animator.Play("Fade");
    }
}
