using UnityEngine;
using UnityEngine.SceneManagement;

public class Durandal : Facility
{
    public override bool _CanBeHeated =>false;
    public override bool _CanBeDisabled => false;

    public override int _ColdEndurance => 10;
    public override float _Effectivity => 1;
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        return new CityFactors.Factor[] {new CityFactors.Factor("Космолёт", 1, -1)};
    }

    public override void RightMouse()
    {
        UserInteract.AskVariants("Распоряжение", new string[] { "Снести", $"Вернуться домой (необходимо 100 энергомёда, в наличии: {City._Storage._EnergyHoney})" }, new int[] { 0, 1 }, RightMouseActions);
    }
    public override void RightMouseActions(int index)
    {
        switch (index)
        {
            case 0:
                UserInteract.AskConfirm("Снос", $"Вы собираетесь дать распоряженио о сносе данного строения.\nПри сносе здания вы получите половину от его стоимости строительства (древесина: {ConstructInfo.WoodCost / 2f}, металл: {ConstructInfo.MetalCost / 2f}, березениум: {ConstructInfo.BerezenuimCost / 2f}).\nВы уверены, что хотите снести космолёт ?", Deconstruct);
                break;
            case 1:
                if(City._Storage._EnergyHoney >= 100)
                {
                    UserInteract.AskConfirm("Завершение игры", "Вы готовы вернуться домой ?", EndGame);
                }
                else
                {
                    UserInteract.AskMessage("Недостаточно ресурсов", $"На данный момент вам не хватает {100 - City._Storage._EnergyHoney} до 500 энергомёда");
                }
                break;
        }
    }
    public void EndGame(bool answer)
    {
        if (answer)
        {
            SceneManager.LoadScene(3);
        }
    }

    public override bool AssignBear(Bear bear, bool remove)
    {
        return false;
    }

    public override float GetPotencialEffectivity()
    {
        return 1;
    }
}
