using UnityEngine;

public class BioinginerBattler : BearBattler
{
    //public override string _Describtion => $"БИОИНЖЕНЕР\n\nБазовая мощь: {BasePower}\n\nМодификаторы:\nАтака = 0.75\nЗащита = 1\nУклонение = 1\nЛечение = 0.5\n\nСкорость: [3; 6]\nШанс крита: 10\n\nСпособен Лечить союзников.\nСтоит в арьергарде (будет атакован последним).\n\nМедведь, который избрал путь биоинженера, несёт свет органических учений. Он с энтузиазмом будет заниматься как анатомией, так и ботаникой с биоинженерией, выведением лучших пчёл и растений и так далее. Здесь, на поле сражений, берёт ответственность за лечение союзников.";

    //public override float _Power
    //{
    //    get
    //    {
    //        switch (Action)
    //        {
    //            case Actions.Attack:
    //                return 0.75f * (BasePower + PowerBonus[0]);
    //            case Actions.Block:
    //                return (BasePower + PowerBonus[0]);
    //            case Actions.Evade:
    //                return (BasePower + PowerBonus[0]);
    //            case Actions.Heal:
    //                return (BasePower + PowerBonus[0]) * 0.5f;
    //        }

    //        return 0;
    //    }
    //}
    //public override int[] _MinMaxSpeed => new int[] { 3, 6 };
    //public override float[] _MinMaxKrit => new float[] { 10, 10 };

    //public override void NextAction()
    //{
    //    _Action = (Actions)((Action.GetHashCode() + 1) % 4);

    //    if(Target != null && StaticTools.Contains(BattleProvider._PlayerSide, Target) && Action != Actions.Heal)
    //    {
    //        Target = null;
    //    }
    //}
}
