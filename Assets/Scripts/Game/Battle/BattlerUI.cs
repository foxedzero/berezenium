using UnityEngine;
using UnityEngine.UI;

public class BattlerUI : MonoBehaviour
{
    //[SerializeField] private Battler Battler;
    //[SerializeField] private Transform Canvas;

    //[SerializeField] private Text Name;
    //[SerializeField] private Text Power;
    //[SerializeField] private Text Speed;
    //[SerializeField] private Text HP;
    //[SerializeField] private Image Action;
    //[SerializeField] private Slider HPBar;

    //[SerializeField] private Sprite[] ActionIcons;

    //[SerializeField] private float ScaleCoefficient;
    //private Transform Camera;

    //private void Start()
    //{
    //    Camera = FindObjectOfType<Camera>().transform;
    //    Battler.OnChange += UpdateInfo;
    //    UpdateInfo();
    //}

    //public void UpdateInfo()
    //{
    //    Name.text = Battler._Name;

    //    Action.sprite = ActionIcons[Battler._Action.GetHashCode()];
    //    if (Battler._Action == 0 && Battler._FarAttack)
    //    {
    //        Action.sprite = ActionIcons[4];
    //    }

    //    Power.text = $": {Battler._Power}{(Battler._PowerBonus[0] > 0 ? $" (+{Battler._PowerBonus[0]})" : "")}";
    //    Speed.text = $": {Battler._Speed}";
    //    HP.text = $"ОЗ: {Mathf.RoundToInt(Battler._Health)}/{Battler._MaxHealth}";
    //    HPBar.value = Battler._Health / Battler._MaxHealth;
    //}

    //private void Update()
    //{
    //    Vector3 rotation = Quaternion.LookRotation(Camera.position - transform.position).eulerAngles;
    //    rotation.x = 0;
    //    rotation.z = 0;

    //    transform.eulerAngles = rotation;

    //    Canvas.transform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(Camera.position, transform.position) * ScaleCoefficient, 0.01f, 0.05f);
    //}
}
