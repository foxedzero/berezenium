using UnityEngine;

public class DangerScroller : MonoBehaviour
{
    [SerializeField] private Material Material;
    [SerializeField] private float Speed;

    private void Update()
    {
        Material.SetFloat("_UnscaledTime", Time.unscaledTime * Speed);
    }
}
