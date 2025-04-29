using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapCapturer : MonoBehaviour, IInteractable
{
    [SerializeField] private Camera Camera;
    [SerializeField] int Height = 1024;
    [SerializeField] int Width = 1024;
    [SerializeField] int Depth = 24;
    private float DayTime = 0;
    private bool Captured = false;

    [SerializeField] private GameObject MapViewPrefab;
    [SerializeField] private Transform Canvas;
    [SerializeField] private Outline Indicator;
    [SerializeField] private Sprite ViewImage;
    [SerializeField] private Material Material;
    private Texture Texture = null;
    private MapView Instance = null;

    public bool _AbstractUse => true;

    public string _Info => "Просмотреть карту";

    public void Indicate(bool state) => Indicator.enabled = state;

    public void Interact()
    {
        if(Instance == null)
        {
            Instance = Instantiate(MapViewPrefab, Canvas).GetComponent<MapView>();
            Instance.SetInfo(ViewImage);
        }
        else
        {
            Destroy(Instance.gameObject);
        }
    }

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

        DayTime = Random.Range(0, 1500);
        Captured = false;
    }

    private void Update()
    {
        if(City._Time._WorldTime % 1500 >= DayTime && !Captured)
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

        ViewImage = Sprite.Create(texture, new Rect(0,0, Width, Height), Vector2.one / 2); 

        if(Instance != null)
        {
            Instance.SetInfo(ViewImage);
        }

        Camera.gameObject.SetActive(false);
    }
}
