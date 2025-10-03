using UnityEngine;

public class LifetimeDestroyer : MonoBehaviour
{

    public float Time;
    void Start()
    {
        Destroy(this.gameObject, Time);
    }


}
