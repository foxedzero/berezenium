using UnityEngine;
using System.IO;

public class CityDataBase : MonoBehaviour
{
    [SerializeField] private Constructor FacilityBuilder;

    [SerializeField] private Facility[] Facilities = new Facility[0];
    [SerializeField] private Bear[] Bears = new Bear[0];

    public event SimpleVoid OnBearChanges = null;
    public event SimpleVoid OnFacilityChanges = null;

    public Facility[] _Facilities => Facilities;
    public Bear[] _Bears => Bears;

    public void RegisterFacility(Facility facility, bool remove)
    {
        int index = StaticTools.IndexOf(Facilities, facility);
        if (remove)
        {
            if (index > -1)
            {
                Facilities = StaticTools.ReduceMassive(Facilities, index);

                if (OnFacilityChanges != null)
                {
                    OnFacilityChanges.Invoke();
                }
            }
        }
        else
        {
            if (index < 0)
            {
                Facilities = StaticTools.ExpandMassive(Facilities, facility);

                if (OnFacilityChanges != null)
                {
                    OnFacilityChanges.Invoke();
                }
            }
        }

    }
    public void RegisterBear(Bear bear, bool remove)
    {
        int index = StaticTools.IndexOf(Bears, bear);
        if (remove)
        {
            if(index > -1)
            {
                Bears = StaticTools.ReduceMassive(Bears, index);

                if(OnBearChanges != null)
                {
                    OnBearChanges.Invoke();
                }
            }
        }
        else
        {
            if(index < 0)
            {
                Bears = StaticTools.ExpandMassive(Bears, bear);

                if (OnBearChanges != null)
                {
                    OnBearChanges.Invoke();
                }
            }
        }

    }
    public void RegisterBear(Bear[] bear, bool remove)
    {
        if (remove)
        {
            Bear[] bears = new Bear[Bears.Length - bear.Length];
            int index = 0;
            foreach (Bear bear1 in Bears)
            {
                if (!StaticTools.Contains(bear, bear1))
                {
                    bears[index] = bear1;
                    index++;
                }
            }

            Bears = bears;

            if (OnBearChanges != null)
            {
                OnBearChanges.Invoke();
            }
        }
        else
        {
            Bears = StaticTools.ExpandMassive(Bears, bear);

            if (OnBearChanges != null)
            {
                OnBearChanges.Invoke();
            }
        }

    }

    public void Load(SaveData saveData)
    {
        Bears = new Bear[saveData.Bears.Length];
        for(int i = 0; i < saveData.Bears.Length; i++)
        {
            Bears[i] = new Bear();
            Bears[i]._SaveInfo = saveData.Bears[i];
        }

        Facilities = new Facility[saveData.Facilities.Length];
        for(int i = 0; i < saveData.Facilities.Length; i++)
        {
            Facilities[i] = FacilityBuilder.Load(saveData.Facilities[i]);

        }
    }
}
