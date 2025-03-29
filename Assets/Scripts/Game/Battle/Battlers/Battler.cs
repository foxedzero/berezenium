
using UnityEngine;

public class Battler : MonoBehaviour
{
    //public enum Actions {Attack, Block, Evade, Heal}
    //public enum BattleAnimation {Pose, Strafe, Attack, Block, Evade, TakeDamage}
    //public enum BattleEffects {Attack, Block}

    //public event SimpleVoid OnChange = null;

    //[SerializeField] protected BattleProvider BattleProvider;
    //[SerializeField] protected BattlerUI UI;
    //[SerializeField] protected Animator Animator;
    //[SerializeField] protected GameObject Ocantovka;
    //[SerializeField] protected MeshRenderer BattleCircle;

    //[SerializeField] protected Battler Target;
    //[SerializeField] protected Actions Action;
    //[SerializeField] protected int Speed;

    //[SerializeField] protected float[] PowerBonus = new float[2]; // 0 - текущий, 1 - след ход

    //[SerializeField] protected float Health;

    //[SerializeField] protected bool Stunned = false;

    //[Header("Эффекты")]
    //[SerializeField] protected GameObject OverPowerCircle;
    //[SerializeField] protected GameObject StunEffect;

    //[SerializeField] protected GameObject AttackEffect;
    //[SerializeField] protected Transform AttackEffectPoint;

    //[SerializeField] protected GameObject BlockEffect;

    //[Header("...")]

    //public Battler _Target
    //{
    //    get
    //    {
    //        return Target;
    //    }
    //    set
    //    {
    //        Target = value;
    //    }
    //}
    //public Actions _Action
    //{
    //    get
    //    {
    //        return Action;
    //    }
    //    set
    //    {
    //        Action = value;

    //        BattleCircle.material = BattleProvider._ActionMaterial[Action.GetHashCode()];

    //        OnChange?.Invoke();
    //    }
    //}

    //public virtual ColoredSprite[] _Icon => new ColoredSprite[1]; 
    //public virtual string _Name => "";
    //public virtual string _Describtion => "";

    //public virtual float _MaxHealth => 100;
    //public float _Health
    //{
    //    get
    //    {
    //        return Health;
    //    }
    //    set
    //    {
    //        Health = value;

    //        OnChange?.Invoke();
    //    }
    //}
    //public virtual float _Power => 1;
    //public float[] _PowerBonus => PowerBonus;
    //public virtual int[] _MinMaxSpeed => new int[] { 2, 5 };
    //public virtual float[] _MinMaxKrit => new float[] { 2, 6 };
    //public int _Speed
    //{
    //    get
    //    {
    //        return Speed;
    //    }
    //    set
    //    {
    //        Speed = value;
    //    }
    //}
    //public bool _Stunned
    //{
    //    get
    //    {
    //        return Stunned;
    //    }
    //    set
    //    {
    //        Stunned = value;
    //        StunEffect.SetActive(value);
    //    }
    //}
    //public virtual bool _SideAction
    //{
    //    get
    //    {
    //        if(Action == Actions.Block || Action == Actions.Evade)
    //        {
    //            return true;
    //        }

    //        return false;
    //    }
    //}
    //public virtual bool _FarAttack => false;
   
    //public void SetBattleUI(bool active) => UI.gameObject.SetActive(active);
    //public void SetIndicate(bool state) => Ocantovka.SetActive(state);

    //public virtual void NewRound()
    //{
    //    OverPowerCircle.SetActive(PowerBonus[0] > 0);
    //    OverPowerCircle.transform.localScale = new Vector3(1, 8 * PowerBonus[0], 1);

    //    OnChange?.Invoke();
    //}

    //public virtual void NextAction()
    //{
    //    _Action = (Actions)((Action.GetHashCode() + 1) % 3);
    //}

    //public virtual void SetAnimation(BattleAnimation animation)
    //{

    //}

    //public virtual void TakeDamage(float value)
    //{
    //    _Health -= value;
    //}

    //public virtual void PlayEffect(BattleEffects effect, float force)
    //{
    //    switch (effect)
    //    {
    //        case BattleEffects.Attack:
    //            Transform attack = Instantiate(AttackEffect, null).transform;
    //            attack.position = AttackEffectPoint.position;
    //            attack.rotation = AttackEffectPoint.rotation;
    //            attack.localScale = new Vector3(0.5f, 0.5f, 0.5f) * Mathf.Max(force, 1);
    //            break;
    //        case BattleEffects.Block:
    //            Transform block = Instantiate(BlockEffect, null).transform;
    //            block.position = transform.position + transform.forward * 0.5f + Vector3.up * 1.5f;
    //            block.rotation = transform.rotation;
    //            block.localScale = new Vector3(0.5f, 0.5f, 0.5f) * Mathf.Max(force, 1);
    //            break;
    //    }
    //}

    //public class ColoredSprite
    //{
    //    public Sprite Sprite;
    //    public Color Color;

    //    public ColoredSprite(Sprite sprite, Color color)
    //    {
    //        Sprite = sprite;
    //        Color = color;
    //    }
    //}
}
