using UnityEngine;

public class PervoprohodecBattler : BearBattler
{
    //[SerializeField] private int Streak;

    //public override string _Describtion => $"ПЕРВОПРОХОДЕЦ\n\nБазовая мощь: {BasePower}\n\nМодификаторы:\nАтака = 1\nЗащита = 0.8\nУклонение = 1.5\n\nСкорость: [4; 8]\nШанс крита: [5; 10]\n\nСкорость повышается за каждую единицу мощи.\nШансы крита повышаются за каждую единицу скорости.\nЕсли удар критический - атакует случайного противника, плоть до 3 раз.\n\nМедведи-первопроходцы всегда критиковались за их чрезмерный авантюризм, расточительство и безбашенность. Однако благодаря им медведи смогли покорять моря, воздух, космос и другие планеты. В сражении они раскроют всю свою решимость и стремительность.";

    //public override float _Power
    //{
    //    get
    //    {
    //        switch (Action)
    //        {
    //            case Actions.Attack:
    //                return  BasePower + PowerBonus[0];
    //            case Actions.Block:
    //                return (BasePower + PowerBonus[0]) * 0.8f;
    //            case Actions.Evade:
    //                return (BasePower + PowerBonus[0]) * 1.5f;
    //        }

    //        return 0;
    //    }
    //}
    //public int _Streak
    //{
    //    get
    //    {
    //        return Streak;
    //    }
    //    set
    //    {
    //        Streak = value;
    //    }
    //}
    //public override int[] _MinMaxSpeed => new int[] { 4 + (int)(BasePower + PowerBonus[0]), 8 + (int)(BasePower + PowerBonus[0]) };
    //public override float[] _MinMaxKrit => new float[] { 5 + Speed, 10 + Speed };

    //public override void PlayEffect(BattleEffects effect, float force)
    //{
    //    base.PlayEffect(effect, force);

    //    switch (effect)
    //    {
    //        case BattleEffects.Attack:
    //            transform.position += transform.forward * Random.Range(1, 10f);
    //            break;
    //    }
    //}
}
