using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private Material Material;
    [SerializeField] private bool Shot;
    [SerializeField] private RenderTexture RenderTexture;
    [SerializeField] private SpriteRenderer Renderer;

    private void Start()
    {
       
    }

    private void Update()
    {
        if (Shot)
        {
            Shot = false;
            Renderer.sprite = CaptureScreen();
        }
    }

    [SerializeField] int height = 1024;
    [SerializeField] int width = 1024;
    [SerializeField] int depth = 24;

    //method to render from camera
    public Sprite CaptureScreen()
    {
        RenderTexture renderTexture = new RenderTexture(width, height, depth);
        Rect rect = new Rect(0, 0, width, height);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Camera.targetTexture = renderTexture;
        Camera.Render();

        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        Camera.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        Destroy(renderTexture);

        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

        return sprite;
    }
}
