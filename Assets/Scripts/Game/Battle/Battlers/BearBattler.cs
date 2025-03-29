using UnityEngine;

public class BearBattler : Battler
{
    //[SerializeField] protected GameObject Trail;
    //[SerializeField] protected BearVisual Visual;

    //[SerializeField] protected string Name;
    //[SerializeField] protected int[] Look;
    //[SerializeField] protected float BasePower;
    //[SerializeField] protected float MaxHealth;

    //public BearVisual _Visual => Visual;

    //public override float _MaxHealth => MaxHealth;
    //public override string _Name => Name;
    //public override ColoredSprite[] _Icon
    //{
    //    get
    //    {
    //        BearLook bearLook = BearObjectioner._Instance.GetBearIcon(Look[0], Look[1], Look[2]);
    //        return new ColoredSprite[] {new ColoredSprite(bearLook.Head, bearLook.SkinColor), new ColoredSprite(bearLook.Face, Color.white), new ColoredSprite(bearLook.Brows,  Color.white)};
    //    }
    //}

    //private void Start()
    //{
    //    Animator.Play("FightPose");
    //}

    //public void SetInfo(string name, float health, int basePower, int[] look)
    //{
    //    Name = name;
    //    Look = look;
    //    BasePower = basePower;

    //    MaxHealth = health;
    //    Health = health;
    //}

    //public override void NewRound()
    //{
    //    Trail.SetActive(BasePower + PowerBonus[0] > 7);

    //    base.NewRound();
    //}

    //public override void SetAnimation(BattleAnimation animation)
    //{
    //    switch (animation)
    //    {
    //        case BattleAnimation.Pose:
    //            Animator.SetInteger("fightState", 0);
    //            break;
    //        case BattleAnimation.Strafe:
    //            Animator.SetInteger("fightState", 1);
    //            break;
    //        case BattleAnimation.Attack:
    //            Animator.SetInteger("fightState", 2);
    //            break;
    //        case BattleAnimation.Block:
    //            Animator.SetInteger("fightState", 3);
    //            break;
    //        case BattleAnimation.Evade:
    //            Animator.SetInteger("fightState", 4);
    //            break;
    //        case BattleAnimation.TakeDamage:
    //            Animator.SetInteger("fightState", 5);
    //            break;
    //    }
    //}
}
