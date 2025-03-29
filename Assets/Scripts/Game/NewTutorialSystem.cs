using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class NewTutorialSystem : MonoBehaviour
{
    public static NewTutorialSystem Instance;

    [SerializeField] private GameObject TutorialPanel;
    [SerializeField] private Text Info;
    [SerializeField] private Image ProgressBar;
    [SerializeField] private int Stage;
    [SerializeField] private bool Closed = false;

    [SerializeField] private GameObject[] Locked;

    private float Progress = 0;
    public int _TutorialStage => Stage;
    public bool _Closed => Closed;

    private void Start()
    {
        Instance = this;

        if (PlayerPrefs.GetInt("Tutorial") == 0)
        {
            TutorialPanel.SetActive(false);
            Closed = true;
            return;
        }

        Stage = PlayerPrefs.GetInt("TutorialTip");

        foreach (GameObject gameObject in Locked)
        {
            gameObject.SetActive(false);
        }

        TutorialPanel.SetActive(true);  
    }

    public void StartCapsuleQuit()
    {
        if (Stage == 0)
        {
            Progress += 1;
        }
    }

    public void SaveAbleCollected()
    {
        if(Stage == 3)
        {
            Progress += 0.2f;
        }
    }

    public void BearSaved()
    {
        if (Stage == 4)
        {
            Progress += 0.5f;
        }
    }

    public void ManageModeCameraRotated()
    {
        if(Stage == 6)
        {
            Progress += 0.005f;
        }
    }

    public void ManageModeCameraMoved()
    {
        if (Stage == 7)
        {
            Progress += 0.005f;
        }
    }

    public void OpenedBuildWindow() 
    {
        if(Stage == 8)
        {
            Progress += 1;
        }
    }
    public void BarakSelected()
    {
        if (Stage == 9)
        {
            Progress += 1;
        }
    }
    public void BarakSetuped()
    {
        if (Stage == 10)
        {
            Progress += 1;
        }
    }
    public void SalliesOpened()
    {
        if(Stage == 11)
        {
            Progress += 1;
        }
    }
    public void SallyCreated()
    {
        if (Stage == 12)
        {
            Progress += 1;
        }
    }
    public void AutoEquipSally()
    {
        if (Stage == 13)
        {
            Progress += 1;
        }
    }
    public void SendSally()
    {
        if (Stage == 14)
        {
            Progress += 1;
        }
    }
    public void GamePaused()
    {
        if (Stage == 15)
        {
            Progress += 1;
        }
    }
    public void PokazatelsOpened()
    {
        if (Stage == 16)
        {
            Progress += 1;
        }
    }
    public void MessageClicked()
    {
        if (Stage == 17)
        {
            Progress += 1;
        }
    }
    public void SallyDestinated()
    {
        if (Stage == 18)
        {
            Progress += 1;
        }
    }
    public void VoprosCollected()
    {
        if (Stage == 19)
        {
            Progress += 1;
        }
    }
    public void SallyToBase()
    {
        if (Stage == 20)
        {
            Progress += 1;
        }
    }
    public void SallyDeleted()
    {
        if (Stage == 21)
        {
            Progress += 1;
        }
    }

    private void Update()
    {
        if (Closed)
        {
            return;
        }

        if (InputManager.GetButtonDown(InputManager.ButtonEnum.ShowTutorial))
        {
            foreach (GameObject gameObject in Locked)
            {
                gameObject.SetActive(true);
            }
            TutorialPanel.SetActive(false);
            Closed = true;

            PlayerPrefs.SetInt("Tutorial", 0);
            return;
        }

        switch (Stage)
        {
            case 0: //приветствие (скип туториала) либо его продолжение
                {
                    Info.text = $"Вас приветствует обучение !\nЕсли вы уже знаете что и как здесь работает, то можете нажать <{InputManager._Instance._KeyMap.ShowTutorial}>. \nВ ином случае для продолжения нажмите <{InputManager._Instance._Interact._Keys[0]}> и выйдете из капсулы.";

                }
                break;
            case 1: //обзор мышкой
                {
                    Info.text = $"Осмотритесь вокруг с помощью мыши.";

                    Progress += (Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs( Input.GetAxis("Mouse Y"))) * 0.005f;
                }
                break;
            case 2: //передвижение на WASD
                {
                    Info.text = $"Попробуйте подвигаться с помощью <WASD> или <стрелочек>.";

                    Progress += (Mathf.Abs(InputManager.GetAxis(InputManager.AxisEnum.Horizontal)) + Mathf.Abs(InputManager.GetAxis(InputManager.AxisEnum.Vertical))) * 0.005f;
                }
                break;
            case 3: //Сбор ресурсов (~5 объектов)
                {
                    Info.text = $"Итак, ваша первая задача - собрать ресурсы, которые раскиданы неподалёку. \nСоберите как минимум 5 объектов.";
                }
                break;
            case 4: //Спасение медведей
                {
                    Info.text = $"Теперь когда у вас есть капитал, можно пробудить своих товарищей. \nНайдите капсулы и откройте их.";
                }
                break;
            case 5: //Переходв в капитанский режим
                {
                    Info.text = $"Пора начинать командование ! \nПерейдите в капитанский режим с помощью клавиши <{InputManager._Instance._ManageMode._Keys[0]}>";

                    if (InputManager.GetButtonDown(InputManager.ButtonEnum.ManageMode))
                    {
                        Progress += 0.4f;
                    }
                }
                break;
            case 6: //обзор
                {
                    Info.text = $"Данный режим отличается наличием интерфейса, поэтому мышь свободна. \nПопробуйте помотать камерой с зажатым <ПКМ> на участке экрана без интерфейса.";

                }
                break;
            case 7: //перемещение
                {
                    Info.text = $"Также зажав <ПКМ> вы можете перемещаться на <WASD> и <стрелочки>.";
                }
                break;
            case 8: //первое здание - окно строительства
                {
                    Info.text = $"Давайте сразу перейдём к делу. \nНайдите в нижнем левом углу на панели кнопок ту, на котором значок молотка и отвёртки.";
                    
                }
                break;
            case 9: //
                {
                    Info.text = $"Вы открыли окно, где содержатся проекты зданий.\nСейчас нас интересует Барак.\nЧтобы узнать по-подробнее о здании, наведитесь на него.";
                }
                break;
            case 10: //
                {
                    Info.text = $"Итак вы нашли и выбрали барак, теперь поставьте его на карте.";
                }
                break;
            case 11: //
                {
                    Info.text = $"Отлично, свободные конструкторы сразу займутся постройкой. \nСейчас вам нужно открыть панель вылазок. \nНайдите на панельке с кнопками карту - это вылазки.";
                }
                break;
            case 12: //
                {
                    Info.text = $"Не пугайтесь этой панели, она очень важна. \nНа карте будут находиться значки вопроса, нажмите на них и создайте отряд вылазки.";
                }
                break;
            case 13: //
                {
                    Info.text = $"У вас открылось окно вылазки, вы можете добавлять медведей, добавлять еду и в будущем снегоходы. \nОднако не утруждайтесь, нажмите на кнопку Снарядить.";
                }
                break;
            case 14: //
                {
                    Info.text = $"Итак, если у вас есть медведь и еда, то отряд готов действовать. \nНажмите на окне кнопку В Путь, чтобы отряд выдвинулся.";
                }
                break;
            case 15: //
                {
                    Info.text = $"Сейчас пока отряд на вылазке а строитель занят, вы можете изучить интерфейс.\nЧтобы у вас точно хватило времени, нажмите слева снизу на панели со временем паузу.\nТакже можно нажать на <{InputManager._Instance._KeyMap.Pause}>";
                }
                break;
            case 16: //
                {
                    Info.text = $"Раскройте отображение ресурсов слева сверху, эта панель очень важна.\nБез неё, как без глаз.";
                }
                break;
            case 17: //
                {
                    Info.text = $"Когда конструкция будет завершена или отряд пришел к месту назначения и т.д.\nВ правом нижнем углу придёт сообщение.\nНажмите на нёё, чтобы были в курсе.";
                }
                break;
            case 18: //
                {
                    Info.text = $"Дождитесь, когда отряд дойдёт до вопросика.\nВы можете ускорить время, нажимая на коэффициенты правее паузы.";
                }
                break;
            case 19: //
                {
                    Info.text = $"Итак, когда наш отряд на клетке с иконкой вопроса, нажмите на неё.\nВ сплывающем окне выберите свой отряд.\nЧтобы не путаться, советую переименовывать отряды в будущем.";
                }
                break;
            case 20: //
                {
                    Info.text = $"Вы же выбрали \"Вернуться домой \" ? Если нет, то не беда, выберите свой отряд и нажмите на колонию (Красный крестик).";
                }
                break;
            case 21: //
                {
                    Info.text = $"Когда отряд дойдёт до базы, стоит этот отряд расформировать.\nТак отряд скинет всех спасённых медведей и их ресурсы.";
                }
                break;
            case 22: //
                {
                    Info.text = $"Хорошо бы дать цель для начала.\nПостройте лесопилку, пасеку, древесную электростанцию и лабораторию.\nПопутно спасите близлезжащих медведей, не стоит сразу набирать множество, так как есть риск не прокормить их.\nИ ещё, медлить нельзя, погода с каждым 5 днем будет хуже.\nЭтот пункт последний, чтобы его закрыть нажмите <{InputManager._Instance._KeyMap.ShowTutorial}>.";
                }
                break;
        }

        ProgressBar.fillAmount = Progress;

        if (Progress >= 1)
        {
            Progress = 0;
            Stage++;
            PlayerPrefs.SetInt("TutorialTip", Stage);
            PlayerPrefs.Save();
        }
    }
}
