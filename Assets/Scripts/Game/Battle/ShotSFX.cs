
using UnityEngine;

public class ShotSFX : BattleSfx
{
    [SerializeField] private Transform Shot;
    [SerializeField] private Transform Bar;
    [SerializeField] private Transform Exp;
    private float Distance;
    private float SideScale = 0.25f;
    private float Force = 0;

    public void SetInfo(Vector3 shot, Vector3 hit, float force)
    {
        Distance = (hit - shot).magnitude;
        Vector3 direction = (hit - shot) / Distance;

        Force = force * 0.1f;
        SideScale = Force;

        Shot.position = shot + direction * 0.5f;
        Bar.position = (shot + hit)/2;
        Exp.position = hit - direction * 0.5f;

        Bar.localScale = new Vector3(SideScale, SideScale, Distance - 1);

        Quaternion rotation = Quaternion.LookRotation(direction);

        Shot.rotation = rotation;
        Bar.rotation = rotation;
        Exp.rotation = rotation;
    }

    protected override void Update()
    {
        base.Update();

        SideScale -= Force * Time.unscaledDeltaTime;
        Bar.localScale = new Vector3(SideScale, SideScale, Distance - 1);
    }
}
