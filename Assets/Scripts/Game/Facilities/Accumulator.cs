using UnityEngine;

public class Accumulator : Facility
{
    [SerializeField] private float Capacity;

    public float _Capacity => Capacity;

    public override bool _CanBeHeated => false;

    public override float _Effectivity => 1;
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        return new CityFactors.Factor[] {new CityFactors.Factor("Автономное строение", 1, true)};
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////\nОбъект: {ConstructInfo.Name}\nАвтономное строение\n/////////";
    }
}
