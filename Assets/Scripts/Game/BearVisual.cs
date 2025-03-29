using UnityEngine;
using System.Collections;

public class BearVisual : MonoBehaviour
{
    [SerializeField] private BearObject BearObject;
    [SerializeField] private Transform HeadBoen;
    [SerializeField] private SkinnedMeshRenderer Brows;
    [SerializeField] private SkinnedMeshRenderer Head;
    [SerializeField] private SkinnedMeshRenderer[] Skin;
    [SerializeField] private int[] SkinId;
    [SerializeField] private float WinkTime = 0;
    private float Unwink = 0;

    private bool ClosedEyes = false;

    public Transform _HeadBone => HeadBoen;
    public SkinnedMeshRenderer _Brows => Brows;
    public SkinnedMeshRenderer _Head => Head;
    public SkinnedMeshRenderer[] _Skin => Skin;
    private float _MaxUnwink
    {
        get
        {
            if(BearObject._Bear._Tired > 0.8f || BearObject._Bear._Health <= 3 || BearObject._Bear._Effects[3] < 50)
            {
                return 60;
            }

            return  BearObject._Bear._Stress * 0.3f;
        }
    }
    public int[] _SkinId => SkinId;
    public bool _ClosedEyes
    {
        get
        {
            return ClosedEyes;
        }
        set
        {
            ClosedEyes = value;
            Head.SetBlendShapeWeight(1, 100);
        }
    }

    private void Update()
    {
        if (ClosedEyes)
        {
            return;
        }

        WinkTime -= Time.unscaledDeltaTime;
        if(WinkTime < 0)
        {
            Unwink = _MaxUnwink;
            StopAllCoroutines();
            StartCoroutine(Wink());
            WinkTime = Random.Range(2, 5f);
        }
    }

    private IEnumerator Wink()
    {
        float weight = Unwink;

        while (weight < 100)
        {
            weight += Time.unscaledDeltaTime * 500;
            Head.SetBlendShapeWeight(1, weight);
            yield return new WaitForEndOfFrame();
        }

        while (weight > Unwink)
        {
            weight -= Time.unscaledDeltaTime * 500;
            Head.SetBlendShapeWeight(1, weight);
            yield return new WaitForEndOfFrame();
        }

        weight = Unwink;
        Head.SetBlendShapeWeight(1, weight);
    }
}
