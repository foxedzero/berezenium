using UnityEngine;
using UnityEngine.UI;

public class ScheduleHolder : MonoBehaviour
{
    [SerializeField] private ScheduleWindow Window;
    [SerializeField] private Text Name;
    [SerializeField] private MyButton[] Hours;
    [SerializeField] private Schedule Schedule = null;

    public void SetInfo(ScheduleWindow window, Schedule schedule)
    {
        Name.text = $"Расписание #{schedule._Index}";

        Window = window ;
        Schedule = schedule;

        for(int i = 0; i < schedule._Hours.Length; i++)
        {
            Hours[i]._DefaultColor = Schedule._Hours[i] == '1' ? new Color(0.2f, 0.2f, 0.75f) : new Color(1, 1, 1);
            Hours[i]._ActiveColor = Schedule._Hours[i] == '1' ? new Color(0.1f, 0.1f, 0.45f) : new Color(0.5f, 0.5f, 0.5f);
        }
    }

    public void AddBear()
    {
        UserBear userBear = UserInteract.AskBear($"Назначить на распорядок #{Schedule._Index}", AddBear, UserBear.Sorting.Schedule);
        userBear.SetBearSorting(6);
    }
    public void AddBear(Bear bear)
    {
        City._CitySchedule._Schedules[bear._Schedule].RegisterBear(bear, true);

        Schedule.RegisterBear(bear, false);
    }

    public void ShowBears()
    {
        string[] variants = new string[Schedule._Bears.Length];
        int[] indexes = new int[Schedule._Bears.Length];
        for(int i = 0; i < variants.Length; i++)
        {
            indexes[i] = i;
            variants[i] = $"{Schedule._Bears[i]._Kasta} {Schedule._Bears[i]._Name}";
        }

        if(variants.Length < 1)
        {
            UserInteract.AskMessage("Пусто", "Ни один медведь не назначен.");
            return;
        }

        UserInteract.AskVariants("Список медведей", variants, indexes, SelectBear);
    }
    public void SelectBear(int index)
    {
        WindowCreator.CreateWindow<BearWindow>().SetInfo(Schedule._Bears[index]);
    }

    public void Delete()
    {
        UserInteract.AskConfirm("Удалить распорядок", "Вы собираетесь удалить распорядок. \nМедведи, которые его придерживаются, перейдут на первый распорадок.", Delete);
    }
    public void Delete(bool state)
    {
        if (state)
        {
            City._CitySchedule.RemoveSchedule(Schedule);
        }
    }

    public void ChangeHour(int index)
    {
        Schedule._Hours = Schedule._Hours.Remove(index) + (Schedule._Hours[index] == '1' ? "0" : "1") + Schedule._Hours.Remove(0, index + 1);

        Hours[index]._DefaultColor = Schedule._Hours[index] == '1' ? new Color(0.2f, 0.2f,0.75f) : new Color(1,1,1);
        Hours[index]._ActiveColor = Schedule._Hours[index] == '1' ? new Color(0.1f, 0.1f, 0.45f) : new Color(0.5f, 0.5f, 0.5f);
    }
}
