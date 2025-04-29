using UnityEngine;

public class BattleSfx : MonoBehaviour
{
    [SerializeField] protected MeshRenderer[] Meshes;
    [SerializeField] protected Color Color;
    [SerializeField] protected float FallofSpeed;
    protected Material[] Materials;

    protected float Value = 1;

    protected virtual void Start()
    {
        Materials = new Material[Meshes.Length];

       for(int i = 0; i < Meshes.Length; i++)
        {
            Materials[i] = new Material( Meshes[i].material);

            Meshes[i].material = Materials[i];
        }

    }

    protected virtual void Update()
    {
        Value -= Time.unscaledDeltaTime;

        foreach(Material material in Materials)
        {
            material.color = new Color(Color.r, Color.g, Color.b, Value);

            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", Color * 25* Value);

            DynamicGI.UpdateEnvironment();
        }
    }
}
