using UnityEngine;

public class Steps : MonoBehaviour
{
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private Transform[] Points;

    [SerializeField] private Texture StepTexture;

    [SerializeField] private AudioSource WalkSound;
    [SerializeField] private AudioClip[] WalkSounds;

    public void Spawn(int point)
    {
        SnowDowner.AddDown(new SnowDowner.DownAsk(StepTexture, Points[point].position.x + Points[point].forward.x * 0.2f, Points[point].position.z + Points[point].forward.z * 0.2f, transform.eulerAngles.y, 150));

        if (Vector3.Distance(CameraChanger.Instance._Camera.position, transform.position) > 5)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Points[point].position + Vector3.up * 0.25f, Vector3.down, out hit, 1, GroundMask))
        {
            Ground ground = hit.transform.GetComponent<Ground>();
            if (ground != null)
            {
                int sound = 0;

                switch (ground._Material)
                {
                    case Ground.GroundMaterial.Wood:
                        sound = Random.Range(0, 2);
                        break;
                    case Ground.GroundMaterial.Metal:
                        sound = Random.Range(2, 4);
                        break;
                    case Ground.GroundMaterial.Beton:
                        sound = Random.Range(4, 6);
                        break;
                    case Ground.GroundMaterial.Snow:
                        sound = Random.Range(6, 8); 
                        break;
                }

                WalkSound.clip = WalkSounds[sound];
                WalkSound.pitch = Random.Range(1f, 1.2f);
                WalkSound.Play();
            }
        }
    }
}
