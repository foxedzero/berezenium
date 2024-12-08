using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Titles : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform;
    [SerializeField] private Image CG;
    [SerializeField] private GameObject EndLabel;
    [SerializeField] private float Distance;
    [SerializeField] private float SpeedMultiplier;
    [SerializeField] private float Speed;

    private bool Going = false;

    public void SetMultiplier(float value) { SpeedMultiplier = value; }
    public void Exit()
    {
        if (Distance == -228 && !Going)
        {
            Going = true;
            StartCoroutine(GoToMenu());
        }
    }


    private void Update()
    {
        if (Distance > 0)
        {
            float speed = Time.unscaledDeltaTime * SpeedMultiplier * Speed;
            Distance -= speed;
            RectTransform.anchoredPosition += Vector2.up * speed;
        }
        else if (Distance != -228)
        {
            Distance = -228;
            EndLabel.SetActive(true);
        }
    }

    private IEnumerator GoToMenu()
    {
        float value = 1;

        while(value > 0)
        {
            value -= Time.unscaledDeltaTime;
            CG.color = Color.white * value;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(0);
    }

}
