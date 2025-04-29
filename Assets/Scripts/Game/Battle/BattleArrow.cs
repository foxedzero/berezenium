using UnityEngine;

public class BattleArrow : MonoBehaviour
{
   // [SerializeField] private BattleProvider BattleProvider;

   // [SerializeField] private MeshRenderer[] Meshes;

   // [SerializeField] private Transform Arrow;
   // [SerializeField] private Transform Bar;

   // public void Show(bool state)
   // {
   //     Arrow.gameObject.SetActive(state);
   //     Bar.gameObject.SetActive(state);
   // }

   //public void Set(Vector3 start, Vector3 end)
   // {
   //     float distance = Vector3.Distance(start, end);
     
   //     if(distance < 1.5f)
   //     {
   //         return;
   //     }
        
   //     Vector3 direction = (end - start)/ distance;
   //     Arrow.position = end - direction * 0.5f;

   //     Arrow.rotation = Quaternion.LookRotation(direction);
   //     Bar.rotation = Quaternion.LookRotation(direction);

   //     Bar.position = (start + end) / 2f;
   //     Bar.localScale = new Vector3(1, 1, distance - 2.5f);
   // }

   // public void SetColor(Battler.Actions action)
   // {
   //     foreach(MeshRenderer mesh in Meshes)
   //     {
   //         mesh.material = BattleProvider._ActionMaterial[action.GetHashCode()];
   //     }
   // }
}
