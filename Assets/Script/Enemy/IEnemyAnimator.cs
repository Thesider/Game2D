using UnityEngine;

// Minimal animator abstraction for behaviour-tree nodes.
// Keeps nodes decoupled from Unity's Animator so we can swap implementations or mock in tests.
public interface IEnemyAnimator
{
    void SetTrigger(string name);
    void SetBool(string name, bool value);
    void Play(string stateName);
}
