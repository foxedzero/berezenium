using UnityEngine;

public class CityHeater : MonoBehaviour
{
    private Facility[] FacitiltyHeaters = new Facility[0];

    public float _EnergyConsume
    {
        get
        {
            float value = 0;

            foreach(Facility facility in FacitiltyHeaters)
            {
               if(facility._Heater == 1)
                {
                    value += 5;
                }
            }

            return value;
        }
    }

    public void ApplyFacilityHeater(Facility facility, bool remove)
    {
        int index = StaticTools.IndexOf(FacitiltyHeaters, facility);

        if (remove)
        {
            if(index > -1)
            {
                FacitiltyHeaters = StaticTools.ReduceMassive(FacitiltyHeaters, index);
            }
        }
        else
        {
            if(index < 0)
            {
                FacitiltyHeaters = StaticTools.ExpandMassive(FacitiltyHeaters, facility);
            }
        }


    }

    public void Heat()
    {
        foreach (Facility facility in FacitiltyHeaters)
        {
            switch (facility._Heater)
            {
                case 1:
                    facility._Heated = Mathf.RoundToInt(2 * City._Energosystem._Effectivity);
                    break;
                case 2:
                    if(City._Storage._EnergyHoney >= 1)
                    {
                        City._Storage._EnergyHoney -= 1;
                        facility._Heated = 2;
                    }
                    else
                    {
                        facility._Heated = 0;
                    }
                    break;
                case 3:
                    if (City._Storage._Wood >= 3)
                    {
                        City._Storage._Wood -= 3;
                        facility._Heated = 2;
                    }
                    else
                    {
                        facility._Heated = 0;
                    }
                    break;
            }
        }
    }
}
