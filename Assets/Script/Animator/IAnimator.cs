using UnityEngine;

public interface IAnimator
{
    void SetTrigger(string name);
    void SetBool(string name, bool value);
    void Play(string stateName);

}
