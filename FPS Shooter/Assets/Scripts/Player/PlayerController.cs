using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera PlayerCamera;

    [Header("General")]
    public float GravityDownForce = 20f;

    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask GroundCheckLayers = -1;

    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float GroundCheckDistance = 0.05f;

    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float MaxSpeedOnGround = 10f;

    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float MovementSharpnessOnGround = 15;

    [Tooltip("Max movement speed when not grounded")]
    public float MaxSpeedInAir = 10f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float RotationSpeed = 200f;

    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed multiplier when aiming")]
    public float AimingRotationMultiplier = 0.4f;

    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float JumpForce = 9f;

    [Header("Stance")]
    [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    public float CameraHeightRatio = 0.9f;

    [Tooltip("Height of character when standing")]
    public float CapsuleHeightStanding = 1.8f;

    [Header("Fall Damage")]
    [Tooltip("Whether the player will recieve damage when hitting the ground at high speed")]
    public bool RecievesFallDamage;

    [Tooltip("Minimun fall speed for recieving fall damage")]
    public float MinSpeedForFallDamage = 10f;

    [Tooltip("Fall speed for recieving th emaximum amount of fall damage")]
    public float MaxSpeedForFallDamage = 30f;

    [Tooltip("Damage recieved when falling at the mimimum speed")]
    public float FallDamageAtMinSpeed = 10f;

    [Tooltip("Damage recieved when falling at the maximum speed")]
    public float FallDamageAtMaxSpeed = 50f;

    public UnityAction<bool> OnStanceChanged;

    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded;
    public bool HasJumpedThisFrame { get; private set; }
    public bool IsDead { get; private set; }

    //public float RotationMultiplier
    //{
    //    get
    //    {
    //        return weaponsManager.IsAiming ? AimingRotationMultiplier : 1f;
    //    }
    //}

    Health health;
    PlayerInputHandler inputHandler;
    CharacterController controller;
    PlayerWeaponsManager weaponsManager;
    //Actor m_Actor;
    Vector3 m_GroundNormal;
    Vector3 m_CharacterVelocity;
    Vector3 m_LatestImpactSpeed;
    float m_LastTimeJumped = 0f;
    float cameraVerticalAngle = 0f;
    float m_FootstepDistanceCounter;
    float m_TargetCharacterHeight;

    const float k_GroundCheckDistanceInAir = 0.07f;

    void Awake()
    {
        ActorsManager actorsManager = FindFirstObjectByType<ActorsManager>();
        //if (actorsManager != null)
           // actorsManager.SetPlayer(gameObject);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
        weaponsManager = GetComponent<PlayerWeaponsManager>();
        //health = GetComponent<Health>();

        //m_Actor = GetComponent<Actor>();

        controller.enableOverlapRecovery = true;
       // health.OnDie += OnDie;
    }

    void Update()
    {
        HasJumpedThisFrame = false;
        bool wasGrounded = IsGrounded;

        GroundCheck();

        HandleCharacterMovement();
    }

    void OnDie()
    {
        IsDead = true;

        // Tell the weapons manager to switch to a non-existing weapon in order to lower the weapon
       // m_WeaponsManager.SwitchToWeaponIndex(-1, true);

        //TODO: EventManager.Broadcast(Events.PlayerDeathEvent);
    }

    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = controller.skinWidth + GroundCheckDistance;

        // reset values before the ground check
        IsGrounded = false;
        m_GroundNormal = Vector3.up;

        // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
        if (Physics.SphereCast(GetCapsuleBottomHemisphere(), controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
            QueryTriggerInteraction.Ignore))
        {
            // storing the upward direction for the surface found
            m_GroundNormal = hit.normal;

            // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
            // and if the slope angle is lower than the character controller's limit
            if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                IsNormalUnderSlopeLimit(m_GroundNormal))
            {
                IsGrounded = true;
            }
        }
    }

    void HandleCharacterMovement()
    {
        transform.Rotate(new Vector3(0f, (inputHandler.GetLookInputsHorizontal() * RotationSpeed),0f), Space.Self);

        cameraVerticalAngle += inputHandler.GetLookInputsVertical() * RotationSpeed;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
        PlayerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector(inputHandler.GetMoveInput());

        if (IsGrounded)
        {
            Vector3 targetVelocity = worldspaceMoveInput * MaxSpeedOnGround;
            // reduce speed if crouching by crouch speed ratio
            targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) *
                                targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                MovementSharpnessOnGround * Time.deltaTime);

            // jumping
            if (inputHandler.GetJumpInputDown())
            {
                CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                CharacterVelocity += Vector3.up * JumpForce;

                m_LastTimeJumped = Time.time;
                HasJumpedThisFrame = true;

                IsGrounded = false;
                m_GroundNormal = Vector3.up;
            }
        }
        else
        {
            CharacterVelocity += worldspaceMoveInput * Time.deltaTime;

            float verticalVelocity = CharacterVelocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir);
            CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

            CharacterVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
        }


        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(controller.height);
        controller.Move(CharacterVelocity * Time.deltaTime);

        // detect obstructions to adjust velocity accordingly
        m_LatestImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, controller.radius,
            CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            // We remember the last impact speed because the fall damage logic might need it
            m_LatestImpactSpeed = CharacterVelocity;

            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
        }
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= controller.slopeLimit;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * controller.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - controller.radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
}
