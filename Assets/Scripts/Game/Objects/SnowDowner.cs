using UnityEngine;

public class SnowDowner : MonoBehaviour
{
    private static SnowDowner Instance;

    [SerializeField] private CustomRenderTexture CustomRenderTexture;
    [SerializeField] private Material HeightMapUpdate;
    [SerializeField] private Material SnowMaterial;
    [SerializeField] private MeshFilter SnowMesh;
    [SerializeField] private MeshRenderer SnowMeshRenderer;
    [SerializeField] private Mesh[] SnowMeshes;
    [SerializeField] private Settings Settings;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private DownAsk[] DownQueue = new DownAsk[10];
    private bool HasQueue = false;
    private float Sizes = 0;

    private int LastResolution = -1;

    private void Awake()
    {
        Instance = this;

        switch (SaveManager._Instance._SaveData.MapSize)
        {
            case 0:
                Sizes = 150;
                break;
            case 1:
                Sizes = 175;
                break;
            case 2:
                Sizes = 200;
                break;
        }
        Sizes += 50;
    }

    private void Start()
    {
        Settings.OnChanges += UpdateSnowResolution;
        UpdateSnowResolution();
    }

    public void UpdateSnowResolution()
    {
        if(Settings._Data.SnowResolution != LastResolution)
        {
            LastResolution = Settings._Data.SnowResolution;
         //   SnowMesh.sharedMesh = SnowMeshes[Settings._Data.SnowResolution];

            CustomRenderTexture.Release();
            SnowMeshRenderer.shadowCastingMode = Settings._Data.SnowResolution == 2 ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
            SnowMaterial.SetFloat("_UVDelta", 0.001f * Mathf.Pow(0.05f, Settings._Data.SnowResolution));
            CustomRenderTexture.width = 1024 * (int)Mathf.Pow(2, Settings._Data.SnowResolution);
            CustomRenderTexture.height = 1024 * (int)Mathf.Pow(2, Settings._Data.SnowResolution);
            CustomRenderTexture.Initialize();
        }
    }

    private void Update()
    {
        if (!HasQueue)
        {
            return;
        }

        bool has = false;
        for(int i = 0; i < 10; i++)
        {
            if (DownQueue[i] != null)
            {
                if (Physics.Raycast(new Vector3(DownQueue[i].x, 100, DownQueue[i].z), Vector3.down, out RaycastHit hit, 300, LayerMask))
                {
                    HeightMapUpdate.SetTexture("_DrawBrush", DownQueue[i].Mask);
                    HeightMapUpdate.SetFloat("_Mastab", DownQueue[i].mastab);
                    HeightMapUpdate.SetVector("_DrawPosition", new Vector4(0.5f - hit.point.x / Sizes, 0.5f - hit.point.z / Sizes, 0, 0));
                    HeightMapUpdate.SetFloat("_DrawAngle", DownQueue[i].rotation * Mathf.Deg2Rad);
                }

                DownQueue[i] = null;
                has = true;
                break;
            }
        }

        if (!has)
        {
            HasQueue = false;
            HeightMapUpdate.SetVector("_DrawPosition", new Vector4(-1, -1, 0 ,0));
        }
    }

    public static bool AddDown(DownAsk ask)
    {
        Instance.HasQueue = true;
        for (int i =0; i < 10; i++)
        {
            if (Instance.DownQueue[i] == null)
            {
                Instance.DownQueue[i] = ask;
                return true;
            }
        }

        return false;
    }

    [System.Serializable]
    public class DownAsk
    {
        public Texture Mask;
        public float x;
        public float z;
        public float rotation;
        public float mastab;

        public DownAsk()
        {

        }

        public DownAsk(Texture mask, float x, float z, float rot, float mastab = 200)
        {
            this.Mask = mask;
            this.x = x;
            this.z = z;
            this.rotation = rot;
            this.mastab = mastab;
        }
    }
}
