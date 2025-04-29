using System.Collections.Generic;
using UnityEngine;

public class BattlerSpawner : MonoBehaviour
{
    //[SerializeField] private GameObject[] BearBattlersPrefab;
    //[SerializeField] private GameObject[] BattlerPrefabs;
    //[SerializeField] private BearModelData BearModels;

    //public Battler Spawn(Bear bear)
    //{
    //    GameObject prefab;
    //    Mesh brows;
    //    Mesh head;
    //    switch (bear._Kasta)
    //    {
    //        case Bear.Kasta.Пасечник:
    //            prefab = BearBattlersPrefab[0];
    //            brows = BearModels._PasechnikBrows[bear._Brows];
    //            head = BearModels._PasechnikHeads[bear._Face];
    //            break;
    //        case Bear.Kasta.Конструктор:
    //            prefab = BearBattlersPrefab[1];
    //            brows = BearModels._ConstructorBrows[bear._Brows];
    //            head = BearModels._ConstructorHeads[bear._Face];
    //            break;
    //        case Bear.Kasta.Программист:
    //            prefab = BearBattlersPrefab[2];
    //            brows = BearModels._ProgramistBrows[bear._Brows];
    //            head = BearModels._ProgramistHeads[bear._Face];
    //            break;
    //        case Bear.Kasta.Биоинженер:
    //            prefab = BearBattlersPrefab[3];
    //            brows = BearModels._BioingenerBrows[bear._Brows];
    //            head = BearModels._BioingenerHeads[bear._Face];
    //            break;
    //        case Bear.Kasta.Первопроходец:
    //            prefab = BearBattlersPrefab[4];
    //            brows = BearModels._PervoprohodecBrows[bear._Brows];
    //            head = BearModels._PervoprohodecHeads[bear._Face];
    //            break;
    //        default:
    //            prefab = BearModels._PasechnikPrefab;
    //            brows = BearModels._PasechnikBrows[bear._Brows];
    //            head = BearModels._PasechnikHeads[bear._Face];
    //            break;
    //    }

    //    BearBattler bearBattler = Instantiate(prefab, null).GetComponent<BearBattler>();

    //    bearBattler._Visual._Brows.sharedMesh = brows;
    //    bearBattler._Visual._Head.sharedMesh = head;
    //    for (int i = 0; i < bearBattler._Visual._Skin.Length; i++)
    //    {
    //        List<Material> materials = new List<Material>();
    //        bearBattler._Visual._Skin[i].GetSharedMaterials(materials);
    //        materials[bearBattler._Visual._SkinId[i]] = BearObjectioner._Instance._SkinMaterials[bear._BodyColor];
    //        bearBattler._Visual._Skin[i].SetSharedMaterials(materials);
    //    }

    //    bearBattler.SetInfo(bear._Name, bear._Health * 10, 1, new int[] { bear._Face, bear._Brows, bear._BodyColor});

    //    return bearBattler;
    //}

    //public Battler Spawn(string battler)
    //{
    //    foreach(GameObject prefab in BattlerPrefabs)
    //    {
    //        if (prefab.name == battler)
    //        {
    //            return Instantiate(prefab, null).GetComponent<Battler>();
    //        }
    //    }

    //    Debug.Log($"<color=red>Не найден противник</color> {battler}");
    //    return null;
    //}
}
