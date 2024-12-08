using UnityEngine;

public class UserInteract : MonoBehaviour
{
    [SerializeField] private GameObject UserInputPrefab;
    [SerializeField] private GameObject ChooseVariantPrefab;
    [SerializeField] private GameObject UserConfirmPrefab;
    [SerializeField] private GameObject UserMessagePrefab;
    [SerializeField] private GameObject UserColorPrefab;
    [SerializeField] private GameObject UserBearPrefab;
    [SerializeField] private GameObject UserFacilityPrefab;

    [SerializeField] private Transform Canvas;
    private static UserInteract Instance;
    private GameObject LastInteract = null;

    private void Awake()
    {
        Instance = this;
    }

    static public UserFacility AskFacility(string label, UserFacility.FacilityReturn ask)
    {
        if (Instance.LastInteract != null)
        {
            Destroy(Instance.LastInteract);
        }

        UserFacility facility = Instantiate(Instance.UserFacilityPrefab, Instance.Canvas).GetComponent<UserFacility>();

        facility.SetInfo(label, ask);

        return facility;
    }

    static public UserBear AskBear(string label, UserBear.BearReturn ask)
    {
        if (Instance.LastInteract != null)
        {
            Destroy(Instance.LastInteract);
        }

        UserBear bear = Instantiate(Instance.UserBearPrefab, Instance.Canvas).GetComponent<UserBear>();

        bear.SetInfo(label, ask);

        return bear;
    }

    static public UserInput AskInput(string label, UserInput.Delegate ask)
    {
        if(Instance.LastInteract !=  null)
        {
            Destroy(Instance.LastInteract);
        }

        UserInput userInput = Instantiate(Instance.UserInputPrefab, Instance.Canvas).GetComponent<UserInput>();

        userInput.SetInfo(ask, label);

        Instance.LastInteract = userInput.gameObject;

        return userInput;
    }

    static public ChooseVariant AskVariants(string label, string[] variants, int[] indexes, ChooseVariant.VariantChosed ask)
    {
        if (Instance.LastInteract != null)
        {
            Destroy(Instance.LastInteract);
        }

        ChooseVariant variant = Instantiate(Instance.ChooseVariantPrefab, Instance.Canvas).GetComponent<ChooseVariant>();
        variant.SetInfo(label, variants, indexes, ask);

        Instance.LastInteract = variant.gameObject;

        return variant;
    }

    static public UserConfirm AskConfirm(string label, string info, UserConfirm.BoolAnswer ask)
    {
        if(Instance.LastInteract != null)
        {
            Destroy(Instance.LastInteract);
        }

        UserConfirm confirm = Instantiate(Instance.UserConfirmPrefab, Instance.Canvas).GetComponent<UserConfirm>();
        confirm.SetInfo(label, ask, info);

       Instance.LastInteract= confirm.gameObject;

        return confirm;
    }

    static public UserMessage AskMessage(string label, string info)
    {
        if (Instance.LastInteract != null)
        {
            Destroy(Instance.LastInteract);
        }

        UserMessage mesage = Instantiate(Instance.UserMessagePrefab, Instance.Canvas).GetComponent<UserMessage>();
        mesage.SetInfo(label, info);

        Instance.LastInteract = mesage.gameObject;

        return mesage;
    }

    static public UserColor AskColor(UserColor.ColorVoid ask, Color startColor)
    {
        UserColor user = Instantiate(Instance.UserColorPrefab, Instance.Canvas).GetComponent<UserColor>();
        user.SetInfo(ask, startColor);

        return user;
    }
}
