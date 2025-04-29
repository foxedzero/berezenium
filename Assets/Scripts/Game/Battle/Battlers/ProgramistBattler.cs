using UnityEngine;

public class ProgramistBattler : BearBattler
{
    //[SerializeField] private Transform Dron;

    //public override string _Describtion => $"ПРОГРАММИСТ\n\nБазовая мощь: {BasePower}\n\nМодификаторы:\nАтака = 1\nЗащита = 0.7\nУклонение = 1\n\nСкорость: [1; 4]\nШанс крита: [10; 15]\n\nСтоит в арьергарде (будет атакован последним).\nАтакует дистанционно.\nИмеет бонус к пониженной сложности.\n\nМедведи программисты работают с большими данными, сложными вычислениями, а также настройкой и программированием роботов и дронов. В современное время без них никак не обойтись. В сражении стоят позади и, настраивая дрона, атакуют издалека.";

    //public override float _Power
    //{
    //    get
    //    {
    //        switch (Action)
    //        {
    //            case Actions.Attack:
    //                return  (BasePower + PowerBonus[0]);
    //            case Actions.Block:
    //                return (BasePower + PowerBonus[0]) * 0.7f;
    //            case Actions.Evade:
    //                return (BasePower + PowerBonus[0]) * 1;
    //        }

    //        return 0;
    //    }
    //}
    //public override int[] _MinMaxSpeed => new int[] { 1, 4 };
    //public override float[] _MinMaxKrit => new float[] { 10, 15 };

    //public override bool _FarAttack => true;

    //private void Update()
    //{
    //    if (Target != null)
    //    {
    //        Dron.rotation = Quaternion.LookRotation(Target.transform.position + Vector3.up * 1.5f - Dron.position);
    //    }
    //    else
    //    {
    //        Dron.localEulerAngles = Vector3.zero;
    //    }
    //}
    //public override void PlayEffect(BattleEffects effect, float force)
    //{
    //    switch (effect)
    //    {
    //        case BattleEffects.Attack:

    //            ShotSFX attack = Instantiate(AttackEffect, null).GetComponent<ShotSFX>();
    //            attack.SetInfo(AttackEffectPoint.position, Target.transform.position + Vector3.up * 1.5f, force);
    //            break;
    //        case BattleEffects.Block:
    //            base.PlayEffect(effect, force);
    //            break;
    //    }
    //}
}
