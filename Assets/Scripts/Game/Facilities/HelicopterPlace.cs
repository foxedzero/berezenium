using UnityEngine;

public class HelicopterPlace : Facility
{
    [SerializeField] private int EnergyHoney;
    [SerializeField] private int Honey;
    [SerializeField] private int Metal;
    [SerializeField] private int Wood;
    [SerializeField] private int Robots;

    [SerializeField] private int DaysLeft;

    private void Start()
    {
        City._Time.DayChanged += DayPassed;
    }

    public void DayPassed()
    {
        DaysLeft--;

        if(DaysLeft < 0)
        {
            DaysLeft = Random.Range(2, 5);
        }
        else if(DaysLeft == 0)
        {
            City._Storage._EnergyHoney += EnergyHoney;
            City._Storage._Metal += Metal;
            City._Storage._Wood += Wood;
            City._Storage._Robots += Robots;
            City._Foodstream._StoredFood += Honey;
        }
    }
}
