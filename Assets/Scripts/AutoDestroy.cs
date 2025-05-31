using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifeTime = 2f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
