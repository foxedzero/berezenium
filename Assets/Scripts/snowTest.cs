using UnityEngine;

public class snowTest : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private CustomRenderTexture CustomRenderTexture;
    [SerializeField] private Material HeightMapUpdate;

    private void Start()
    {
        CustomRenderTexture.Initialize();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 100, new Color(1, 0, 0));
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                HeightMapUpdate.SetVector("_DrawPosition", hit.textureCoord);
                HeightMapUpdate.SetFloat("_DrawAngle", 45 * Mathf.Deg2Rad);
            }
        }
    }
}
