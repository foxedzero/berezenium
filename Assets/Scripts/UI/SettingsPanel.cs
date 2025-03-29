
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using Unity.VisualScripting;
using static Settings;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private Settings Settings;

    [SerializeField] private Text Resolution;
    [SerializeField] private Text ScreenMode;
    [SerializeField] private Text PostProcess;
    [SerializeField] private Text TextureLevel;
    [SerializeField] private Text SnowResolution;
    [SerializeField] private Text ShadowCascades;
    [SerializeField] private Text SMAA;

    [SerializeField] private Text MaxSnowCountInfo;
    [SerializeField] private Slider MaxSnowCount;

    [SerializeField] private Slider Brightness;
    [SerializeField] private Slider Sensitivity;
    [SerializeField] private Slider MusicVolume;
    [SerializeField] private Slider EffectsVolume;
    [SerializeField] private Slider VoiceVolume;

    public void ApplySettings() => Settings.Apply();

    private void OnEnable()
    {
        UpdateInfo();
    }

    private void OnDisable()
    {
        ApplySettings();
    }

    public void ScaleIt(Text text)
    {
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth + 100, 50);
    }

    public void UpdateInfo()
    {
        Resolution.text = $"{Settings._Data.XResolution}x{Settings._Data.YResolution}";
        ScaleIt(Resolution);

        switch (Settings._Data.ScreenMode)
        {
            case 0:
                ScreenMode.text = "ОКНО";
                break;
            case 1:
                ScreenMode.text = "ПОЛНЫЙ ЭКРАН";
                break;
            case 2:
                ScreenMode.text = "РАЗВЁРНУТОЕ ОКНО";
                break;
        }
        ScaleIt(ScreenMode);

        PostProcess.text = Settings._Data.PostProcessing ? "ВКЛЮЧЕНА" : "ОТКЛЮЧЕНА";
        ScaleIt(PostProcess);

        switch (Settings._Data.TextureLevel)
        {
            case 0:
                TextureLevel.text = "НИЗКОЕ";
                break;
            case 1:
                TextureLevel.text = "СРЕДНЕЕ";
                break;
            case 2:
                TextureLevel.text = "ВЫСОКОЕ";
                break;
        }
        ScaleIt(TextureLevel);

        switch (Settings._Data.SnowResolution)
        {
            case 0:
                SnowResolution.text = "НИЗКОЕ";
                break;
            case 1:
                SnowResolution.text = "СРЕДНЕЕ";
                break;
            case 2:
                SnowResolution.text = "ВЫСОКОЕ";
                break;
        }
        ScaleIt(SnowResolution);

        switch (Settings._Data.AntiAliasing)
        {
            case 0:
                SMAA.text = "Нет";
                break;
            case 1:
                SMAA.text = "FXAA";
                break;
            case 2:
                SMAA.text = "SMAA";
                break;
            case 3:
                SMAA.text = "TAA";
                break;
        }
        ScaleIt(SMAA);

        switch (Settings._Data.ShadowCascades)
        {
            case 0:
                ShadowCascades.text = "НИЗКОЕ";
                break;
            case 1:
                ShadowCascades.text = "СРЕДНЕЕ";
                break;
            case 2:
                ShadowCascades.text = "ВЫСОКОЕ";
                break;
            case 3:
                ShadowCascades.text = "ОТЛИЧНОЕ";
                break;
        }
        ScaleIt(ShadowCascades);

        MaxSnowCount.SetValueWithoutNotify(Settings._Data.MaxSnowCount / 200000f);
        MaxSnowCountInfo.text = $"{Settings._Data.MaxSnowCount}";

        Brightness.SetValueWithoutNotify((Settings._Data.Brightness + 1) / 2);

        Sensitivity.SetValueWithoutNotify(Settings._Data.Sensitivity / 8f);

        VoiceVolume.SetValueWithoutNotify(Settings._Data.Voice);

        MusicVolume.SetValueWithoutNotify(Settings._Data.Music);

        EffectsVolume.SetValueWithoutNotify(Settings._Data.Effects);
    }

    public void SetResolution()
    {
        Vector2[] resolutions =GetResolutions();

        string[] variants = new string[resolutions.Length];
        int[] indexes = new int[resolutions.Length];    
        for(int i = 0; i < variants.Length; i++)
        {
            variants[i] = $"{resolutions[i].x}x{resolutions[i].y}";
            indexes[i] = i;
        }

        UserInteract.AskVariants("Разрешение экрана", variants, indexes, SetResolution);
    }
    public void SetResolution(int index)
    {
        Vector2[] resolutions = GetResolutions();

        Settings._Data.XResolution = (int)resolutions[index].x;
        Settings._Data.YResolution = (int)resolutions[index].y;

        Resolution.text = $"{Settings._Data.XResolution}x{Settings._Data.YResolution}";
        ScaleIt(Resolution);

        Settings.Apply();
    }

    public void SetScreenMode()
    {
        UserInteract.AskVariants("Режим экрана", new string[] {"Окно","Полный экран","Развёрнутое окно"}, new int[] {0, 1, 2}, SetScreenMode);
    }
    public void SetScreenMode(int index)
    {
        Settings._Data.ScreenMode = index;

        switch (Settings._Data.ScreenMode)
        {
            case 0:
                ScreenMode.text = "ОКНО";
                break;
            case 1:
                ScreenMode.text = "ПОЛНЫЙ ЭКРАН";
                break;
            case 2:
                ScreenMode.text = "РАЗВЁРНУТОЕ ОКНО";
                break;
        }
        ScaleIt(ScreenMode);

        Settings.Apply();
    }

    public void SetPostProcess()
    {
        UserInteract.AskVariants("Пост обработка", new string[] { "Включена", "Отключена" }, new int[] { 1, 0 }, SetPostProcess);
    }
    public void SetPostProcess(int index)
    {
        Settings._Data.PostProcessing = index == 1;

        PostProcess.text = Settings._Data.PostProcessing ? "ВКЛЮЧЕНА" : "ОТКЛЮЧЕНА";
        ScaleIt(PostProcess);

        Settings.Apply();
    }

    public void SetTextureLevel()
    {
        UserInteract.AskVariants("Качество текстур", new string[] { "Низкое", "Среднее", "Высокое" }, new int[] { 0, 1, 2 }, SetTextureLevel);
    }
    public void SetTextureLevel(int index)
    {
        Settings._Data.TextureLevel = index;

        switch (Settings._Data.TextureLevel)
        {
            case 0:
                TextureLevel.text = "НИЗКОЕ";
                break;
            case 1:
                TextureLevel.text = "СРЕДНЕЕ";
                break;
            case 2:
                TextureLevel.text = "ВЫСОКОЕ";
                break;
        }
        ScaleIt(TextureLevel);

        Settings.Apply();
    }

    public void SetSnowResolution()
    {
        UserInteract.AskVariants("Тесселяция снега", new string[] { "Низкое", "Среднее", "Высокое" }, new int[] { 0, 1, 2 }, SetSnowResolution);
    }
    public void SetSnowResolution(int index)
    {
        Settings._Data.SnowResolution = index;

        switch (Settings._Data.SnowResolution)
        {
            case 0:
                SnowResolution.text = "НИЗКОЕ";
                break;
            case 1:
                SnowResolution.text = "СРЕДНЕЕ";
                break;
            case 2:
                SnowResolution.text = "ВЫСОКОЕ";
                break;
        }
        ScaleIt(SnowResolution);

        Settings.Apply();
    }

    public void SetSMAA()
    {
        UserInteract.AskVariants("Сглаживание", new string[] { "Нет", "FXAA", "SMAA", "TAA" }, new int[] { 0, 1, 2, 3 }, SetSMAA);
    }
    public void SetSMAA(int index)
    {
        Settings._Data.AntiAliasing = index;

        switch (Settings._Data.AntiAliasing)
        {
            case 0:
                SMAA.text = "Нет";
                break;
            case 1:
                SMAA.text = "FXAA";
                break;
            case 2:
                SMAA.text = "SMAA";
                break;
            case 3:
                SMAA.text = "TAA";
                break;
        }
        ScaleIt(SMAA);

        Settings.Apply();
    }

    public void SetShadowQualit()
    {
        UserInteract.AskVariants("Качество теней", new string[] { "Низкое", "Среднее", "Высокое", "Отличное" }, new int[] { 0, 1, 2, 3 }, SetShadowQualit);
    }
    public void SetShadowQualit(int index)
    {
        Settings._Data.ShadowCascades = index;

        switch (Settings._Data.ShadowCascades)
        {
            case 0:
                ShadowCascades.text = "НИЗКОЕ";
                break;
            case 1:
                ShadowCascades.text = "СРЕДНЕЕ";
                break;
            case 2:
                ShadowCascades.text = "ВЫСОКОЕ";
                break;
            case 3:
                ShadowCascades.text = "ОТЛИЧНОЕ";
                break;
        }
        ScaleIt(ShadowCascades);

        Settings.Apply();
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
        Settings._Data.Sensitivity = value * 8;

        Settings.Apply();
    }

    public void SetBrightness(float value)
    {
        Settings._Data.Brightness = value * 2 - 1;

        Settings.Apply();
    }

    public void SetMaxSnow(float value)
    {
        Settings._Data.MaxSnowCount = (int)Mathf.Max(100, value * 200000);
        MaxSnowCountInfo.text = $"{Settings._Data.MaxSnowCount}";

        Settings.Apply();
    }

    public void SetMusic(float value)
    {
        Settings._Data.Music = value;

        if (Settings._Data.Music == 0)
        {
            Mixer.SetFloat("Music", -80);
        }
        else
        {
            Mixer.SetFloat("Music", 30 * Settings._Data.Music - 30);
        }
    }

    public void SetVoice(float value)
    {
        Settings._Data.Voice = value;

        if (Settings._Data.Voice == 0)
        {
            Mixer.SetFloat("Voice", -80);
        }
        else
        {
            Mixer.SetFloat("Voice", 30 * Settings._Data.Voice - 30);
        }
    }

    public void SetEffects(float value)
    {
        Settings._Data.Effects = value;

        if (Settings._Data.Effects == 0)
        {
            Mixer.SetFloat("Effects", -80);
        }
        else
        {
            Mixer.SetFloat("Effects", 30 * Settings._Data.Effects - 30);
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
