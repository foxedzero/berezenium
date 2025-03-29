
using UnityEngine;
using UnityEngine.UI;

public class BattlerInfo : MonoBehaviour
{
    //[SerializeField] private RectTransform RectTransform;
    //[SerializeField] private Image[] Icon;
    //[SerializeField] private Text Name;
    //[SerializeField] private Image Action;
    //[SerializeField] private Text Power;
    //[SerializeField] private Text Speed;
    //[SerializeField] private Slider HpBar;
    //[SerializeField] private Text HpInfo;
    //[SerializeField] private Text Describtion;

    //[SerializeField] private Sprite[] ActionIcons;

    //[SerializeField] private Animator Animator;

    //private Battler Battler = null;
    //private bool Hidden = false;

    //private void Start()
    //{
    //    if(Battler == null)
    //    {
    //        Animator.SetBool("isOpen", false);
    //    }
    //}

    //public void SetBattler(Battler battler)
    //{
    //    if(Battler != null)
    //    {
    //        Battler.OnChange -= UpdateInfo;
    //    }

    //    Battler = battler;

    //    if(Battler == null)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        Animator.SetBool("isOpen", !Hidden);
    //    }

    //    Battler.OnChange += UpdateInfo;

    //    UpdateInfo();
    //}

    //public void UpdateInfo()
    //{
    //    foreach (Image image in Icon)
    //    {
    //        image.gameObject.SetActive(false);
    //    }
    //    for (int i = 0; i < Battler._Icon.Length; i++)
    //    {
    //        Icon[i].gameObject.SetActive(true);
    //        Icon[i].sprite = Battler._Icon[i].Sprite;
    //        Icon[i].color = Battler._Icon[i].Color;
    //    }

    //    Name.text = Battler._Name;

    //    Action.sprite = ActionIcons[Battler._Action.GetHashCode()];
    //    if (Battler._Action == 0 && Battler._FarAttack)
    //    {
    //        Action.sprite = ActionIcons[4];
    //    }

    //    Power.text = $": {Battler._Power}{(Battler._PowerBonus[0] > 0 ? $" (+{Battler._PowerBonus[0]})" : "")}";
    //    Speed.text = $": {Battler._Speed}";
    //    HpInfo.text = $"ОЗ: {Mathf.RoundToInt(Battler._Health)}/{Battler._MaxHealth}";
    //    HpBar.value = Battler._Health / Battler._MaxHealth;

    //    Describtion.text = Battler._Describtion;

    //    if (Describtion.text.Length == 0)
    //    {
    //        Describtion.gameObject.SetActive(false);
    //        RectTransform.sizeDelta = new Vector2(500, 160);
    //    }
    //    else
    //    {
    //        Describtion.gameObject.SetActive(true);

    //        float height = Mathf.Min(Describtion.preferredHeight, StaticTools.ScreenHeight - 286);

    //        Describtion.rectTransform.sizeDelta = new Vector2(468, height);
    //        RectTransform.sizeDelta = new Vector2(500, 176 + height);
    //    }
    //}

    //public void Hide()
    //{
    //    if(Battler == null)
    //    {
    //        return;
    //    }

    //    Hidden = !Hidden;
    //    Animator.SetBool("isOpen", !Hidden);
    //}
}
