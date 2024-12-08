using UnityEngine;
using UnityEngine.Audio;

public class EnvironmentPass : MonoBehaviour
{
    [SerializeField] private AudioMixer Mixer;

    public void SetPass(bool inside)
    {
        if (inside)
        {
            Mixer.SetFloat("EnvPass", 625);
        }
        else
        {
            Mixer.SetFloat("EnvPass", 22000);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            Mixer.SetFloat("EnvPass", 625);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            Mixer.SetFloat("EnvPass", 22000);
        }
    }
}
