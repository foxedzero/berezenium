using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerPlacer : MonoBehaviour
{
    [SerializeField] private PlayerMovement Player;
    [SerializeField] private CharacterController Controller;

    public string _SaveInfo => $"X({Player.transform.position.x})Y({Player.transform.position.y})Z({Player.transform.position.z})XRot({Player._Rotation.x})YRot({Player._Rotation.y})";

    public void PlacePlayer(string data)
    {
        Controller.enabled = false;
        Dictionary<string, string> parameters = StaticTools.GetParameters(data);
        Player.transform.position = new Vector3(StaticTools.StringToFloat(parameters["X"]), StaticTools.StringToFloat(parameters["Y"]), StaticTools.StringToFloat(parameters["Z"]));
        Player._Rotation = new Vector3(StaticTools.StringToFloat(parameters["XRot"]), StaticTools.StringToFloat(parameters["YRot"]), 0);
    }
     
    private void Start()
    {
        Controller.enabled = true;
    }
}
