using UnityEngine;

public interface IAnimator
{
    void SetTrigger(string name);
    void SetFloat(string name, float value);
    void SetBool(string name, bool value);
    void Play(string stateName);

}
