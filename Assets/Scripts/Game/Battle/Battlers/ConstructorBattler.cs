using UnityEngine;

public class ConstructorBattler : BearBattler
{
    //public override string _Describtion => $"КОНСТРУКТОР\n\nБазовая мощь: {BasePower}\n\nМодификаторы:\nАтака = 1.5\nЗащита = 1\nУклонение = 0.6\n\nСкорость: [2; 5]\nШанс крита: 6\n\nКогда его действие - атака, атакует в ответ (без штрафов) всех тех, кто на него нападает.\n\nМедведи становятся конструкторами, чтобы реализовать немыслемые конструкции творцов, мегапроекты и создавать роботов-дронов. Они отвечают за инфраструктуру городов. В поле боя ведут себя медлительно, однако способны показать свою силу.\n";

    //public override float _Power
    //{
    //    get
    //    {
    //        switch (Action)
    //        {
    //            case Actions.Attack:
    //                return 1.5f * (BasePower + PowerBonus[0]);
    //            case Actions.Block:
    //                return (BasePower + PowerBonus[0]);
    //            case Actions.Evade:
    //                return (BasePower + PowerBonus[0]) * 0.6f;
    //        }

    //        return 0;
    //    }
    //}
    //public override int[] _MinMaxSpeed => new int[] { 2, 5 };
    //public override float[] _MinMaxKrit => new float[] { 6, 6 };

    //public override bool _SideAction => true;
}
