using System.Runtime.ConstrainedExecution;
using KinematicCharacterController;
using UnityEngine;

public enum CrouchInput{
    None, Toggle, Crouch, Uncrouch // ADDED CROUCH AND UNCROUCH
}

public enum Stance {
    Stand, Crouch, Slide
}

public struct CharacterState {
    public bool Grounded;
    public Stance Stance;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public struct CharacterInput {
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool JumpSustain;
    public CrouchInput Crouch;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform cameraTarget;
    [Space]
    public float walkSpeed = 15f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float walkResponse = 25f;
    [SerializeField] private float crouchResponse = 20f;
    [Space]
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float coyoteTime = 0.2f;
    [Range(0f, 1f)]
    [SerializeField] private float JumpSustainGravity = 0.6f; // Lower the gravity for sustained jumps
    [SerializeField] private float gravity = -90f;
    [Space]
    [SerializeField] private float slideStartSpeed = 25f;
    [SerializeField] private float slideEndSpeed = 15f;
    [SerializeField] private float slideFriction = 0.8f;
    [SerializeField] private float slideSteerAcceleration = 5f;
    [SerializeField] private float slideGravity = -90f; 
    [Space]
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float  crouchHeightResponse = 15f;
    [Space]
    [SerializeField] private float airSpeed = 15f; // Max speed while in air
    [SerializeField] private float airAcceleration = 70f; // How fast your speed in air will change
    [Range(0f, 1f)]
    [SerializeField] private float standCameraTargetHeight = 0.9f; // Normalized height between the top and bottom of character capsule
    [Range(0f, 1f)]
    [SerializeField] private float crouchCameraTargetHeight = 0.7f;

    private CharacterState _state;
    private CharacterState _tempState;
    private CharacterState _lastState;
    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedSustainedJump;
    // Made public so that sprinting ability works
    public bool _requestedCrouch;
    private bool _requestedCrouchInAir;
    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;
    private Collider[] _uncrouchOverlapResults;

    public void Initialize() {
        _state.Stance = Stance.Stand;
        _uncrouchOverlapResults = new Collider[8];

        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input) {
        _requestedRotation = input.Rotation;

        // Take the 2D input vector and create a 3D movement vector on the XZ plane
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        // Clamp the length to 1 to prevent moving faster diagonally with WASD
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1);
        // Orient the imput so its relative to the direction the player is facing
        _requestedMovement = input.Rotation * _requestedMovement;

        var wasRequestingJump = _requestedJump;
        // True if jump input pressed or if already true
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && !wasRequestingJump) {
            _timeSinceJumpRequest = 0f;
        }
        _requestedSustainedJump = input.JumpSustain;

