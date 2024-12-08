using UnityEngine;

public class CoffeeMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource Sound;
    [SerializeField] private PlayerMovement PlayerMovement;

    private void OnDisable()
    {
        PlayerMovement._Speed = 5;
    }

    public void Interact()
    {
        UserInteract.AskInput("Изменить скорость игрока", SetSpeed);
    }

    public void SetSpeed(string value)
    {
        Sound.Play();
        float speed = Mathf.Clamp(StaticTools.StringToFloat(value), 3f, 30f);

        PlayerMovement._Speed = speed;
    }
}
