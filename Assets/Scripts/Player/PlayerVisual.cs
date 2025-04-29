using System.Collections;
using UnityEngine;
using static BearVisual;

public class PlayerVisual : MonoBehaviour
{
    public enum ActAnimations { GroundChop, GroundMine }

    [SerializeField] private SkinnedMeshRenderer Head;
    [SerializeField] private PlayerMovement Movement;
    [SerializeField] private Animator Animator;
    [SerializeField] private float WinkTime = 0;

    [SerializeField] private AudioSource VzmahSound;

    [SerializeField] private Transform Camera;
    [SerializeField] private SkinnedMeshRenderer[] PlayerModel;

    private SimpleVoid ToReturn = null;

    public Animator _Animator => Animator;

    public void SetManaging(bool state)
    {
        Animator.SetBool("Managing", state);
    }

    public void ShowModel(bool state)
    {
        foreach (SkinnedMeshRenderer renderer in PlayerModel)
        {
            if (state)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
            else
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
    }

    private void Update()
    {
        WinkTime -= Time.unscaledDeltaTime;
        if (WinkTime < 0)
        {
            StopAllCoroutines();
            StartCoroutine(Wink());
            WinkTime = Random.Range(2, 5f);
        }
    }

    public void PlayAnimation(ActAnimations act, SimpleVoid toReturn)
    {
        if (Movement._InCapsule)
        {
            return;
        }

        ToReturn = toReturn;

        Movement.transform.localEulerAngles = new Vector3(0, Movement._Rotation.y, 0);

        switch (act)
        {
            case ActAnimations.GroundChop:
                Animator.Play("Chop");
                break;
            case ActAnimations.GroundMine:
                Animator.Play("Mine");
                break;
        }
    }
    public void ReturnAction()
    {
        ToReturn?.Invoke();
        ToReturn = null;
    }

    public void PlayVzmah()
    {
        VzmahSound.Play();
    }

    private IEnumerator Wink()
    {
        float weight = 0;

        while (weight < 100)
        {
            weight += Time.unscaledDeltaTime * 500;
            Head.SetBlendShapeWeight(0, weight);
            yield return new WaitForEndOfFrame();
        }

        while (weight > 0)
        {
            weight -= Time.unscaledDeltaTime * 500;
            Head.SetBlendShapeWeight(0, weight);
            yield return new WaitForEndOfFrame();
        }

        weight = 0;
        Head.SetBlendShapeWeight(0, weight);
    }
}
