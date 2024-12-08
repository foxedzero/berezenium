
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private Settings Settings;

    [SerializeField] private Text Language;

    [SerializeField] private Text Resolution;
    [SerializeField] private Text ScreenMode;
    [SerializeField] private Text PostProcess;
    [SerializeField] private Text TextureLevel;
    [SerializeField] private Text ShadowCascades;
    [SerializeField] private Text SMAA;

    [SerializeField] private Slider Sensitivity;
    [SerializeField] private Slider MusicVolume;
    [SerializeField] private Slider EffectsVolume;

    private Settings.SettingsData SettingsData;

    public void ApplySettings() => Settings.Apply(SettingsData.Clone());

    private void Start()
    {
        Settings.OnChanges += UpdateInfo;
       // LocalizationSettings.SelectedLocaleChanged += UpdateInfo;
    }
    private void OnDestroy()
    {
        Settings.OnChanges -= UpdateInfo;
       // LocalizationSettings.SelectedLocaleChanged -= UpdateInfo;
    }

    private void OnEnable()
    {
        UpdateInfo();
    }

    //public void UpdateInfo(Locale locale) => UpdateInfo();

    public void UpdateInfo()
    {
        SettingsData = Settings._Data.Clone();

        switch (SettingsData.LanguageID)
        {
            case 0:
                Language.text = $"Русский";
                break;
            case 1:
                Language.text = $"English";
                break;
            case 2:
                Language.text = $"中文";
                break;
            case 3:
                Language.text = $"Deutsch";
                break;
            case 4:
                Language.text = $"हिन्दी";
                break;
            case 5:
                Language.text = $"日本語";
                break;
        }

        Resolution.text = $"{SettingsData.XResolution}x{SettingsData.YResolution}";

        switch (SettingsData.ScreenMode)
        {
            case 0:
                ScreenMode.text = "Окно";
                break;
            case 1:
                ScreenMode.text = "Полный экран";
                break;
            case 2:
                ScreenMode.text = "Развёрнутое окно";
                break;
        }

        PostProcess.text = SettingsData.PostProcessing ? "Включена" : "Отключена";

        switch (SettingsData.TextureLevel)
        {
            case 0:
                TextureLevel.text = "Низкое";
                break;
            case 1:
                TextureLevel.text = "Среднее";
                break;
            case 2:
                TextureLevel.text = "Высокое";
                break;
        }

        switch (SettingsData.AntiAliasing)
        {
            case 0:
                SMAA.text = "Нет";
                break;
            case 1:
                SMAA.text = "2x";
                break;
            case 2:
                SMAA.text = "4x";
                break;
            case 3:
                SMAA.text = "8x";
                break;
        }

        switch (SettingsData.ShadowCascades)
        {
            case 0:
                ShadowCascades.text = "Низкое";
                break;
            case 1:
                ShadowCascades.text = "Среднее";
                break;
            case 2:
                ShadowCascades.text = "Высокое";
                break;
            case 3:
                ShadowCascades.text = "Отличное";
                break;
        }
        
        Sensitivity.SetValueWithoutNotify(SettingsData.Sensitivity / 8f);

        MusicVolume.SetValueWithoutNotify(SettingsData.Music);

        EffectsVolume.SetValueWithoutNotify(SettingsData.Effects);
    }

    public void SetLanguage()
    {
       // UserInteract.AskVariants("", new string[] { "Русский", "English", "中文",  "Deutsch",  "हिन्दी", "日本語", }, new int[] { 0, 1, 2, 3, 4, 5 }, SetLanguage);
        UserInteract.AskVariants("", new string[] { "Русский" }, new int[] { 0 }, SetLanguage);
    }
    public void SetLanguage(int index)
    {
        SettingsData.LanguageID = index;
        ApplySettings();
    }

    public void SetResolution()
    {
        Vector2[] resolutions = GetResolutions();

        string[] variants = new string[resolutions.Length];
        int[] indexes = new int[resolutions.Length];
        for (int i = 0; i < resolutions.Length; i++)
        {
            variants[i] = $"{resolutions[resolutions.Length - 1 - i].x}x{resolutions[resolutions.Length - 1 - i].y}";
            indexes[i] = resolutions.Length - 1 - i;
        }

        UserInteract.AskVariants("", variants, indexes, SetResolution);
    }
    public void SetResolution(int index)
    {
        Vector2 resolution = GetResolutions()[index];
        SettingsData.XResolution = (int)resolution.x;
        SettingsData.YResolution = (int)resolution.y;

        Resolution.text = $"{SettingsData.XResolution}x{SettingsData.YResolution}";
    }

    public void SetScreenMode()
    {
        UserInteract.AskVariants("", new string[] { "Окно", "Полный экран", "Развёрнутое окно" }, new int[] { 0, 1, 2 }, SetScreenMode);
    }
    public void SetScreenMode(int index)
    {
        SettingsData.ScreenMode = index;

        switch (SettingsData.ScreenMode)
        {
            case 0:
                ScreenMode.text = "Окно";
                break;
            case 1:
                ScreenMode.text = "Полный экран";
                break;
            case 2:
                ScreenMode.text = "Развёрнутое окно";
                break;
        }
    }

    public void SetPostProcess()
    {
        UserInteract.AskVariants("", new string[] {"Включить", "Отключить"}, new int[] { 0, 1}, SetPostProcess);
    }
    public void SetPostProcess(int index)
    {
        SettingsData.PostProcessing = index == 0;

        PostProcess.text = SettingsData.PostProcessing ? "Включена" : "Отключена";
    }
    public void SetPostProcess(bool state)
    {
        SettingsData.PostProcessing = state;
    }

    public void SetTextureLevel()
    {
        UserInteract.AskVariants("", new string[] { "Низкое", "Среднее", "Высокое" }, new int[] { 0, 1, 2 }, SetTextureLevel);
    }
    public void SetTextureLevel(int index)
    {
        SettingsData.TextureLevel = index;

        switch (SettingsData.TextureLevel)
        {
            case 0:
                TextureLevel.text = "Низкое";
                break;
            case 1:
                TextureLevel.text = "Среднее";
                break;
            case 2:
                TextureLevel.text = "Высокое";
                break;
        }
    }

    public void SetMSAA()
    {
        UserInteract.AskVariants("", new string[] { "Нет", "2x", "4x", "8x" }, new int[] { 0, 1, 2, 3 }, SetMSAA);
    }
    public void SetMSAA(int index)
    {
        SettingsData.AntiAliasing = index;

        switch (SettingsData.AntiAliasing)
        {
            case 0:
                SMAA.text = "Нет";
                break;
            case 1:
                SMAA.text = "2x";
                break;
            case 2:
                SMAA.text = "4x";
                break;
            case 3:
                SMAA.text = "8x";
                break;
        }
    }

    public void SetShadowCascades()
    {
        UserInteract.AskVariants("", new string[] { "Низкое", "Среднее", "Высокое", "Отличное" }, new int[] { 0, 1, 2, 3 }, SetShadowCascades);
    }
    public void SetShadowCascades(int index)
    {
        SettingsData.ShadowCascades = index;

        switch (SettingsData.ShadowCascades)
        {
            case 0:
                ShadowCascades.text = "Низкое";
                break;
            case 1:
                ShadowCascades.text = "Среднее";
                break;
            case 2:
                ShadowCascades.text = "Высокое";
                break;
            case 3:
                ShadowCascades.text = "Отличное";
                break;
        }
    }

    public void EditKeyMap()
    {
        string path = Path.Combine(Application.dataPath, "keyConfig.txt");
        if (!File.Exists(path))
        {
            InputManager._Instance.SaveKeyMap();
        }

        Application.OpenURL(Path.Combine(Application.dataPath, "keyConfig.txt"));
    }

    public void SetSensitivity(float value)
    {
        SettingsData.Sensitivity = value * 8;
    }

    public void SetMusic(float value)
    {
        SettingsData.Music = value;

        if (SettingsData.Music == 0)
        {
            Mixer.SetFloat("Music", -80);
        }
        else
        {
            Mixer.SetFloat("Music", 30 * SettingsData.Music - 30);
        }
    }

    public void SetEffects(float value)
    {
        SettingsData.Effects = value;

        if (SettingsData.Effects == 0)
        {
            Mixer.SetFloat("Effects", -80);
        }
        else
        {
            Mixer.SetFloat("Effects", 30 * SettingsData.Effects - 30);
        }
    }

    private Vector2[] GetResolutions()
    {
        Vector2[] resolutions = new Vector2[0];
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Vector2 resolution = new Vector2(Screen.resolutions[i].width, Screen.resolutions[i].height);

            if (!StaticTools.Contains(resolutions, resolution))
            {
                resolutions = StaticTools.ExpandMassive(resolutions, resolution);
            }
        }

        return resolutions;
    }
}
