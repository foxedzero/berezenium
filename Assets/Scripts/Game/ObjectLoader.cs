using System.Collections.Generic;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] Prefabs;

    public string[] _GetInfo
    {
        get
        {
            SaveableObject[] saveables = GetComponentsInChildren<SaveableObject>();

            string[] info = new string[saveables.Length];
            for(int i = 0; i < info.Length; i++)
            {
                info[i] = saveables[i]._SaveInfo;
            }
            
            return info;
        }
    }

    public void Load(string[] info)
    {
        foreach(string value in info)
        {
            Dictionary<string, string> parameters = StaticTools.GetParameters(value);

            foreach(GameObject gameObject in Prefabs)
            {
                if(gameObject.name == parameters["Prefab"])
                {
                    SaveableObject saveable = Instantiate(gameObject, transform).GetComponent<SaveableObject>();
                    saveable.SetInfo(parameters);
                    break;
                }
            }
        }
    }
}
