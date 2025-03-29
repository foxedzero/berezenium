using UnityEngine;

public class PasechnikBattler : BearBattler
{
    //public override string _Describtion => $"ПАСЕЧНИК\n\nБазовая мощь: {BasePower} + 1\n\nМодификаторы:\nАтака = 1.25\nЗащита = 1.3\nУклонение = 1\n\nСкорость: [4; 7]\nШанс крита: [4; 8]\n\nПолучает +1 к мощи.\nОтсутствует штраф к сложности при атаке на атаку противника.\nЕсли удар критический - противник будет оглушён.\nПолучает на 25% меньше урона.\n\nМедведь, становясь пасечником, должен быть закалён и плечист. Пасечники обладают специальной подготовкой и бронекомплектом, чтобы защищать других медведей и давать отпор недовольным пчелам. В бою пасечник не будет жалеть противника, поднявшего руку на его товарища. ";

    //public override float _Power
    //{
    //    get
    //    {
    //        switch (Action)
    //        {
    //            case Actions.Attack:
    //                return (BasePower + PowerBonus[0] + 1) * 1.25f;
    //            case Actions.Block:
    //                return (BasePower + PowerBonus[0] + 1) * 1.3f;
    //            case Actions.Evade:
    //                return (BasePower + PowerBonus[0]) + 1;
    //        }

    //        return 0;
    //    }
    //}
    //public override int[] _MinMaxSpeed => new int[] {4, 7};
    //public override float[] _MinMaxKrit => new float[] { 4, 8 };

    //public override void TakeDamage(float value)
    //{
    //    base.TakeDamage(value * 0.75f);
    //}
}
