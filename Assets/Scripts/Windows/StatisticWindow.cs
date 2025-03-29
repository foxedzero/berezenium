using UnityEngine;
using UnityEngine.UI;
using WindowInterfaces;

public class StatisticWindow : DefaultWindow, ISingleOne
{
    [SerializeField] private GameObject[] Panels;
    [SerializeField] private Image[] Buttons;
    [SerializeField] private StatisticCurentPanel CurrentPanel;
    [SerializeField] private int OpenedPanel;

    public override string _Label => "Статистика";

    public void SetPanel(int panel)
    {
        if(OpenedPanel == panel)
        {
            return;
        }

        OpenedPanel = panel;
        for(int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(i == OpenedPanel);
            Buttons[i].color = i == OpenedPanel ? new Color(0.25f, 0.25f, 0.25f, 1) : new Color(0, 0, 0, 0);
        }
    }
}
