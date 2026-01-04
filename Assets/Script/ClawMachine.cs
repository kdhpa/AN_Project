using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem;

public class ClawMachine : MonoBehaviour
{
    private Vector2 moveVec;

    private GameObject claw;
    private GameObject joyStick;
    private GameObject crainTrain;
    private GameObject crainButton;

    private Vector2 input_move_vec;

    private void Awake()
    {
        claw = this.gameObject.GetNamedChild("Crain");
        joyStick = this.gameObject.GetNamedChild("JoyStick");
        crainTrain = this.gameObject.GetNamedChild("crainTrain");
        crainButton = this.gameObject.GetNamedChild("Button");
    }

    private void Start()
    {
        
    }

}
