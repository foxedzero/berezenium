using UnityEngine;

public class Snowrunner : MonoBehaviour
{
    [SerializeField] private Texture Sleif;
    [SerializeField] private float Interval = 1;
    [SerializeField] private float Size = 1;
    private float Timer = 0;

    private void Update()
    {
        Timer -= Time.deltaTime;
        if(Timer < 0)
        {
            Timer = Interval + Random.Range(-0.1f, 0.1f);

            SnowDowner.AddDown(new SnowDowner.DownAsk(Sleif, transform.position.x, transform.position.z, transform.eulerAngles.y, Size));
        }
    }
}
