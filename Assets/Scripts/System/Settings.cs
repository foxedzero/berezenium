using UnityEngine;
using System.IO;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
//using UnityEngine.Localization.Settings;

public class Settings : MonoBehaviour
{
    [SerializeField] private UniversalRenderPipelineAsset Pipeline;
    [SerializeField] private VolumeProfile VolumeProfile;
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private SettingsData Data;
    [SerializeField] private Camera[] Cameras;

    public SettingsData _Data => Data;

    public event SimpleVoid OnChanges = null;

    private void Awake()
    {
        string path = Path.Combine(Application.dataPath, "settings.txt");

        if (File.Exists(path))
        {
            Data = JsonUtility.FromJson<SettingsData>(File.ReadAllText(path));
        }
        else
        {
            Data = new SettingsData();

            Resolution resolution = Screen.resolutions[Screen.resolutions.Length - 1];
            Data.XResolution = resolution.width;
            Data.YResolution = resolution.height;
            Data.ScreenMode = 2;

            //Data.LanguageID = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        }

        Apply();
    }

    public void Apply()
    {
        Screen.SetResolution(Data.XResolution, Data.YResolution, ScreenMode(Data.ScreenMode));

        StartCoroutine(SetMixer());

        Application.targetFrameRate = Data.FrameRate;
   
        foreach (VolumeComponent component in VolumeProfile.components)
        {
            if(component is LiftGammaGain)
            {
                (component as LiftGammaGain).gain.value = new Vector4(1, 1, 1, 1) * Mathf.Max(-0.8f, Data.Brightness);
            }
            else
            {
                component.active = Data.PostProcessing;
            }
        }
        VolumeProfile.isDirty = true;

        Pipeline.shadowDistance = Data.ShadowDistance;
        Pipeline.shadowCascadeCount = Mathf.Clamp(Data.ShadowCascades + 1, 1, 4);
        switch (Data.ShadowCascades)
        {
            case 0:
                Pipeline.mainLightShadowmapResolution = 512;
                break;
            case 1:
                Pipeline.mainLightShadowmapResolution = 2048;
                break;
            case 2:
                Pipeline.mainLightShadowmapResolution = 4096;
                break;
            case 3:
                Pipeline.mainLightShadowmapResolution = 8192;
                break;
        }
        QualitySettings.globalTextureMipmapLimit = 2 - Data.TextureLevel;
        Pipeline.supportsDynamicBatching = true;

        switch (Data.AntiAliasing)
        {
            case 0:
                foreach(Camera camera in Cameras)
                {
                    camera.GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.None;
                }
                break;
            case 1:
                foreach (Camera camera in Cameras)
                {
                    camera.GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                }
                break;
            case 2:
                foreach (Camera camera in Cameras)
                {
                    camera.GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                }
                break;
            case 3:
                foreach (Camera camera in Cameras)
                {
                    camera.GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.TemporalAntiAliasing;
                }
                break;
        }

        if (OnChanges != null)
        {
            OnChanges.Invoke();
        }

        File.WriteAllText(Path.Combine(Application.dataPath, "settings.txt"), JsonUtility.ToJson(Data));
    }

    private FullScreenMode ScreenMode(int index)
    {
        switch (index)
        {
            case 0:
                return FullScreenMode.Windowed;
            case 1:
                return FullScreenMode.ExclusiveFullScreen;
            case 2:
                return FullScreenMode.FullScreenWindow;
        }

        return FullScreenMode.Windowed;
    }

    private IEnumerator SetMixer()
    {
        yield return new WaitForEndOfFrame();

        if (Data.Music == 0)
        {
            Mixer.SetFloat("Music", -80);
        }
        else
        {
            Mixer.SetFloat("Music", 30 * Data.Music - 30);
        }
        if (Data.Effects == 0)
        {
            Mixer.SetFloat("Effects", -80);
        }
        else
        {
            Mixer.SetFloat("Effects", 30 * Data.Effects - 30);
        }
        if (Data.Voice == 0)
        {
            Mixer.SetFloat("Voice", -80);
        }
        else
        {
            Mixer.SetFloat("Voice", 30 * Data.Voice - 30);
        }
    }

    [System.Serializable]
    public class SettingsData
    {
        //Экран и качество
        public int XResolution = 1280;
        public int YResolution = 720;
        public int ScreenMode = 0;
        public int FrameRate = -1;

        public int TextureLevel = 1; // 1/4 1/2 1
        public int SnowResolution = 1; //0- 1024, 1 -2048, 2 - 4096

        public float Brightness = 0;

        public int MaxSnowCount = 5000;// [100, 200000]

        public float ShadowDistance = 500;
        public int ShadowCascades = 1; //0 - 1, 1 - 2, 2 - 3, 3 - 4
        
        public int AntiAliasing = 1; //FXAA

        public bool PostProcessing = true;

        //Звук
        public float Music = 0.5f;
        public float Effects = 0.5f;
        public float Voice = 0.5f;

        //Гейплей
        public float Sensitivity = 1;

        public SettingsData Clone()
        {
            SettingsData data = new SettingsData();

            data.XResolution = XResolution;
            data.YResolution = YResolution;
            data.ScreenMode = ScreenMode;
            data.FrameRate = FrameRate;
            data.SnowResolution = SnowResolution;
            
            data.MaxSnowCount = MaxSnowCount;

            data.TextureLevel = TextureLevel;

            data.Brightness = Brightness;

            data .ShadowDistance = ShadowDistance;
            data .ShadowCascades = ShadowCascades;

            data.AntiAliasing = AntiAliasing;

            data.PostProcessing = PostProcessing;

            data.Sensitivity = Sensitivity;

            data.Music = Music;
            data.Effects = Effects;

            return data;
        }
    }
}
