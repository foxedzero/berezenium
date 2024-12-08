using Nto;
using System.Net.Http;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public class NtoServerInterface : MonoBehaviour
{
    private static NtoServerInterface Instance;
    private HttpClient HttpClient = new HttpClient();

    public delegate void StringInfo(string value);

    private string Nickname = "";
    private Nto.Log Log;
    private ResourceType Key;
    private string Data = "";
    private StringInfo Callback = null;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void CreatePlayer(Nto.Player player, Nto.Log log, StringInfo callback)
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (callback != null)
            {
                callback.Invoke("");
            }
            return;
        }

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(player);

        Instance.StartCoroutine(Instance.PostRequest("https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/players/", json, callback));

        json = log.Serialize();
        Instance.StartCoroutine(Instance.PostRequest("https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/logs/", json, null));

        Shop shop = new Shop();
        shop.name = "MetaShop";
        shop.resources = new ShopResources();
        shop.resources.CoffeeMachine = 1000;
        shop.resources.MusicBox = 1000;
        shop.resources.EmotionBuffer = 2500;
        shop.resources.EffectivityBooster = 3000;
        shop.resources.WinCondition = 10000;
        json = Newtonsoft.Json.JsonConvert.SerializeObject(shop);
        Instance.StartCoroutine(Instance.PostRequest($"https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/players/{player.name}/shops/", json, null));
    }
    public static void GetShopData(string nickname, StringInfo callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (callback != null)
            {
                callback.Invoke("");
            }
            return;
        }
        Instance.StartCoroutine(Instance.GetRequest($"https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/players/{nickname}/shops/MetaShop/", callback));
    }

    public static void GetSaveData(string nickname, StringInfo callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if(callback != null)
            {
                callback.Invoke("");
            }
            return;
        }
        Instance.StartCoroutine(Instance.GetRequest($"https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/players/{nickname}", callback));
    }

    public static void PutPlayerData(Nto.Log log, Nto.Player player, StringInfo callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback.Invoke("");
            return;
        }

        Nto.Resources resources = player.resources;

        Nto.ResourceChange change = new ResourceChange();
        change.resources = resources;
       log.player_name = player.name;
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(change);
        Instance.StartCoroutine(Instance.PutRequest($"https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/players/{player.name}/", json, callback));

        json = log.Serialize();
        Instance.StartCoroutine(Instance.PostRequest("https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/logs/", json, null));
    }
    public static void SetResource(Nto.Log log, ResourceType key, string data, StringInfo callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback.Invoke("");
            return;
        }
        Instance.Nickname = log.player_name;
        Instance.Log = log;
        Instance.Key = key;
        Instance.Data = data;
        Instance.Callback = callback;

        GetSaveData(log.player_name, Instance.SetResource);
    }
    public void SetResource(string callback)
    {
        if(callback == "")
        {
            Callback.Invoke("");
            return;
        }

        Nto.Player player = Newtonsoft.Json.JsonConvert.DeserializeObject<Nto.Player>(callback);

        Nto.Resources resources = player.resources;

        switch (Key)
        {
            case ResourceType.SaveData:
                resources.SaveData = Data;
                Log.resources_changed.Add("SaveData", $"{player.resources.SaveData} ---------------------------------------------------------------------------------->>>>> {resources.SaveData}");
                break;
            case ResourceType.LapoMoney:
                resources.LapoMoney = StaticTools.StringToInt(Data);
                Log.resources_changed.Add("LapoMoney", $"{player.resources.LapoMoney} -> {resources.LapoMoney}");
                break;
        }

        Nto.ResourceChange change = new ResourceChange();
        change.resources = resources;

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(change);
        StartCoroutine(PutRequest($"https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/players/{Nickname}/", json, Callback));

        json = Log.Serialize();
        StartCoroutine(PostRequest("https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/logs/", json, null));

        Nickname = "";
        Data = "";
        Log = null;
    }

    public static void DeletePlayer(string nickname, StringInfo callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (callback != null)
            {
                callback.Invoke("");
            }
            return;
        }
        Instance.StartCoroutine(Instance.DeleteRequest($"https://2025.nti-gamedev.ru/api/games/46b266a2-a385-42e1-b66d-bc9ace73d6c3/players/{nickname}", callback));
    }

    private IEnumerator GetRequest(string url, StringInfo callback)
    {
        UnityWebRequest request = new UnityWebRequest(url, "GET");

        request.downloadHandler = new DownloadHandlerBuffer();

        AsyncOperation operation = request.Send();
        float timer = 0;
        while (!operation.isDone)
        {
            timer += Time.unscaledDeltaTime;

            if(timer > 6)
            {
                Debug.Log($"Сервер не отвечает");
                if (callback != null)
                {
                    callback.Invoke("");
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

        if (request.isNetworkError)
        {
            Debug.LogError(request.error);
            if (callback != null)
            {
                callback.Invoke("");
            }
        }
        else if(request.downloadHandler.text == "{\"detail\":\"No Player matches the given query.\"}")
        {
            if (callback != null)
            {
                callback.Invoke("");
            }
        }
        else
        {
            if (callback != null)
            {
                callback.Invoke(request.downloadHandler.text);
            }
        }

    }
    private IEnumerator PutRequest(string url, string json, StringInfo callback)
    {
        UnityWebRequest request = new UnityWebRequest(url, "PUT");

        request.SetRequestHeader("Content-Type", "application/json");
        byte[] jsonData = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonData);
        request.uploadHandler.contentType = "application/json";
        request.downloadHandler = new DownloadHandlerBuffer();

        AsyncOperation operation = request.Send();
        float timer = 0;
        while (!operation.isDone)
        {
            timer += Time.unscaledDeltaTime;

            if (timer > 4)
            {
                Debug.Log($"Сервер не отвечает");
                if (callback != null)
                {
                    callback.Invoke("0");
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

        if (request.isNetworkError)
        {
            Debug.LogError(request.error);
            if (callback != null)
            {
                callback.Invoke("0");
            }
        }
        else
        {
            if (callback != null)
            {
                callback.Invoke("1");
            }
        }
    }

    private IEnumerator PostRequest(string url, string json, StringInfo callback)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.SetRequestHeader("Content-Type", "application/json");
        byte[] jsonData = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonData);
        request.uploadHandler.contentType = "application/json";
        request.downloadHandler = new DownloadHandlerBuffer();

        AsyncOperation operation = request.Send();
        float timer = 0;
        while (!operation.isDone)
        {
            timer += Time.unscaledDeltaTime;

            if (timer > 5)
            {
                Debug.Log($"Сервер не отвечает");
                if (callback != null)
                {
                    callback.Invoke("0");
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

        if (request.isNetworkError)
        {
            Debug.LogError(request.error);
            if (callback != null)
            {
                callback.Invoke("0");
            }
        }
        else
        {
            if (callback != null)
            {
                callback.Invoke("1");
            }
        }
    }

    private IEnumerator DeleteRequest(string url, StringInfo callback)
    {
        UnityWebRequest request = new UnityWebRequest(url, "DELETE");

        request.downloadHandler = new DownloadHandlerBuffer();

        AsyncOperation operation = request.Send();
        float timer = 0;
        while (!operation.isDone)
        {
            timer += Time.unscaledDeltaTime;

            if (timer > 3)
            {
                Debug.Log($"Сервер не отвечает");
                if (callback != null)
                {
                    callback.Invoke("0");
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

        if (request.isNetworkError)
        {
            Debug.LogError(request.error);
            if (callback != null)
            {
                callback.Invoke("0");
            }
        }
        else
        {
            if (callback != null)
            {
                callback.Invoke("1");
            }
        }
    }
}

namespace Nto
{
    public enum ResourceType { SaveData, LapoMoney }

    public class Player
    {
        public string name;
        public Resources resources;
    }
    
    public class ResourceChange
    {
        public Resources resources;
    }
    public struct Resources
    {
        public int LapoMoney;
        public string[] ItemsID;
        public string SaveData;
    }

    public class Shop
    {
        public string name;
        public ShopResources resources;
    }
    public struct ShopResources
    {
        public int CoffeeMachine;
        public int MusicBox;
        public int EmotionBuffer;
        public int EffectivityBooster;
        public int WinCondition;
    }

    public class Log
    {
        public string comment = "";
        public string player_name = "";
        public string shop_name = "";

        public Dictionary<string, string> resources_changed = new Dictionary<string, string>();

        public string Serialize()
        {
            if(shop_name.Length > 0)
            {
                return $"{{\"comment\":\"{comment}\",\"player_name\":\"{player_name}\",\"shop_name\":\"{shop_name}\",\"resources_changed\":{Newtonsoft.Json.JsonConvert.SerializeObject(resources_changed)}}}";
            }
            else
            {
                return $"{{\"comment\":\"{comment}\",\"player_name\":\"{player_name}\",\"resources_changed\":{Newtonsoft.Json.JsonConvert.SerializeObject(resources_changed)}}}";
            }
        }
    }
}
