using UnityEngine;
using System.Collections;

public class BearVisualCut : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer Head;
    [SerializeField] private float WinkTime = 0;
    private float Unwink = 0;

    private void Update()
    {
        WinkTime -= Time.unscaledDeltaTime;
        if (WinkTime < 0)
        {
            Unwink = 0;
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
