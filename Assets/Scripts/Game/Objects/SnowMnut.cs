using UnityEngine;

public class SnowMnut : MonoBehaviour
{
    [SerializeField] private Vector2Int Size;
    [SerializeField] private float Mastab;
    [SerializeField] private bool Mnul;
    private SnowDowner.DownAsk DownAsk = new SnowDowner.DownAsk();

    private void Generate()
    {
        DownAsk = new SnowDowner.DownAsk();
        Texture2D texture = new Texture2D(Size.x, Size.y);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                if (y == 0)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else if (x == 0)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else if (x == Size.x - 1)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else if (y == Size.y - 1)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.black);
                }
            }
        }
        texture.Apply();

        DownAsk.Mask = texture;
        DownAsk.x = transform.position.x;
        DownAsk.z = transform.position.z;
        DownAsk.rotation = transform.eulerAngles.y;
        DownAsk.mastab = Mastab;
    }

    private void Update()
    {
        if (!Mnul)
        {
            Generate();
            if (SnowDowner.AddDown(DownAsk))
            {
                Mnul = true;
            }
        }
    }
}
