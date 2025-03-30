using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter = null;
    [SerializeField] private PlayerCamera playerCamera;
    [Space]
    //[SerializeField] private CameraSpring cameraSpring;
    //[SerializeField] private CameraLean cameraLean;

    private PlayerInputActions _inputActions;
    private CharacterState _state;

    void Start () {
        Cursor.lockState = CursorLockMode.Locked;

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());

        //cameraSpring.Initialize();
        //cameraLean.Initialize();
    }

    void OnDestroy () {
        _inputActions.Dispose();
    }

    public void DestroyInput () {
        _inputActions.Dispose();
    }

    void Update () {
        try {
            playerCharacter.enabled = true;
        }
        catch {
            SceneFadeController.instance.LoadScene(1);
        }

        var input = _inputActions.Gameplay;
        var deltaTime = Time.deltaTime;

        // Get Camera input and update its rotation
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        // Get character input and update it
        var CharacterInput = new CharacterInput {
            Rotation       = playerCamera.transform.rotation,
            Move           = input.Move.ReadValue<Vector2>(),
            Jump           = input.Jump.WasPressedThisFrame(),
            JumpSustain    = input.Jump.IsPressed(),             // MAYBE ADD A NEW INPUT CROUCH TOGGLE AND NORMAL CROUCH
            Crouch         = input.Crouch.WasPressedThisFrame() //|| input.Crouch.WasReleasedThisFrame () // Can be used to set crouching as toggle or hold
                ? CrouchInput.Toggle
                : CrouchInput.None
        };
        playerCharacter.UpdateInput(CharacterInput);
        playerCharacter.UpdateBody(deltaTime);
    }

    void LateUpdate() {
        var deltaTime = Time.deltaTime;
        var cameraTarget = playerCharacter.GetCameraTarget();
        var state = playerCharacter.GetState();
        
        playerCamera.UpdatePosition(cameraTarget);
        //cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);

        //cameraLean.UpdateLean(deltaTime, state.Acceleration, cameraTarget.up);
    }
}
