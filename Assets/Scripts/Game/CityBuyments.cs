using UnityEngine;
using UnityEngine.SceneManagement;

public class CityBuyments : MonoBehaviour
{
    [SerializeField] private GameObject MusicBox;
    [SerializeField] private GameObject CoffeeMachine;
    [SerializeField] private bool EffectivityBoost;
    [SerializeField] private bool EmotionBuffer;

    public bool _EffectivityBoost => EffectivityBoost;
    public bool _EmotionBuffer => EmotionBuffer;

    private void Start()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        MusicBox.SetActive(false);
        CoffeeMachine.SetActive(false);
        EffectivityBoost = false;
        EmotionBuffer = false;

        NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceivePlayerData);
    }

    public void ReceivePlayerData(string info)
    {
        if(info == "")
        {
            return;
        }

        Nto.Player player = Newtonsoft.Json.JsonConvert.DeserializeObject<Nto.Player>(info);

        foreach(string item in player.resources.ItemsID)
        {
            switch (item)
            {
                case "CoffeeMachine":
                    CoffeeMachine.SetActive(true);
                    break;
                case "MusicBox":
                    MusicBox.SetActive(true);
                    break;
                case "EmotionBuffer":
                    EffectivityBoost = true;
                    break;
                case "EffectivityBoost":
                    EmotionBuffer = true;
                    break;
                case "End2":
                    SceneManager.LoadScene(3);
                    break;
            }
        }
    }

   
}
