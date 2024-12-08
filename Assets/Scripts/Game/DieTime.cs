using UnityEngine;

public class DieTime : MonoBehaviour
{
    [SerializeField] private float LifeTime;
    [SerializeField] private bool UnscaledTime;

    private void Update()
    {
        if (UnscaledTime)
        {
            LifeTime -= Time.unscaledDeltaTime;
        }
        else
        {
            LifeTime -= Time.deltaTime;
        }

        if(LifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
