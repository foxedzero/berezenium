using UnityEngine;

public class Gathering : MonoBehaviour
{
    [SerializeField] private Transform AxePoint;
    [SerializeField] private Transform PickaxePoint;
    [SerializeField] private LayerMask WoodLayer;

    public void GatherWood()
    {
        foreach(Collider collider in Physics.OverlapSphere(AxePoint.position, 0.25f, WoodLayer))
        {
            City._Storage._Wood += 2;
            break;
        }
    }

    public void GatherOre()
    {
        foreach (Collider collider in Physics.OverlapSphere(AxePoint.position, 0.25f, WoodLayer))
        {
            City._Storage._Wood += 2;
            break;
        }
    }
}
