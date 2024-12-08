using System.Collections;
using UnityEngine;

public class MapCapturer : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] int Height = 1024;
    [SerializeField] int Width = 1024;
    [SerializeField] int Depth = 24;
    private float DayTime = 0;
    private bool Captured = false;

    [SerializeField] private Material Material;
    private Texture Texture = null;

    private void Start()
    {
        StartCoroutine(CaptureScreen());

        City._Time.DayChanged += DayPassed;
    }

    public void DayPassed()
    {
        if (!Captured)
        {
            StartCoroutine(CaptureScreen());
        }

        DayTime = Random.value;
        Captured = false;
    }

    private void Update()
    {
        if(City._Time._DayProgress >= DayTime && !Captured)
        {
            StartCoroutine(CaptureScreen());
            Captured = true;
        }
    }

    private IEnumerator CaptureScreen()
    {
        Camera.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(Width, Height, Depth);
        Rect rect = new Rect(0, 0, Width, Height);
        Texture2D texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);

        Camera.targetTexture = renderTexture;
        Camera.Render();

        RenderTexture currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
        texture.Apply();

        Camera.targetTexture = null;
        RenderTexture.active = currentRenderTexture;
        Destroy(renderTexture);

        Texture = texture;
        Material.SetTexture("_BaseMap", Texture);

        Camera.gameObject.SetActive(false);
    }
}
