using UnityEngine;

public class BearModelData : MonoBehaviour
{
    [Header("Пасечник")]
    [SerializeField] private GameObject PasechnikPrefab;
    [SerializeField] private Mesh[] PasechnikBrows;
    [SerializeField] private Mesh[] PasechnikHeads;
    [Header("Конструктор")]
    [SerializeField] private GameObject ConstructorPrefab;
    [SerializeField] private Mesh[] ConstructorBrows;
    [SerializeField] private Mesh[] ConstructorHeads;
    [Header("Программист")]
    [SerializeField] private GameObject ProgramistPrefab;
    [SerializeField] private Mesh[] ProgramistBrows;
    [SerializeField] private Mesh[] ProgramistHeads;
    [Header("Биоинженер")]
    [SerializeField] private GameObject BioingenerPrefab;
    [SerializeField] private Mesh[] BioingenerBrows;
    [SerializeField] private Mesh[] BioingenerHeads;
    [Header("Первопроходец")]
    [SerializeField] private GameObject PervoprohodecPrefab;
    [SerializeField] private Mesh[] PervoprohodecBrows;
    [SerializeField] private Mesh[] PervoprohodecHeads;

    public GameObject _PasechnikPrefab => PasechnikPrefab;
    public Mesh[] _PasechnikBrows => PasechnikBrows;
    public Mesh[] _PasechnikHeads => PasechnikHeads;


    public GameObject _ConstructorPrefab => ConstructorPrefab;
    public Mesh[] _ConstructorBrows => ConstructorBrows;
    public Mesh[] _ConstructorHeads => ConstructorHeads;


    public GameObject _ProgramistPrefab => ProgramistPrefab;
    public Mesh[] _ProgramistBrows => ProgramistBrows;
    public Mesh[] _ProgramistHeads => ProgramistHeads;


    public GameObject _BioingenerPrefab => BioingenerPrefab;
    public Mesh[] _BioingenerBrows => BioingenerBrows;
    public Mesh[] _BioingenerHeads => BioingenerHeads;


    public GameObject _PervoprohodecPrefab => PervoprohodecPrefab;
    public Mesh[] _PervoprohodecBrows => PervoprohodecBrows;
    public Mesh[] _PervoprohodecHeads => PervoprohodecHeads;
}