        var wasRequestingCrouch = _requestedCrouch;
        _requestedCrouch = input.Crouch switch {
            CrouchInput.Toggle => !_requestedCrouch,
            CrouchInput.None => _requestedCrouch,
            CrouchInput.Crouch => _requestedCrouch, // ADDED BY ME
            CrouchInput.Uncrouch => !_requestedCrouch, // ADDED BY ME
            _=> _requestedCrouch

        };
        // Fixes phantom slide boost on hills too steep to climp
        if (_requestedCrouch && !wasRequestingCrouch) {
            _requestedCrouchInAir = !_state.Grounded;
        } else if (!_requestedCrouch && wasRequestingCrouch) {
            _requestedCrouchInAir = false;
        }
    }

    public void UpdateBody(float deltaTime) {
        var currentHeight = motor.Capsule.height;
        var normalizedHeight = currentHeight / standHeight;

        var CameraTargetHeight = currentHeight * (
            _state.Stance is Stance.Stand
                ? standCameraTargetHeight
                : crouchCameraTargetHeight
            );
        var rootTargetScale = new Vector3 (1f, normalizedHeight, 1f);
        
        cameraTarget.localPosition = Vector3.Lerp (cameraTarget.localPosition, new Vector3(0f, CameraTargetHeight, 0f), 1f - Mathf.Exp(-crouchHeightResponse * deltaTime));
        root.localScale = Vector3.Lerp (root.localScale, rootTargetScale, 1f - Mathf.Exp(-crouchHeightResponse * deltaTime));
    }

    public void AfterCharacterUpdate(float deltaTime){
        // Uncrouch
        if (!_requestedCrouch && _state.Stance is not Stance.Stand) {
            // Tentatively standup the character capsule
            motor.SetCapsuleDimensions(radius: motor.Capsule.radius, height: standHeight, yOffset: standHeight * 0.5f);

            // Then see if the capsule overlaps any colliders before allowing the character to standup
            // This function returns an int
            if (motor.CharacterOverlap(motor.TransientPosition, motor.TransientRotation, _uncrouchOverlapResults, motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0) {
                //Re-crouch
                _requestedCrouch = true;
                motor.SetCapsuleDimensions(radius: motor.Capsule.radius, height: crouchHeight, yOffset: crouchHeight * 0.5f);
            }
            else {
                _state.Stance = Stance.Stand;
            }
        }

        // Update state to reflect relevant motor properties
        _state.Grounded = motor.GroundingStatus.IsStableOnGround;
        _state.Velocity = motor.Velocity;
        // And update the _lastState to store the character state snapshot taken at the beginning of this character update
        _lastState = _tempState;
    }

    public void BeforeCharacterUpdate(float deltaTime){
        _tempState = _state;
        // Crouch
        if (_requestedCrouch && _state.Stance is Stance.Stand) {
            _state.Stance = Stance.Crouch;
            motor.SetCapsuleDimensions(radius: motor.Capsule.radius, height: crouchHeight, yOffset: crouchHeight * 0.5f);
        }
    }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnDiscreteCollisionDetected(Collider hitCollider){}

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}

    public void PostGroundingUpdate(float deltaTime){
        if (!motor.GroundingStatus.IsStableOnGround && _state.Stance is Stance.Slide) {
            _state.Stance = Stance.Crouch;
        }
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport){}

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        // Updates character rotation to face in the same diorection as requested rotation (camera rotation)
        // We dont want the character to pitch up and down, so the direction the character looks should be flattened
        // Do this by projectiong a vector pointing in the same direction that the player is looking onto a flat plane

        var forward = Vector3.ProjectOnPlane(_requestedRotation * Vector3.forward, motor.CharacterUp);
        if (forward != Vector3.zero)
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        _state.Acceleration = Vector3.zero;
             // If on the ground...
            if (motor.GroundingStatus.IsStableOnGround)
            {
                _timeSinceUngrounded = 0f;
                _ungroundedDueToJump = false;

                // Snap the requested movement direction to the angle of the surface the caracter is walking on
                var groundedMovement = motor.GetDirectionTangentToSurface(direction: _requestedMovement, surfaceNormal: motor.GroundingStatus.GroundNormal) * _requestedMovement.magnitude;

                // Start sliding.
                {
                    var moving = groundedMovement.sqrMagnitude > 0f;
                    var crouching = _state.Stance is Stance.Crouch;
                    var wasStanding = _lastState.Stance is Stance.Stand;
                    var wasInAir = !_lastState.Grounded;
                    if (moving && crouching && (wasInAir || wasStanding)) {
                        _state.Stance = Stance.Slide;

                        // When landing on stable ground the ccharacter motor projects the velocity onto a flat ground plane.
                        // This is normally good, because normally the player shouldnt slide when landing on the ground
                        // But in this case we want the player to slide
                        // Reproject the last frames falling velocity onto the ground normal to slide
                        if (wasInAir)
                        {
                            currentVelocity = Vector3.ProjectOnPlane(_lastState.Velocity, motor.GroundingStatus.GroundNormal);
                        }

                        // Also prevents pphantom slide boost
                        var effectiveSlideStartSpeed = slideStartSpeed;
                        if (!_lastState.Grounded && !_requestedCrouchInAir) {
                            effectiveSlideStartSpeed = 0f;
                            _requestedCrouchInAir = false;
                        }

                        var slideSpeed = Mathf.Max(effectiveSlideStartSpeed, currentVelocity.magnitude);
                        currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, motor.GroundingStatus.GroundNormal) * slideSpeed;
                    }
                }

                // Move.
                if (_state.Stance is Stance.Stand or Stance.Crouch)
                {
                    // Set speed and responsiveness based on when crouching or walking
                    var speed = _state.Stance is Stance.Stand
                        ? walkSpeed
                        : crouchSpeed;
                    var response = _state.Stance is Stance.Stand
                        ? walkResponse
                        : crouchResponse;

                    // and smoothly move along the ground in that direction
                    var targetVelocity = groundedMovement * speed;
                    var moveVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Exp(-response * deltaTime));
                    _state.Acceleration = moveVelocity - currentVelocity;
                    currentVelocity = moveVelocity;
                } 
                // Continue sliding
                else {
                    // Friction
                    currentVelocity -= currentVelocity * (slideFriction * deltaTime);

                    // Slope
                    {
                        var force = Vector3.ProjectOnPlane (-motor.CharacterUp, motor.GroundingStatus.GroundNormal) * slideGravity;

                        currentVelocity -= force * deltaTime;
                    }

                    // Steer
                    {
                        // Tarrget velocity is the players movement direction, but at the current speed
                        var currentSpeed = currentVelocity.magnitude;
                        var targetVelocity = groundedMovement * currentSpeed;
                        var steerVelocity = currentVelocity;
                        var steerForce = (targetVelocity - steerVelocity) * slideSteerAcceleration * deltaTime;
                        // Add steer force but clamp speed so the slide doesent accelerate due to direction movement input
                        steerVelocity += steerForce;
                        steerVelocity = Vector3.ClampMagnitude(steerVelocity, currentSpeed);

                        _state.Acceleration = steerVelocity - currentVelocity;
                        currentVelocity = steerVelocity;
                    }

                    // Stop
                    if (currentVelocity.magnitude < slideEndSpeed) {
                        _state.Stance = Stance.Crouch;
                    }
                }

            }// else, in the air 
            else {
            _timeSinceUngrounded += deltaTime;

            // Movement
            if (_requestedMovement.sqrMagnitude > 0f) {
                var planarMovement = Vector3.ProjectOnPlane(_requestedMovement, motor.CharacterUp) * _requestedMovement.magnitude;

                // Current velocity on movement plane
                var currentPlanarVelocity = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

                // Calculate movement force
                var movementForce = planarMovement * airAcceleration * deltaTime;

                // If moving slowerr than the max air speeed, treat movementForce as a simple steering force
                if (currentPlanarVelocity.magnitude < airSpeed) {
                    // Add it to the current pllanar velocity for a target velocity, limited to airspeed
                    var targetPlanarVelocity = Vector3.ClampMagnitude(currentPlanarVelocity + movementForce, airSpeed);

                    // Steer towards target velocity
                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                }
                // Otherwise, nerf the movement force when it is in the direction of the current planar velocity to prevent going past the max air speed
                else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0f) {
                    //Project movementForce onto the plane whos normal is the current planar velocity
                    var constrainedMovementForce = Vector3.ProjectOnPlane (movementForce, currentPlanarVelocity.normalized);
                    movementForce = constrainedMovementForce;
                }

                // Prevent air-climbing steep slopes
                if (motor.GroundingStatus.FoundAnyGround) {
                    // If moving in the same direction as the resultant velocity
                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f) {
                        var obstructionNormal = Vector3.Cross (motor.CharacterUp, Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal)).normalized;

                        // Project movement force onto obstruction plane
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                    }
                }

                // Steer towards current velocity
                currentVelocity += movementForce;
            }
            // Gravity
            var effectiveGravity = gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (_requestedSustainedJump && verticalSpeed > 0f) {
                    effectiveGravity *= JumpSustainGravity;
            }
            currentVelocity += motor.CharacterUp * effectiveGravity * deltaTime;
        }

        if (_requestedJump) {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteTime = _timeSinceUngrounded < coyoteTime;

            if (grounded || canCoyoteTime && !_ungroundedDueToJump) {
            _requestedJump = false; // Unset jump request
             _requestedCrouch = false; // And request the character uncrouches
             _requestedCrouchInAir = false;

            // Unstick player from ground
            motor.ForceUnground(time: 0);
            _ungroundedDueToJump = true;

            // Set minimum verticle speed to the jump speed instead of a jump force
            var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
            // Add the difference in the current vertical speed and target vertical speed to the players velocity
            currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            } else {
                _timeSinceJumpRequest += deltaTime;

                // Defer the jump until coyote time has passed
                var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                _requestedJump = canJumpLater;
            }
        }

    }

    // Used to spawn the player
    public void SetPosition(Vector3 position, bool killVelocity = true) {
        motor.SetPosition(position);
        if (killVelocity) {
            motor.BaseVelocity = Vector3.zero;
        }
    }

    private float elapsedTime;
    public float LerpPosition (Vector3 start, Vector3 end, float duration, bool killVelocity) {
        elapsedTime += Time.deltaTime;
        float percentComplete = elapsedTime / duration;

        motor.SetPosition(Vector3.Lerp(start, end, percentComplete));
        return percentComplete;
    }

    public Transform GetCameraTarget () => cameraTarget;

    public CharacterState GetState() => _state;
    public CharacterState GetLastState() => _lastState;
}
