using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constructor;

public class SaveableObject : MonoBehaviour
{
    [SerializeField] private string Prefab;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private LayerMask TreeMask;

    public virtual string _SaveInfo 
    {
        get
        {
            return $"Prefab({Prefab})Transform({(int)(transform.position.x * 10) / 10f};{(int)(transform.position.z * 10) / 10f};{(int)transform.eulerAngles.y})";
        }
    }

    private void Start()
    {
        StartCoroutine(DestroyInside());
    }

    public virtual void SetInfo(Dictionary<string, string> parameters)
    {
        string[] position = parameters["Transform"].Split(";");

        Vector3 positionV = new Vector3(StaticTools.StringToFloat(position[0]), 0, StaticTools.StringToFloat(position[1]));

        if (Physics.Raycast(positionV + Vector3.up * 50, Vector3.down, out RaycastHit hit, 200, GroundMask))
        {
            transform.position = hit.point ;
        }
        else
        {
            transform.position = positionV;
        }
        transform.eulerAngles = new Vector3(0, StaticTools.StringToFloat(position[2]), 0);
    }

    private IEnumerator DestroyInside()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Debug.DrawRay(transform.position + Vector3.up * 50, Vector2.down * 200, Color.red);
        foreach (RaycastHit hit2 in Physics.RaycastAll(transform.position + Vector3.up * 50, Vector3.down, 200, TreeMask))
        {
            Destroy(hit2.transform.gameObject);
        }
    }
}
