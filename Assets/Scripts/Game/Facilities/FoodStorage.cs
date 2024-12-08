using UnityEngine;

public class FoodStorage : Facility
{
    [SerializeField] private float Capacity;

    public override bool _CanBeDisabled => false;
    public override bool _CanBeHeated => false;

    public float _Capacity => Capacity;

    public override float _Effectivity => 1;
    public override float GetPotencialEffectivity() => 1;
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        return new CityFactors.Factor[] { new CityFactors.Factor("Автономное строение", 1, -1, true) };
    }
}
