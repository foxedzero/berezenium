
using System.Collections.Generic;
using UnityEngine;

public class FacilityWorkVisual : MonoBehaviour
{
    [SerializeField] private Facility Facility;

    [SerializeField] private GameObject[] Enables;

    [SerializeField] private Animator Animator;

    [SerializeField] private MeshRenderer MeshRenderer;
    [SerializeField] private Material[] WindowMaterial;
    [SerializeField] private int WindowMaterialIndex;

    private void Start()
    {
        Facility.OnSmthChange += UpdateState;
        UpdateState();
    }

    public void UpdateState()
    {
        bool work = Facility._Effectivity > 0 && Facility._Bears.Length > 0;
        foreach (GameObject game in Enables)
        {
            game.SetActive(work);
        }

        if(Animator != null)
        {
            Animator.SetBool("Work", work);
        }

        if(WindowMaterialIndex < 0)
        {
            return;
        }

        Material material;
        if(work && City._Energosystem._Effectivity > 0)
        {
            material = WindowMaterial[1];
        }
        else
        {
            material = WindowMaterial[0];
        }

        List<Material> materials = new List<Material>();
        MeshRenderer.GetSharedMaterials(materials);
        materials[WindowMaterialIndex] = material;
        MeshRenderer.SetMaterials(materials);
    }
}
