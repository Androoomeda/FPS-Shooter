using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera PlayerCamera;

    [Header("General")]
    public float GravityDownForce = 9.8f;

    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask GroundCheckLayers = -1;

    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float MaxSpeedOnGround = 10f;

    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float JumpForce = 9f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float RotationSpeed = 200f;

    public Vector3 CharacterVelocity;

    private CharacterController controller;
    private PlayerInputHandler inputHandler;
    private Vector3 m_GroundNormal;
    private float cameraVerticalAngle = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        m_GroundNormal = Vector3.up;
        Movement();
    }

    private void Movement()
    {
        transform.Rotate(new Vector3(0f, (inputHandler.GetLookInputsHorizontal() * RotationSpeed), 0f), Space.Self);

        cameraVerticalAngle += inputHandler.GetLookInputsVertical() * RotationSpeed;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
        PlayerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

        Vector3 worldspaceMoveInput = transform.TransformVector(inputHandler.GetMoveInput());
        Vector3 targetVelocity = worldspaceMoveInput * MaxSpeedOnGround;
        targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) *
                            targetVelocity.magnitude;

        if(!controller.isGrounded)
            CharacterVelocity.y -= GravityDownForce * Time.deltaTime;

        CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, Time.deltaTime);
        controller.Move(CharacterVelocity * Time.deltaTime);

        if(controller.isGrounded && inputHandler.GetJumpInputDown())
            CharacterVelocity += Vector3.up * JumpForce;
    }

    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
}
