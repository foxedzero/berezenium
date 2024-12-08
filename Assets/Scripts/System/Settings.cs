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

            //Data.LanguageID = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        }

        Apply(Data);
    }

    public void Apply(SettingsData data)
    {
        Data = data; 

        Screen.SetResolution(Data.XResolution, Data.YResolution, ScreenMode(Data.ScreenMode));

        StartCoroutine(SetMixer());

        foreach (VolumeComponent component in VolumeProfile.components)
        {
            component.active = Data.PostProcessing;
        }

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
        Pipeline.msaaSampleCount = Data.AntiAliasing > 0 ? Mathf.RoundToInt(Mathf.Pow(2, Data.AntiAliasing)) : 0;

        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[Data.LanguageID];

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

        public float ShadowDistance = 500;
        public int ShadowCascades = 1; //0 - 1, 1 - 2, 2 - 3, 3 - 4
        
        public int AntiAliasing = 1; //0x 2x 4x 8x

        public bool PostProcessing = true;

        //Звук
        public float Music = 0.5f;
        public float Effects = 0.5f;

        //Язык
        public int LanguageID;

        //Гейплей
        public float Sensitivity = 1;

        public SettingsData Clone()
        {
            SettingsData data = new SettingsData();

            data.XResolution = XResolution;
            data.YResolution = YResolution;
            data.ScreenMode = ScreenMode;
            data.FrameRate = FrameRate;

            data.TextureLevel = TextureLevel;

            data .ShadowDistance = ShadowDistance;
            data .ShadowCascades = ShadowCascades;

            data.AntiAliasing = AntiAliasing;

            data.PostProcessing = PostProcessing;

            data.Sensitivity = Sensitivity;

            data.Music = Music;
            data.Effects = Effects;

            data.LanguageID = LanguageID;

            return data;
        }
    }
}
