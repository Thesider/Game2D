using UnityEngine;


public class AnimatorAdapter : IAnimator
{
    private readonly Animator animator;

    public AnimatorAdapter(Animator animator)
    {
        this.animator = animator;
    }

    public void SetTrigger(string name)
    {
        if (animator != null) animator.SetTrigger(name);
    }

    public void SetBool(string name, bool value)
    {
        if (animator != null) animator.SetBool(name, value);
    }
    public void SetFloat(string name, float value) => animator?.SetFloat(name, value);


    public void Play(string stateName)
    {
        if (animator != null) animator.Play(stateName);
    }

}
