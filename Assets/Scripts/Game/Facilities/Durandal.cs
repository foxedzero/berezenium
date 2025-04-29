using UnityEngine;
using UnityEngine.SceneManagement;

public class Durandal : Facility
{
    public override bool _CanBeHeated =>false;

    public override int _ColdEndurance => 10;
    public override float _Effectivity => 1;
    public override CityFactors.Factor[] GetEffectivityFactors()
    {
        return new CityFactors.Factor[] {new CityFactors.Factor("Космолёт", 1)};
    }

    public override void Interact()
    {
        SoundEffector.PlayFasilityIntro(IntroSound);

        string[] variants = new string[] { "Информация", "Снести", "Автоматически назначить", $"Вернуться домой (50 медведей на борту, 500 энергомёда на топливо)" };
        int[] indexes = new int[] { -1, 0, 1, 2 };

        UserInteract.AskVariants($"{ConstructInfo.Name} #{StaticTools.IndexOf(City._DataBase._Facilities, this)}", variants, indexes, RightMouseActions);
    }
    public override void RightMouseActions(int index)
    {
        switch (index)
        {
            case -1:
                WindowCreator.CreateWindow<FacilityWindow>().SetInfo(this);
                break;
            case 0:
                UserInteract.AskConfirm("Снос", $"Вы собираетесь дать распоряженио о сносе данного строения.\nПри сносе здания вы получите половину от его стоимости строительства (древесина: {ConstructInfo.WoodCost / 2f}, металл: {ConstructInfo.MetalCost / 2f}, березениум: {ConstructInfo.BerezenuimCost / 2f}).\nВы уверены, что хотите снести космолёт ?", Deconstruct);
                break;
            case 1:
                AutoAssign();
                break;
            case 2:
                if(Bears.Length < 50)
                {
                    UserInteract.AskMessage("Все медведи не на борту !", $"Вам необходимо, чтобы в космолёте находилось 50 медведей.\nМы не можем оставить своих товарищей.");
                }
                else if(City._Storage._EnergyHoney < 500)
                {
                    UserInteract.AskMessage("Недостаточно ресурсов !", $"Вам необходимо иметь на складе 500 единиц энергомёда, чтобы улететь.");
                }
                else
                {
                    UserInteract.AskConfirm("Завершение игры", "Вы готовы вернуться домой ?", EndGame);
                }
                break;
        }
    }
    public void EndGame(bool answer)
    {
        if (answer)
        {
            FindObjectOfType<GameEnder>().ShowPanel();
        }
    }

    public override void AutoAssign()
    {
        foreach (Bear bear in AssignedBears)
        {
            bear._Facility = null;
        }
        Bear[] bears = City._DataBase._Bears;

        AssignedBears = new Bear[0];

        for (int i = 0; i < bears.Length; i++)
        {
            if (bears[i]._Sally == null)
            {
                if (bears[i]._Facility != null)
                {
                    bears[i]._Facility.AssignBear(bears[i], true);
                }

                AssignedBears = StaticTools.ExpandMassive(AssignedBears, bears[i]);
            }
        }

        SmtChanged();
    }

    protected override void UpdateOcantovkaInfo()
    {
        OcantovkaInfo.text = $"/////////\nОбъект: {ConstructInfo.Name}\nАвтономное строение\n//////////";
    }
}
