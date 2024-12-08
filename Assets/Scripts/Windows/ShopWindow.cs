using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;
using Nto;
using System;
using System.Collections.Generic;

public class ShopWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private Text List;
    [SerializeField] private Text Info;
    [SerializeField] private Text Lapomoney;
    [SerializeField] private RectTransform Content;
    [SerializeField] private IlusionHolders IlusionHolders;
    private ShopItem[] ShopItems = new ShopItem[0];
    private int CurrentItem = 0;

    private int LapoMoney = 0;
    private string[] PlayerItems = new string[0];
    private int Action = 0;

    public override string _Label => "Мета магазин";

    private void Start()
    {
        Info.text = "Получение информации из сервера...";
        NtoServerInterface.GetShopData(PlayerPrefs.GetString("playerName"), ReceiveData);
        NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceiveMoney);
    }

    public void ReceiveMoney(string info)
    {
        if(info == "")
        {
            return;
        }

        Player player = Newtonsoft.Json.JsonConvert.DeserializeObject<Player>(info);

        LapoMoney = player.resources.LapoMoney;

        Lapomoney.text = $"Лаподеньги: {LapoMoney}";

        PlayerItems = player.resources.ItemsID;

        if (Action == 1)
        {
            if (StaticTools.Contains(PlayerItems, ShopItems[CurrentItem].Identify))
            {
                UserInteract.AskConfirm($"Продажа", $"Вы уверены, что хотите продать \"{ShopItems[CurrentItem].Name}\" ?", Sell).OnSkip += Cancel;
            }
            else
            {
                if(LapoMoney < ShopItems[CurrentItem].Cost)
                {
                    UserInteract.AskMessage($"Отмена покупки", $"Недостаточно лаподенег для покупки.");
                    Action = 0;
                }
                else
                {
                    UserInteract.AskConfirm($"Покупка", $"Вы уверены, что хотите купить \"{ShopItems[CurrentItem].Name}\" ?", Buy).OnSkip += Cancel;
                }
            }
        }
        else if(Action == 2)
        {
            player.resources.ItemsID = StaticTools.ExpandMassive(player.resources.ItemsID, ShopItems[CurrentItem].Identify);
            player.resources.LapoMoney -= ShopItems[CurrentItem].Cost;

            Nto.Log log = new Nto.Log();
            log.comment = $"Игрок купил -{ShopItems[CurrentItem].Name}-";
            log.player_name = player.name;
            log.shop_name = "MetaShop";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("LapoMoney", $"-{ShopItems[CurrentItem].Cost}");
            dict.Add("ItemsID", $"+{ShopItems[CurrentItem].Identify}");

            log.resources_changed = dict;

            NtoServerInterface.PutPlayerData(log, player, ValidateAction);

            Action = 0;
        }
        else if (Action == 3)
        {
            player.resources.ItemsID = StaticTools.RemoveFromMassive(player.resources.ItemsID, ShopItems[CurrentItem].Identify);
            player.resources.LapoMoney += ShopItems[CurrentItem].Cost;

            Nto.Log log = new Nto.Log();
            log.comment = $"Игрок продал -{ShopItems[CurrentItem].Name}-";
            log.player_name = player.name;
            log.shop_name = "MetaShop";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("LapoMoney", $"+{ShopItems[CurrentItem].Cost}");
            dict.Add("ItemsID", $"-{ShopItems[CurrentItem].Identify}");

            log.resources_changed = dict;

            NtoServerInterface.PutPlayerData(log, player, ValidateAction);

            Action = 0;
        }
    }

    public void ValidateAction(string info)
    {
        if(info == "")
        {
            return;
        }

        UserInteract.AskMessage($"Покупка", $"Операция прошла успешно.");

        Action = 0;
        Info.text = "Получение информации из сервера...";
        NtoServerInterface.GetShopData(PlayerPrefs.GetString("playerName"), ReceiveData);
        NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceiveMoney);

        City._Buyiments.UpdateInfo();
    }

    public void ReceiveData(string info)
    {
        if (info == "")
        {
            Info.text = "Не удалось получить данные из сервера...";
            return;
        }

        Shop shop = Newtonsoft.Json.JsonConvert.DeserializeObject<Shop>(info);

        ShopItems = new ShopItem[5];
        ShopItems[0] = new ShopItem("Кофе машина", "С кофе машиной вы сможете прогуливаться по карте с неплохой скоростью.", shop.resources.CoffeeMachine, "CoffeeMachine");
        ShopItems[1] = new ShopItem("Музыкальная колонка", "Музыкальная колонка хранит некоторые треки от ZtiMarat, вы сможете прослушать их в игре (но можно и в вк).", shop.resources.MusicBox, "MusicBox");
        ShopItems[2] = new ShopItem("Стабилизатор эмоций", "С помощью стабилизатора эмоций, медведи будут получать бонус к удовлетворенности на 3 единицы.", shop.resources.EmotionBuffer, "EmotionBuffer");
        ShopItems[3] = new ShopItem("Промышленные нановнедрения", "Благодаря промышленным нановнедрением эффективность каждого здания увеличится в 1.25", shop.resources.EffectivityBooster, "EffectivityBoost");
        ShopItems[4] = new ShopItem("Концовка #2", "Если вы приобретёте это, то сразу окажетесь во второй концовке (2 концовки еще нет, поэтому переход на 1).", shop.resources.CoffeeMachine, "End2");

        string text = "";
        foreach(ShopItem item in ShopItems)
        {
            text += item.Name+ "\n";
        }
        if (text.EndsWith("\n"))
        {
            text = text.Remove(text.Length - 1);
        }
        List.text = text;

        IlusionHolders.SetInfo(OnPoint, OnClick);
        IlusionHolders._MaxIndex = ShopItems.Length - 1;
    }

    public void OnPoint(int index)
    {
        if(Action != 0)
        {
            return;
        }

        if (StaticTools.Contains(PlayerItems, ShopItems[index].Identify))
        {
            Info.text = $"<size=26>{ShopItems[index].Name}</size>\n{ShopItems[index].Description}\n\n<color=green>Уже приобретено</color>";
        }
        else
        {
            Info.text = $"<size=26>{ShopItems[index].Name}</size>\n{ShopItems[index].Description}\n\nЦена: {ShopItems[index].Cost} Лаподенег";
        }
    }

    public override void RightMouse()
    {
        UserInteract.AskVariants(_Label, new string[] { $"{(Pinned ? "открепить" : "закрепить")} окно", "закрыть окно", "Обновить информацию" }, new int[] { -1, 0, 1 }, RightMouseActions);
    }
    public override void RightMouseActions(int index)
    {
        base.RightMouseActions(index);

        if(index == 1)
        {
            if(Action == 0)
            {
                Info.text = "Получение информации из сервера...";
                NtoServerInterface.GetShopData(PlayerPrefs.GetString("playerName"), ReceiveData);
                NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceiveMoney);
            }
        }
    }

    public void OnClick(int index)
    {
        if (Action != 0)
        {
            return;
        }

        CurrentItem = index;
        Info.text = "Получаем информацию об игроке...";
        Action = 1;
        NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceiveMoney);
    }

    public void Buy(bool answer)
    {
        Action = 0;
        if (answer)
        {
            Action = 2;
            NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceiveMoney);
        }
    }

    public void Sell(bool answer)
    {
        Action = 0;
        if (answer)
        {
            Action = 3;
            NtoServerInterface.GetSaveData(PlayerPrefs.GetString("playerName"), ReceiveMoney);
        }
    }

    public void Cancel()
    {
        Action = 0;
    }

    private class ShopItem
    {
        public string Name;
        public string Description;
        public int Cost;
        public string Identify;
        
        public ShopItem(string name, string describtion, int cost, string id)
        {
            Name = name;
            Description = describtion;
                Cost = cost;
            Identify = id;
        }
    }
}
